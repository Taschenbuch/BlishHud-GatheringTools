using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GatheringTools.ToolSearch.Model;
using GatheringTools.ToolSearch.Services;
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
            using var gw2Client = new Gw2Client(gw2Connection);
            
            Console.WriteLine("getting item ids...");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var itemIds = await gw2Client.WebApi.V2.Items.IdsAsync();
            
            Console.WriteLine($"getting item ids finished in {getFormattedTime(stopWatch)}. Got {itemIds.Count} itemIds!");
            stopWatch.Restart();
            Console.WriteLine("getting items... (this takes about 5 minutes)");
            
            var items = await gw2Client.WebApi.V2.Items.ManyAsync(itemIds);
            
            Console.WriteLine($"getting items finished in {getFormattedTime(stopWatch)} ms");
            stopWatch.Restart();
            Console.WriteLine("searching for gathering tools...");
            
            var gatheringTools = FindAndCreateGatheringTools(items);
            
            Console.WriteLine($"searching for gathering tools finished in {getFormattedTime(stopWatch)} ms. Found {gatheringTools.Count()} gathering tools");
            stopWatch.Restart();
            Console.WriteLine("writing gathering tools to file...");
            
            WriteToJsonOutputFile(gatheringTools, OUTPUT_JSON_FILE_PATH);
            
            Console.WriteLine($"writing gathering tools to file finished in {getFormattedTime(stopWatch)} ms");
            stopWatch.Stop();
        }

        private static string getFormattedTime(Stopwatch stopWatch)
        {
            return stopWatch.Elapsed.ToString("hh':'mm':'ss':'fff' (hh:mm:ss:ms)'");
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
                                IconAssetId = UnknownGatheringToolsService.GetIconAssetId(g.Icon.Url.ToString())
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