using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using GatheringTools.ToolSearch.Model;
using Gw2Sharp.WebApi.V2.Models;
using Microsoft.IdentityModel.Tokens;
using Character = GatheringTools.ToolSearch.Model.Character;

namespace GatheringTools.ToolSearch.Services
{
    public class UnknownGatheringToolsService
    {
        public static GatheringTool CreateNoInventoryAccessPlaceholderTool()
        {
            return new GatheringTool
            {
                Id          = -1,
                IsUnlimited = true, // to always show it in tool search, when it can not be identified correctly
                ToolType    = ToolType.InventoryCanNotBeAccessedPlaceHolder,
            };
        }

        public static GatheringTool CreateUnknownGatheringTool(int id)
        {
            return new GatheringTool
            {
                Id          = id,
                IsUnlimited = true, // to always show it in tool search, when it can not be identified correctly
                ToolType    = ToolType.UnknownId,
            };
        }

        public static async Task UpdateUnknownEquippedGatheringTools(List<Character> characters,
                                                                     Gw2ApiManager gw2ApiManager,
                                                                     Logger logger)
        {
            var unknownGatheringTools = GetUnknownGatheringTools(characters);

            if (unknownGatheringTools.IsNullOrEmpty())
                return;

            var matchingGatheringToolItems = await GetGatheringToolItemsFromApi(unknownGatheringTools, characters, gw2ApiManager, logger);

            if (matchingGatheringToolItems.Any())
                UpdateUnknownGatheringTools(unknownGatheringTools, matchingGatheringToolItems);
        }

        private static List<GatheringTool> GetUnknownGatheringTools(List<Character> charactersToolsList)
        {
            return charactersToolsList.SelectMany(c => c.EquippedGatheringTools)
                                      .Where(g => g.ToolType == ToolType.UnknownId)
                                      .ToList();
        }

        private static async Task<IReadOnlyList<Item>> GetGatheringToolItemsFromApi(List<GatheringTool> unknownGatheringTools,
                                                                                    List<Character> characters,
                                                                                    Gw2ApiManager gw2ApiManager,
                                                                                    Logger logger)
        {
            var unknownGatheringToolIds = unknownGatheringTools.Select(g => g.Id)
                                                               .Distinct()
                                                               .ToList();

            try
            {
                // will throw an exception when at least one itemId can not be found in V2.Items. even when the other itemIds could be found. 
                // Thus gw2sharp behavior seems to differ from a request via url in this case. Probably of the way how it asks the api with ManyAsync().
                return await gw2ApiManager.Gw2ApiClient.V2.Items.ManyAsync(unknownGatheringToolIds);
            }
            catch (Exception e)
            {
                var characterNamesWithUnknownTools = characters.Where(c => c.EquippedGatheringTools.Any(g => unknownGatheringToolIds.Contains(g.Id)))
                                                               .Select(c => c.CharacterName)
                                                               .ToList();

                logger.Warn(e, $"V2.Items.ManyAsync() for unknown gathering tool ids failed. " +
                                $"This can be the case for old historical items. " +
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
                unknownGatheringTool.ToolType    = ToolType.Normal;
            }
        }
    }
}