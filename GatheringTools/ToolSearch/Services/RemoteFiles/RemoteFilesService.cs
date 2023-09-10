using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blish_HUD;

namespace GatheringTools.ToolSearch.Services.RemoteFiles
{
    public class RemoteFilesService
    {
        public static async Task<bool> IsModuleVersionDeprecated(string deprecatedUrl)
        {
            try
            {
                await GetTextFromUrl(deprecatedUrl);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task UpdateLocalWithRemoteFilesIfNecessary(LocalAndRemoteFileLocations localAndRemoteFileLocations, Logger logger)
        {
            try
            {
                var areLocalFilesMissing = AreLocalFilesMissing(localAndRemoteFileLocations.DataFileLocations);
                if (areLocalFilesMissing)
                {
                    await DownloadFilesFromRemote(localAndRemoteFileLocations.DataFileLocations);
                    return;
                }

                var areNewerRemoteFilesAvailable = await AreNewerRemoteFilesAvailable(localAndRemoteFileLocations.ContentVersionFilePath);
                if (areNewerRemoteFilesAvailable)
                    await DownloadFilesFromRemote(localAndRemoteFileLocations.DataFileLocations);
            }
            catch (Exception e)
            {
                // error because there is no fallback data in ref folder. Module may stop working completely without this data
                logger.Error(e, "Failed to update module data from online host. :("); 
            }
        }

        private static async Task DownloadFilesFromRemote(List<FileLocation> dataFilePaths)
        {
            foreach (var dataFilePath in dataFilePaths)
            {
                var remoteFileContent = await GetTextFromUrl(dataFilePath.RemoteUrl); // could be optimized by awaiting multiple at once
                await WriteFileAsync(remoteFileContent, dataFilePath.LocalFilePath);
            }
        }

        private static async Task WriteFileAsync(string fileContent, string filePath)
        {
            var folderPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(folderPath);
            using var streamWriter = new StreamWriter(filePath);
            await streamWriter.WriteAsync(fileContent);
            await streamWriter.FlushAsync();
        }

        private static async Task<bool> AreNewerRemoteFilesAvailable(FileLocation contentVersionFilePath)
        {
            var onlineDataVersion = await GetRemoteVersion(contentVersionFilePath.RemoteUrl);
            var localDataVersion = GetLocalVersion(contentVersionFilePath.LocalFilePath);
            return onlineDataVersion > localDataVersion;
        }

        private static int GetLocalVersion(string contentVersionLocalFilePath)
        {
            var versionText = File.ReadAllText(contentVersionLocalFilePath);
            return int.Parse(versionText);
        }

        private static async Task<int> GetRemoteVersion(string contentVersionRemoteUrl)
        {
            var versionText = await GetTextFromUrl(contentVersionRemoteUrl);
            return int.Parse(versionText);
        }

        private static async Task<string> GetTextFromUrl(string url)
        {
            // dont add try catch. checking if module is deprecated relies on this throwing an exception when deprecated.txt file is not found
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url);
        }

        private static bool AreLocalFilesMissing(List<FileLocation> dataFilePaths)
        {
            return dataFilePaths.Any(d => !File.Exists(d.LocalFilePath));
        }
    }
}