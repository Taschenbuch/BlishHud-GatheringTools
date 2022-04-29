using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Modules.Managers;
using GatheringTools.ToolSearch.Model;
using Gw2Sharp.WebApi.V2.Models;
using Microsoft.IdentityModel.Tokens;

namespace GatheringTools.ToolSearch.Services
{
    public class GatheringToolsService
    {
        public static async Task<AccountTools> GetToolsOnAccount(List<GatheringTool> allGatheringTools, Gw2ApiManager gw2ApiManager)
        {
            var sharedInventoryTask = gw2ApiManager.Gw2ApiClient.V2.Account.Inventory.GetAsync();
            var bankTask            = gw2ApiManager.Gw2ApiClient.V2.Account.Bank.GetAsync();
            var charactersTask      = gw2ApiManager.Gw2ApiClient.V2.Characters.AllAsync();

            await Task.WhenAll(sharedInventoryTask, bankTask, charactersTask);

            var bankGatheringTools            = FindGatheringTools(bankTask.Result, allGatheringTools).ToList();
            var sharedInventoryGatheringTools = FindGatheringTools(sharedInventoryTask.Result, allGatheringTools).ToList();

            var accountTools = new AccountTools();
            accountTools.BankGatheringTools.AddRange(bankGatheringTools);
            accountTools.SharedInventoryGatheringTools.AddRange(sharedInventoryGatheringTools);

            foreach (var characterResponse in charactersTask.Result)
            {
                var inventoryGatheringTools = FindInventoryGatheringTools(characterResponse, allGatheringTools);
                var equippedGatheringTools  = FindEquippedGatheringTools(allGatheringTools, characterResponse).ToList();

                var character = new CharacterTools();
                character.CharacterName = characterResponse.Name;
                character.InventoryGatheringTools.AddRange(inventoryGatheringTools);
                character.EquippedGatheringTools.AddRange(equippedGatheringTools);

                accountTools.Characters.Add(character);
            }

            await UnknownGatheringToolsService.UpdateUnknownEquippedGatheringTools(accountTools.Characters, gw2ApiManager);

            return accountTools;
        }

        private static List<GatheringTool> FindInventoryGatheringTools(Character characterResponse, List<GatheringTool> allGatheringTools)
        {
            var inventoryItems = characterResponse.Bags
                                                  .Where(b => b != null) // empty bag slot = null
                                                  .Select(b => b.Inventory)
                                                  .SelectMany(i => i);

            return FindGatheringTools(inventoryItems, allGatheringTools).ToList();
        }

        private static IEnumerable<GatheringTool> FindGatheringTools(IEnumerable<AccountItem> accountItems, List<GatheringTool> allGatheringTools)
        {
            var itemIds = accountItems.Where(i => i != null) // empty item slot = null
                                      .Select(i => i.Id)
                                      .ToList();

            foreach (var itemId in itemIds)
            {
                var matchingGatheringTool = allGatheringTools.SingleOrDefault(g => g.Id == itemId);

                if (matchingGatheringTool != null)
                    yield return matchingGatheringTool;
            }
        }

        private static IEnumerable<GatheringTool> FindEquippedGatheringTools(List<GatheringTool> allGatheringTools, Character characterResponse)
        {
            var equippedGatheringToolIds = GetEquippedGatheringToolIds(characterResponse.Equipment).ToList();

            foreach (var gatheringToolId in equippedGatheringToolIds)
            {
                var matchingGatheringTool = allGatheringTools.SingleOrDefault(a => a.Id == gatheringToolId);
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
    }
}