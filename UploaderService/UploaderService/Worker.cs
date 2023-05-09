using System.Net.Http.Json;


namespace UploaderService
{
    public class Worker : BackgroundService
    {
        /// <summary>
        /// Class for deserializing the server response
        /// </summary>
        private class JsonResponse
        {
            public LayoutFile? value { get; set; }
            public int statusCode { get; set; }
        }

        private const string SBORKA_DRIVE_API_IP = "SborkaDriveAPI:IP";
        private const string SBORKA_DRIVE_API_REQUEST_NEXT = "SborkaDriveAPI:RequestNext";
        private const string SBORKA_DRIVE_API_REQUEST_UPLOADED = "SborkaDriveAPI:RequestUploaded";
        private const string AUTHENTICATION_KEY = "Authentication:Key";
        private const string API_KEY_HEADER_NAME = "X-Api-Key";

        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;

        private HttpClient _client = new HttpClient();
        private Uploader _uploader;

        //constructor
        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Keeps track of new files to upload and upload them
        /// </summary>
        /// <param name="stoppingToken">Notification that operations should be canceled</param>
        /// <returns>Task object representing the asynchronous operation</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string apiIp = _config.GetValue<string>(SBORKA_DRIVE_API_IP);
            string apiRequest = _config.GetValue<string>(SBORKA_DRIVE_API_REQUEST_NEXT);
            string apiUrl = @$"https://{apiIp}/{apiRequest}";

            HttpResponseMessage response;
            while (!stoppingToken.IsCancellationRequested)
            {
                bool hasError = true;
                try
                {
                    //Get next file info
                    response = await _client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        JsonResponse? jsonResponse = await response.Content.ReadFromJsonAsync<JsonResponse>();
                        if (jsonResponse is not null &&
                            jsonResponse.statusCode == 200 &&
                            jsonResponse.value is not null)
                        {
                            //Upload file if needed
                            string? result = await _uploader.UploadFile(jsonResponse.value);
                            //Notify that file is uploaded
                            if (result is not null)
                            {
                                await OnUploaded(jsonResponse.value.OrderId, result);
                            }
                            _logger.LogInformation("File {name} : {result}", jsonResponse.value.Name, result);
                            hasError = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
                    _logger.LogError("(Function ExecuteAsync), error: {message}", ex.Message);
                }
                //If error or no files to download, wait 5 seconds
                if (hasError)
                    await Task.Delay(5000, stoppingToken);
            }
        }

        /// <summary>
        /// Notifies that file is uploaded
        /// </summary>
        /// <param name="orderId">Order id whose file was uploaded</param>
        /// <param name="googleFileId">ID assigned by Google to the uploaded file</param>
        /// <returns>Task object representing the asynchronous operation</returns>
        private async Task OnUploaded(int orderId, string googleFileId)
        {
            string apiIp = _config.GetValue<string>(SBORKA_DRIVE_API_IP);
            string apiRequest = _config.GetValue<string>(SBORKA_DRIVE_API_REQUEST_UPLOADED);
            string apiUrl = @$"https://{apiIp}/{apiRequest}";
            try
            {
                //Update information in the database
                var response = await _client.PutAsync(string.Format(apiUrl, orderId, googleFileId), null);
            }
            catch (Exception ex)
            {
                _logger.LogError("(Function OnUploaded), error: {message}", ex.Message);
            }
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>Task object representing the asynchronous operation</returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            string authKey = _config.GetValue<string>(AUTHENTICATION_KEY);
            _client.DefaultRequestHeaders.Add(API_KEY_HEADER_NAME, authKey);
            _uploader = new Uploader(_config);
            return base.StartAsync(cancellationToken);
        }
    }
}