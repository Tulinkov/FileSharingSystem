using SborkaDownloader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;
using System.Reflection;

namespace SborkaDownloader.Helper
{
    internal class Downloader
    {
        public readonly string ACCOUNT_KEY_FILE;
        private const string DOWNLOAD_DIRECTORY = "FileStorage";

        private DriveService _driveService;

        //constructor
        public Downloader()
        {
            string accountKeyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            ACCOUNT_KEY_FILE = Path.Combine(accountKeyPath, "SborkaDriveKey.json");

            // Google Drive credential
            GoogleCredential credential = GoogleCredential
                    .FromFile(ACCOUNT_KEY_FILE)
                    .CreateScoped(DriveService.Scope.Drive);
            // Create Drive API service.
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });
        }

        /// <summary>
        /// Downloads a file from Google Drive
        /// </summary>
        /// <param name="file">File information</param>
        /// <returns>True if success, otherwise False</returns>
        public async Task<bool> DownloadFile(LayoutFile file)
        {
            bool result = false;
            try
            {
                var request = _driveService.Files.Get(file.GoogleFileId);
                request.Fields = "*";
                var googleFile = request.Execute();
                file.FileSize = googleFile.Size ?? 100;

                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                    progress =>
                    {
                        switch (progress.Status)
                        {
                            //Update progress bar
                            case DownloadStatus.Downloading:
                                file.Progress = (int)progress.BytesDownloaded;
                                break;
                            case DownloadStatus.Completed:
                                result = true;
                                file.Progress = file.FileSize;
                                break;
                            case DownloadStatus.Failed:
                                file.Progress = 0;
                                break;
                        }
                    };
                //Download and save
                string filePath = Path.Combine(App.Config.GetSection(DOWNLOAD_DIRECTORY).Value ?? "", file.Name);
                await using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await request.DownloadAsync(fs);
                }
                return result;
            }
            catch (Exception ex)
            {
                file.Progress = 0;
                return false;
            }
        }

        /// <summary>
        /// Deletes a file from Google Drive
        /// </summary>
        /// <param name="file">File information</param>
        /// <returns>Task object representing the asynchronous operation</returns>
        public async Task DeleteFile(LayoutFile file)
        {
            await _driveService.Files.Delete(file.GoogleFileId).ExecuteAsync();
        }
    }
}
