using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System.Reflection;

namespace UploaderService
{
    /// <summary>
    /// Upload a file to Google Drive
    /// </summary>
    internal class Uploader
    {
        public readonly string ACCOUNT_KEY_FILE;
        private const string DIRECTORY_ID = "GoogleDrive:DirectoryId";

        private readonly IConfiguration _config;

        //constructor
        public Uploader(IConfiguration config)
        {
            _config = config;
            string accountKeyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            ACCOUNT_KEY_FILE = Path.Combine(accountKeyPath, "SborkaDriveKey.json");
        }

        /// <summary>
        /// Uploads a file to Google Drive
        /// </summary>
        /// <param name="lf">Information about the uploading file</param>
        /// <returns>ID assigned by Google to the uploaded file</returns>
        public async Task<string?> UploadFile(LayoutFile lf)
        {
            //Load the service account credentials and define the scope of its access
            var credential = GoogleCredential.FromFile(ACCOUNT_KEY_FILE)
                .CreateScoped(DriveService.ScopeConstants.Drive);

            //Create the Drive service
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            //Upload file metadata
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = lf.Name,
                Parents = new List<string>() { _config.GetValue<string>(DIRECTORY_ID) }
            };

            //Create a new file on Google Drive
            try
            {
                await using (var fs = new FileStream(Path.Combine(lf.Path, lf.Name), FileMode.Open, FileAccess.Read))
                {
                    var request = service.Files.Create(fileMetadata, fs, "text/plain");
                    request.Fields = "*";
                    var result = await request.UploadAsync(CancellationToken.None);
                    return request.ResponseBody?.Id;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
