using System.Collections.Generic;
using System.Linq;
using GatheringTools.ToolSearch.Model;

namespace GatheringTools.ToolSearch.Services
{
    public class FilterGatheringToolsService
    {
        public static void FilterTools(AccountTools accountTools, bool showOnlyUnlimitedTools, bool showBank, bool showSharedInventory)
        {
            if (showOnlyUnlimitedTools)
                FilterUnlimitedTools(accountTools);

            if(showBank == false)
                accountTools.BankGatheringTools.Clear();

            if(showSharedInventory == false)
                accountTools.SharedInventoryGatheringTools.Clear();
        }

        private static void FilterUnlimitedTools(AccountTools accountTools)
        {
            FilterTools(accountTools.BankGatheringTools);
            FilterTools(accountTools.SharedInventoryGatheringTools);

            foreach (var character in accountTools.Characters)
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