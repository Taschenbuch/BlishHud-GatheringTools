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
    public static class FindGatheringToolsService
    {
        public static async Task<(AccountTools, bool apiAccessFailed)> GetToolsFromApi(List<GatheringTool> allGatheringTools,
                                                                                       Gw2ApiManager gw2ApiManager,
                                                                                       Logger logger)
        {
            if (gw2ApiManager.HasPermissions(NECESSARY_API_TOKEN_PERMISSIONS) == false)
            {
                var missingPermissionsErrorMessage = "HasPermissions() returned false. Possible reasons: " +
                                                     "API subToken does not have the necessary permissions: " +
                                                     $"{String.Join(", ", NECESSARY_API_TOKEN_PERMISSIONS)}. " +
                                                     $"Or module did not get API subToken from Blish yet. Or API key is missing.";

                logger.Warn(missingPermissionsErrorMessage);
                return (new AccountTools(), true);
            }

            try
            {
                // Task.run because not just await but also a lot of cpu-bound look ups
                return (await Task.Run(() => GetToolsOnAccount(allGatheringTools, gw2ApiManager, logger)), false);
            }
            catch (Exception e)
            {
                logger.Error(e, "Could not get gathering tools from API");
                return (new AccountTools(), true);
            }
        }

        private static async Task<AccountTools> GetToolsOnAccount(List<GatheringTool> allGatheringTools,
                                                                  Gw2ApiManager gw2ApiManager,
                                                                  Logger logger)
        {
            var sharedInventoryTask = gw2ApiManager.Gw2ApiClient.V2.Account.Inventory.GetAsync();
            var bankTask            = gw2ApiManager.Gw2ApiClient.V2.Account.Bank.GetAsync();
            var charactersTask      = gw2ApiManager.Gw2ApiClient.V2.Characters.AllAsync();
            await Task.WhenAll(sharedInventoryTask, bankTask, charactersTask);

            var bankGatheringTools            = FindGatheringTools(bankTask.Result, allGatheringTools).ToList();
            var sharedInventoryGatheringTools = FindGatheringTools(sharedInventoryTask.Result, allGatheringTools).ToList();
            var accountTools                  = new AccountTools(bankGatheringTools, sharedInventoryGatheringTools);

            foreach (var characterResponse in charactersTask.Result)
            {
                var inventoryGatheringTools = FindInventoryGatheringTools(characterResponse, allGatheringTools, logger);
                var equippedGatheringTools  = FindEquippedGatheringTools(allGatheringTools, characterResponse).ToList();
                var character               = new CharacterTools(characterResponse.Name, inventoryGatheringTools, equippedGatheringTools);
                accountTools.Characters.Add(character);
            }

            await UnknownGatheringToolsService.UpdateUnknownEquippedGatheringTools(accountTools.Characters, gw2ApiManager, logger);

            return accountTools;
        }

        private static List<GatheringTool> FindInventoryGatheringTools(Character characterResponse,
                                                                       List<GatheringTool> allGatheringTools,
                                                                       Logger logger)
        {
            if (characterResponse.Bags == null) // .Bags == null happened to a user, though the permission check should have prevented it.
            {
                logger.Error($"Character.Bags is NULL for a character (Name: {characterResponse.Name}). " +
                             $"This can happen when api key is missing inventory permission.");

                return new List<GatheringTool>();
            }

            var inventoryItems = characterResponse.Bags
                                                  .Where(IsNotEmptyBagSlot)
                                                  .Select(b => b.Inventory)
                                                  .SelectMany(i => i);

            return FindGatheringTools(inventoryItems, allGatheringTools).ToList();
        }

        private static bool IsNotEmptyBagSlot(CharacterInventoryBag bag) => bag != null;

        private static IEnumerable<GatheringTool> FindGatheringTools(IEnumerable<AccountItem> accountItems, List<GatheringTool> allGatheringTools)
        {
            var itemIds = accountItems.Where(IsNotEmptyItemSlot)
                                      .Select(i => i.Id)
                                      .ToList();

            foreach (var itemId in itemIds)
            {
                var matchingGatheringTool = allGatheringTools.FindToolById(itemId);

                if (matchingGatheringTool != null)
                    yield return matchingGatheringTool;
            }
        }

        private static bool IsNotEmptyItemSlot(AccountItem itemSlot) => itemSlot != null;

        private static IEnumerable<GatheringTool> FindEquippedGatheringTools(List<GatheringTool> allGatheringTools, Character characterResponse)
        {
            var equippedGatheringToolIds = GetEquippedGatheringToolIds(characterResponse.Equipment).ToList();

            foreach (var gatheringToolId in equippedGatheringToolIds)
            {
                var matchingGatheringTool = allGatheringTools.FindToolById(gatheringToolId);
                var gatheringTool         = matchingGatheringTool ?? UnknownGatheringToolsService.CreateUnknownGatheringTool(gatheringToolId);
                yield return gatheringTool;
            }
        }

        private static IEnumerable<int> GetEquippedGatheringToolIds(IReadOnlyList<CharacterEquipmentItem> equipmentItems)
        {
            foreach (var equipmentItem in equipmentItems ?? new List<CharacterEquipmentItem>())
                switch (equipmentItem.Slot.Value)
                {
                    case ItemEquipmentSlotType.Sickle:
                    case ItemEquipmentSlotType.Axe:
                    case ItemEquipmentSlotType.Pick:
                        yield return equipmentItem.Id;
                        break;
                }
        }

        private static GatheringTool FindToolById(this List<GatheringTool> allGatheringTools, int itemId)
        {
            return allGatheringTools.SingleOrDefault(a => a.Id == itemId);
        }

        private static readonly List<TokenPermission> NECESSARY_API_TOKEN_PERMISSIONS = new List<TokenPermission>
        {
            TokenPermission.Account,
            TokenPermission.Characters,
            TokenPermission.Inventories,
            TokenPermission.Builds
        };
    }
}