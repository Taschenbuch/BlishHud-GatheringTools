using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatheringTools.ToolSearch.Model;
using Gw2Sharp;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;
using File = System.IO.File;

namespace GatheringTools.ItemJsonFileCreator
{
    class Program
    {
        private const string OUTPUT_JSON_FILE_PATH = @"C:\Dev\blish\gatheringToolsFromV2ItemsApi.json";

        static async Task Main()
        {
            var gw2Connection = new Connection();
            var gw2Client = new Gw2Client(gw2Connection);
            var itemIds = await gw2Client.WebApi.V2.Items.IdsAsync();
            var items = await gw2Client.WebApi.V2.Items.ManyAsync(itemIds);
            var gatheringTools = FindAndCreateGatheringTools(items);
            WriteToJsonOutputFile(gatheringTools, OUTPUT_JSON_FILE_PATH);
        }

        private static IEnumerable<GatheringTool> FindAndCreateGatheringTools(IReadOnlyList<Item> items)
        {
            return items.Where(i => i.Type == ItemType.Gathering) // includes fishing baits ands lures
                        .Cast<ItemGathering>()
                        .Where(IsPickOrAxeOrSickle)
                        .Select(g =>
                            new GatheringTool()
                            {
                                Id          = g.Id,
                                Name        = g.Name,
                                IsUnlimited = g.Rarity == ItemRarity.Rare,
                                IconUrl     = g.Icon.Url.ToString(),
                            }
                        )
                        .OrderBy(g => g.IsUnlimited)
                        .ThenBy(g => g.Name);
        }

        private static bool IsPickOrAxeOrSickle(ItemGathering item)
        {
            var type = item.Details.Type;

            return type == ItemGatheringType.Mining
                || type == ItemGatheringType.Logging
                || type == ItemGatheringType.Foraging;
        }

        private static void WriteToJsonOutputFile(IEnumerable<GatheringTool> gatheringTools, string filePath)
        {
            var json = JsonConvert.SerializeObject(gatheringTools, Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter());
            File.WriteAllText(filePath, json);
        }
    }
}