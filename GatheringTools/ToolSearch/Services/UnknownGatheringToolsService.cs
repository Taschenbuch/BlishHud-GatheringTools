using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Modules.Managers;
using GatheringTools.ToolSearch.Model;
using Gw2Sharp.WebApi.V2.Models;

namespace GatheringTools.ToolSearch.Services
{
    public class UnknownGatheringToolsService
    {
        public static GatheringTool CreateUnknownGatheringTool(int gatheringToolId)
        {
            return new GatheringTool
            {
                Id   = gatheringToolId,
                Name = UNKNOWN_GATHERING_TOOL_NAME
            };
        }

        public static async Task UpdateUnknownEquippedGatheringTools(List<CharacterTools> characters, Gw2ApiManager gw2ApiManager)
        {
            var unknownGatheringToolIds = characters.SelectMany(c => c.EquippedGatheringTools)
                                                    .Where(g => g.Name == UNKNOWN_GATHERING_TOOL_NAME)
                                                    .Select(g => g.Id)
                                                    .Distinct()
                                                    .ToList();

            if (unknownGatheringToolIds.Any() == false)
                return;

            var gatheringToolItems = await gw2ApiManager.Gw2ApiClient.V2.Items.ManyAsync(unknownGatheringToolIds);

            foreach (var unknownGatheringTool in characters.SelectMany(c => c.EquippedGatheringTools).Where(g => g.Name == UNKNOWN_GATHERING_TOOL_NAME))
            {
                var matchingGatheringToolItem = gatheringToolItems.Single(i => i.Id == unknownGatheringTool.Id);
                unknownGatheringTool.Name        = matchingGatheringToolItem.Name;
                unknownGatheringTool.IconUrl     = matchingGatheringToolItem.Icon.Url.ToString();
                unknownGatheringTool.IsUnlimited = matchingGatheringToolItem.Rarity == ItemRarity.Rare;
            }
        }

        private const string UNKNOWN_GATHERING_TOOL_NAME = "???";
    }
}