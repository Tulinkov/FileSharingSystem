using CommunityToolkit.Mvvm.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using SborkaDownloader.Model;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System;
using System.Net.Http.Json;
using SborkaDownloader.Helper;

namespace SborkaDownloader.ViewModel
{
    public partial class DownloaderVM : ObservableObject
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
        private const string SBORKA_DRIVE_API_REQUEST_DELETE = "SborkaDriveAPI:RequestDelete";
        private const string AUTHENTICATION_KEY = "Authentication:Key";
        private const string API_KEY_HEADER_NAME = "X-Api-Key";

        private readonly string API_NEXT_URL;
        private readonly string API_DELETE_URL;

        private HttpClient _client = new HttpClient();
        private Downloader _downloader = new Downloader();

        [ObservableProperty]
        string status;

        [ObservableProperty]
        ObservableCollection<LayoutFile> layoutFiles = new ObservableCollection<LayoutFile>();

        //constructor
        public DownloaderVM()
        {
            string apiIp = App.Config.GetSection(SBORKA_DRIVE_API_IP).Value ?? "";
            string apiRequestNext = App.Config.GetSection(SBORKA_DRIVE_API_REQUEST_NEXT).Value ?? "";
            string apiRequestDelete = App.Config.GetSection(SBORKA_DRIVE_API_REQUEST_DELETE).Value ?? "";
            API_NEXT_URL = @$"https://{apiIp}/{apiRequestNext}";
            API_DELETE_URL = @$"https://{apiIp}/{apiRequestDelete}";

            string authKey = App.Config.GetSection(AUTHENTICATION_KEY).Value ?? "";
            _client.DefaultRequestHeaders.Add(API_KEY_HEADER_NAME, authKey);

            BackgroundTask();
        }

        /// <summary>
        /// Keeps track of new files to download and download them
        /// </summary>
        /// <returns>Task object representing the asynchronous operation</returns>
        private async Task BackgroundTask()
        {
            bool hasError = false;
            while (true)
            {
                if (hasError)
                    await Task.Delay(5000);
                hasError = true;
                try
                {
                    Status = "Getting information about a file to upload";
                    var response = await _client.GetAsync(API_NEXT_URL);
                    LayoutFile lf;
                    if (response.IsSuccessStatusCode)
                    {
                        //Get the file information
                        JsonResponse? jsonResponse = await response.Content.ReadFromJsonAsync<JsonResponse>();
                        if (jsonResponse is null ||
                            jsonResponse.statusCode != 200 ||
                            jsonResponse.value is null)
                            continue;
                        lf = jsonResponse.value;
                        layoutFiles.Insert(0, lf);
                        //Download the file
                        Status = $"Downloading the \"{lf.Name}\" file";
                        if (!await _downloader.DownloadFile(lf))
                            continue;
                    }
                    else
                        continue;
                    //Delete the downloaded file from Google Drive
                    Status = $"Deleting the \"{lf.Name}\" file";
                    response = await _client.DeleteAsync(string.Format(API_DELETE_URL, lf.OrderId));
                    if (response.IsSuccessStatusCode)
                    {
                        hasError = false;
                        await _downloader.DeleteFile(lf);
                    }
                }
                catch (Exception ex) { 
                    var d = ""; }
                finally
                {
                    Status = "";
                }
            }
        }

    }
}
