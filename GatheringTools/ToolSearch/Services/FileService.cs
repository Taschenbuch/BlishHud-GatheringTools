using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using HttpClient = System.Net.Http.HttpClient;

namespace GatheringTools.ToolSearch.Services
{
    public class FileService
    {
        public static async Task<bool> IsModuleVersionDeprecated()
        {
            try
            {
                await GetTextFromUrl(DEPRECATED_URL);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task UpdateDataInModuleFolderIfNecessary(DirectoriesManager directoriesManager, Logger logger)
        {
            try
            {
                var moduleFolderPath = directoriesManager.GetFullDirectoryPath(MODULE_FOLDER_NAME);
                var areAnyFilesMissingInModuleFolder = AreAnyFilesMissing(moduleFolderPath, DATA_RELATIVE_FILE_PATHS);
                if (areAnyFilesMissingInModuleFolder)
                {
                    await DownloadFiles(BASE_URL, moduleFolderPath, DATA_RELATIVE_FILE_PATHS);
                    return;
                }

                var isNewerDataAvailableOnline = await IsNewerDataAvailableOnline(moduleFolderPath);
                if(isNewerDataAvailableOnline)
                    await DownloadFiles(BASE_URL, moduleFolderPath, DATA_RELATIVE_FILE_PATHS);
            }
            catch (Exception e)
            {
                logger.Warn(e, "Failed to update module data from online host. :("); // just a warn, because module may still work. check for empty gathering tools is somewhere else
            }
        }

        private static async Task DownloadFiles(string baseUrl, string moduleFolderPath, List<string> relativeFilePaths)
        {
            foreach (var relativeFilePath in relativeFilePaths)
            {
                var filePath = Path.Combine(moduleFolderPath, relativeFilePath);
                var fileUrl = Path.Combine(baseUrl, relativeFilePath);
                var fileContent = await GetTextFromUrl(fileUrl); // could be optimized by awaiting multiple at once
                await WriteFileAsync(fileContent, filePath);
            }
        }

        private static async Task WriteFileAsync(string fileContent, string filePath)
        {
            var fileFolder = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(fileFolder);
            using var streamWriter = new StreamWriter(filePath);
            await streamWriter.WriteAsync(fileContent);
            await streamWriter.FlushAsync();
        }

        private static async Task<bool> IsNewerDataAvailableOnline(string moduleDataFolderPath)
        {
            var onlineDataVersion = await GetOnlineVersion();
            var localDataVersion = GetLocalVersion(moduleDataFolderPath);
            return onlineDataVersion > localDataVersion;
        }

        private static int GetLocalVersion(string moduleDataFolderPath)
        {
            var versionFilePath = Path.Combine(moduleDataFolderPath, CONTENT_VERSION_RELATIVE_FILE_PATH);
            var versionText = File.ReadAllText(versionFilePath);
            return int.Parse(versionText);
        }

        private static async Task<int> GetOnlineVersion()
        {
            var versionUrl = Path.Combine(BASE_URL, CONTENT_VERSION_RELATIVE_FILE_PATH);
            var versionText = await GetTextFromUrl(versionUrl);
            return int.Parse(versionText);
        }

        private static async Task<string> GetTextFromUrl(string url)
        {
            // dont add try catch. checking if module is deprecated relies on this throwing an exception when deprecated.txt file is not found
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url);
        }

        private static bool AreAnyFilesMissing(string rootFilePath, List<string> relativeFilePaths)
        {
            return relativeFilePaths
                .Select(relative => Path.Combine(rootFilePath, relative))
                .Any(filePath => !File.Exists(filePath));
        }

        // todo strings -> overengineered, ggf. Path object bauen das alle für einen zusammensetzt? "paths.ModuleDataFolderName"
        public static readonly string MODULE_FOLDER_NAME = "gathering-tools-module-data";
        static readonly string format_version = "1";
        static readonly string BASE_URL = "https://bhm.blishhud.com/ecksofa.gatheringtools";
        static readonly string DEPRECATED_URL = $"https://bhm.blishhud.com/ecksofa.gatheringtools/data/format_version_{format_version}/deprecated.txt";
        static readonly string CONTENT_VERSION_RELATIVE_FILE_PATH = $"data/format_version_{format_version}/content_version.txt";
        public static readonly string GATHERING_TOOLS_FROM_V2_ITEMS_API_RELATIVE_FILE_PATH = $"data/format_version_{format_version}/gatheringToolsFromV2ItemsApi.json";
        public static readonly string GATHERING_TOOLS_MISSING_IN_V2_ITEMS_API_RELATIVE_FILE_PATH = $"data/format_version_{format_version}/gatheringToolsMissingInV2ItemsApi.json";
        static readonly List<string> DATA_RELATIVE_FILE_PATHS = new List<string>()
        {
            CONTENT_VERSION_RELATIVE_FILE_PATH,
            GATHERING_TOOLS_FROM_V2_ITEMS_API_RELATIVE_FILE_PATH,
            GATHERING_TOOLS_MISSING_IN_V2_ITEMS_API_RELATIVE_FILE_PATH
        };
    }
}