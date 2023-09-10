using Blish_HUD;
using GatheringTools.ToolSearch.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GatheringTools.ToolSearch.Services.RemoteFiles;

namespace GatheringTools.ToolSearch.Services
{
    internal class FileReadService
    {
        public static async Task<IEnumerable<GatheringTool>> GetAllGatheringToolsFromFiles(
            LocalAndRemoteFileLocations localAndRemoteFileLocations,
            Logger logger)
        {
            try
            {
                var v2GatheringToolsTask = GetGatheringToolsFromFile(
                    localAndRemoteFileLocations.GetLocalFilePath(FileConstants.GatheringToolsFromV2ItemsApiFileName),
                    logger);

                var missingInV2GatheringToolsTask = GetGatheringToolsFromFile(
                    localAndRemoteFileLocations.GetLocalFilePath(FileConstants.GatheringToolsMissingInV2ItemsApiFileName),
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
