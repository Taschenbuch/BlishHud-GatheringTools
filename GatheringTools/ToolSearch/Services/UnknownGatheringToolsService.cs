using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
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
                Id          = gatheringToolId,
                IdIsUnknown = true,
                Name        = $"unknown itemId: {gatheringToolId}",
                IsUnlimited = true, // to always show it in tool search, when it can not be identified correctly
                IconUrl = @"https://render.guildwars2.com/file/CC2E01E0F566A6EEF4F2EC2B19AA7A3E1FEFB1B4/60984.png"
            };
        }

        public static async Task UpdateUnknownEquippedGatheringTools(List<CharacterTools> characters, 
                                                                     Gw2ApiManager gw2ApiManager,
                                                                     Logger logger)
        {
            var unknownGatheringToolIds = characters.SelectMany(c => c.EquippedGatheringTools)
                                                    .Where(g => g.IdIsUnknown)
                                                    .Select(g => g.Id)
                                                    .Distinct()
                                                    .ToList();

            if (unknownGatheringToolIds.Any() == false)
                return;

            IReadOnlyList<Item> gatheringToolItems;

            try
            {
                gatheringToolItems = await gw2ApiManager.Gw2ApiClient.V2.Items.ManyAsync(unknownGatheringToolIds);
            }
            catch (Exception e)
            {
                var characterNamesWithUnknownTools = characters.Where(c => c.EquippedGatheringTools.Any(g => unknownGatheringToolIds.Contains(g.Id)))
                                                               .Select(c => c.CharacterName)
                                                               .ToList();

                logger.Error(e, $"V2.Items.ManyAsync() for unknown gathering tool ids failed. " +
                                $"This can be the case for historical items like Master Pick/Axe/Sickle and Black Lion Pick/Axe/Sickle. " +
                                $"unknown ids: {String.Join(", ", unknownGatheringToolIds)}. " +
                                $"Characters equipped with unknown tools: {String.Join(", ", characterNamesWithUnknownTools)}.");
                return;
            }

            var unknownGatheringTools = characters.SelectMany(c => c.EquippedGatheringTools)
                                                  .Where(g => g.IdIsUnknown);

            foreach (var unknownGatheringTool in unknownGatheringTools)
            {
                var matchingGatheringToolItem = gatheringToolItems.Single(i => i.Id == unknownGatheringTool.Id);
                unknownGatheringTool.Name        = matchingGatheringToolItem.Name;
                unknownGatheringTool.IconUrl     = matchingGatheringToolItem.Icon.Url.ToString();
                unknownGatheringTool.IsUnlimited = matchingGatheringToolItem.Rarity == ItemRarity.Rare;
            }
        }
    }
}