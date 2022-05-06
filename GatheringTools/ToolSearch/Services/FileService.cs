using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using GatheringTools.ToolSearch.Model;
using Newtonsoft.Json;

namespace GatheringTools.ToolSearch.Services
{
    public class FileService
    {
        public static async Task<IEnumerable<GatheringTool>> GetAllGatheringToolsFromFiles(ContentsManager contentsManager, Logger logger)
        {
            var knownGatheringToolsTask   = GetGatheringToolsFromFile(@"toolSearch\data\gatheringToolsFromV2ItemsApi.json", contentsManager, logger);
            var unknownGatheringToolsTask = GetGatheringToolsFromFile(@"toolSearch\data\gatheringToolsMissingInV2ItemsApi.json", contentsManager, logger);
            await Task.WhenAll(knownGatheringToolsTask, unknownGatheringToolsTask);
            return knownGatheringToolsTask.Result.Concat(unknownGatheringToolsTask.Result);
        }

        private static async Task<List<GatheringTool>> GetGatheringToolsFromFile(string filePath, ContentsManager contentsManager, Logger logger)
        {
            try
            {
                using (var fileStream = contentsManager.GetFileStream(filePath))
                using (var streamReader = new StreamReader(fileStream))
                {
                    var json = await streamReader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<List<GatheringTool>>(json);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $@"Failed to read ref\{filePath}. :(");
                return new List<GatheringTool>();
            }
        }
    }
}