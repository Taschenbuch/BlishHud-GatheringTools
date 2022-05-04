using System.Collections.Generic;
using System.Linq;
using GatheringTools.ToolSearch.Model;

namespace GatheringTools.ToolSearch.Services
{
    public class FilterGatheringToolsService
    {
        public static void FilterTools(Account account, bool showOnlyUnlimitedTools, bool showBank, bool showSharedInventory)
        {
            if (showOnlyUnlimitedTools)
                FilterUnlimitedTools(account);

            if (showBank == false)
                account.BankGatheringTools.Clear();

            if (showSharedInventory == false)
                account.SharedInventoryGatheringTools.Clear();
        }

        private static void FilterUnlimitedTools(Account account)
        {
            FilterTools(account.BankGatheringTools);
            FilterTools(account.SharedInventoryGatheringTools);

            foreach (var character in account.Characters)
            {
                FilterTools(character.EquippedGatheringTools);
                FilterTools(character.InventoryGatheringTools);
            }
        }

        private static void FilterTools(List<GatheringTool> gatheringTools)
        {
            var filteredTools = gatheringTools.Where(g => g.IsUnlimited).ToList();
            gatheringTools.Clear();
            gatheringTools.AddRange(filteredTools);
        }
    }
}