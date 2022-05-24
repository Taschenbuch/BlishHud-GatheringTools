using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GatheringTools.ToolSearch.Model;
using ItemJsonFileCreator.Model;
using Newtonsoft.Json;
using File = System.IO.File;

namespace ItemJsonFileCreator
{
    class Program
    {
        private const string INPUT_FOLDER_PATH_WITH_JSON_FILES = @"c:\gw2\items";
        private const string OUTPUT_JSON_FILE_PATH = @"c:\gw2\gatheringTools.json";

        static void Main()
        {
            var items          = ParseItemsFromJsonFiles(INPUT_FOLDER_PATH_WITH_JSON_FILES);
            var gatheringTools = FindGatheringTools(items);
            WriteToJsonOutputFile(gatheringTools, OUTPUT_JSON_FILE_PATH);
        }

        private static List<Gw2Item> ParseItemsFromJsonFiles(string inputFolderPathWithJsonFiles)
        {
            var filePaths = Directory.GetFiles(inputFolderPathWithJsonFiles);
            var items     = new List<Gw2Item>();

            foreach (var filePath in filePaths)
            {
                var json          = File.ReadAllText(filePath);
                var itemsFromFile = JsonConvert.DeserializeObject<List<Gw2Item>>(json);
                items.AddRange(itemsFromFile);
            }

            return items;
        }

        private static IEnumerable<GatheringTool> FindGatheringTools(List<Gw2Item> items)
        {
            return items.Where(IsGatheringTool)
                        .Select(g =>
                            new GatheringTool()
                            {
                                Id          = g.Id,
                                Name        = g.Name,
                                IsUnlimited = g.Rarity.Equals("rare", StringComparison.OrdinalIgnoreCase),
                                IconUrl     = g.Icon
                            }
                        )
                        .OrderBy(g => g.IsUnlimited)
                        .ThenBy(g => g.Name);
        }

        private static bool IsGatheringTool(Gw2Item c)
        {
            var type = c.Details.Type;

            return type.Equals("Foraging", StringComparison.OrdinalIgnoreCase)
                   || type.Equals("Logging", StringComparison.OrdinalIgnoreCase)
                   || type.Equals("Mining", StringComparison.OrdinalIgnoreCase);
        }

        private static void WriteToJsonOutputFile(IEnumerable<GatheringTool> gatheringTools, string filePath)
        {
            var json = JsonConvert.SerializeObject(gatheringTools, Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter());
            File.WriteAllText(filePath, json);
        }
    }
}