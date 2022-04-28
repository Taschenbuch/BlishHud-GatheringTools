using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Newtonsoft.Json;

namespace GatheringTools.ToolSearch
{
    public class FileService
    {
        public static async Task<List<GatheringTool>> GetAllGatheringToolsFromFile(ContentsManager contentsManager, Logger logger)
        {
            try
            {
                using (var fileStream = contentsManager.GetFileStream("gatheringTools.json"))
                using (var streamReader = new StreamReader(fileStream))
                {
                    var json = await streamReader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<List<GatheringTool>>(json);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to read ref/gatheringTools.json. :(");
                return new List<GatheringTool>();
            }
        }
    }
}