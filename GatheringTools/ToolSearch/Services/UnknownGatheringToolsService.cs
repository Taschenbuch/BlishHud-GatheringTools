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
            };
        }

        public static async Task UpdateUnknownEquippedGatheringTools(List<CharacterTools> characters,
                                                                     Gw2ApiManager gw2ApiManager,
                                                                     Logger logger)
        {
            var unknownGatheringTools = GetUnknownGatheringTools(characters);

            if (unknownGatheringTools.Any())
            {
                var matchingGatheringToolItems = await GetGatheringToolItemsFromApi(unknownGatheringTools, characters, gw2ApiManager, logger);
                UpdateUnknownGatheringTools(unknownGatheringTools, matchingGatheringToolItems);
            }
        }

        private static List<GatheringTool> GetUnknownGatheringTools(List<CharacterTools> characters)
        {
            return characters.SelectMany(c => c.EquippedGatheringTools)
                             .Where(g => g.IdIsUnknown)
                             .ToList();
        }

        private static async Task<IReadOnlyList<Item>> GetGatheringToolItemsFromApi(List<GatheringTool> unknownGatheringTools,
                                                                                      List<CharacterTools> characters,
                                                                                      Gw2ApiManager gw2ApiManager,
                                                                                      Logger logger)
        {
            var unknownGatheringToolIds = unknownGatheringTools.Select(g => g.Id)
                                                               .Distinct()
                                                               .ToList();

            try
            {
                return await gw2ApiManager.Gw2ApiClient.V2.Items.ManyAsync(unknownGatheringToolIds);
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

                return new List<Item>().AsReadOnly();
            }
        }

        private static void UpdateUnknownGatheringTools(List<GatheringTool> unknownGatheringTools, IReadOnlyList<Item> matchingGatheringToolItems)
        {
            foreach (var unknownGatheringTool in unknownGatheringTools)
            {
                var matchingGatheringToolItem = matchingGatheringToolItems.Single(i => i.Id == unknownGatheringTool.Id);
                unknownGatheringTool.Name        = matchingGatheringToolItem.Name;
                unknownGatheringTool.IconUrl     = matchingGatheringToolItem.Icon.Url.ToString();
                unknownGatheringTool.IsUnlimited = matchingGatheringToolItem.Rarity == ItemRarity.Rare;
                unknownGatheringTool.IdIsUnknown = false;
            }
        }
    }
}