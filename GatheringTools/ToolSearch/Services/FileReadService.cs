using Blish_HUD.Modules.Managers;
using Blish_HUD;
using GatheringTools.ToolSearch.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatheringTools.ToolSearch.Services
{
    internal class FileReadService
    {
        public static async Task<IEnumerable<GatheringTool>> GetAllGatheringToolsFromFiles(DirectoriesManager directoriesManager, Logger logger)
        {
            try
            {
                var moduleFolderPath = directoriesManager.GetFullDirectoryPath(FileService.MODULE_FOLDER_NAME);
                var v2GatheringToolsTask = GetGatheringToolsFromFile(
                    Path.Combine(moduleFolderPath, FileService.GATHERING_TOOLS_FROM_V2_ITEMS_API_RELATIVE_FILE_PATH),
                    logger);

                var missingInV2GatheringToolsTask = GetGatheringToolsFromFile(
                    Path.Combine(moduleFolderPath, FileService.GATHERING_TOOLS_MISSING_IN_V2_ITEMS_API_RELATIVE_FILE_PATH),
                    logger);

                await Task.WhenAll(v2GatheringToolsTask, missingInV2GatheringToolsTask);
                return v2GatheringToolsTask.Result.Concat(missingInV2GatheringToolsTask.Result);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to load gathering tools from files. A Blish HUD or module restart may fix that issue. :(");
                return new List<GatheringTool>();
            }
        }

        private static async Task<List<GatheringTool>> GetGatheringToolsFromFile(string filePath, Logger logger)
        {
            try
            {
                using var streamReader = new StreamReader(filePath);
                var json = await streamReader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<GatheringTool>>(json);
            }
            catch (Exception e)
            {
                logger.Error(e, $@"Failed to read {filePath}. :(");
                return new List<GatheringTool>();
            }
        }
    }
}
