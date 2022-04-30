using System.Collections.Generic;
using System.Linq;

namespace GatheringTools.ToolSearch.Model
{
    public class CharacterTools // name because gw2sharp has "Character" type
    {
        public CharacterTools(string characterName, List<GatheringTool> inventoryGatheringTools, List<GatheringTool> equippedGatheringTools)
        {
            CharacterName           = characterName;
            InventoryGatheringTools = inventoryGatheringTools;
            EquippedGatheringTools  = equippedGatheringTools;
        }

        public string CharacterName { get; }
        public List<GatheringTool> EquippedGatheringTools { get; }
        public List<GatheringTool> InventoryGatheringTools { get; }
        public bool HasTools() => EquippedGatheringTools.Any() || InventoryGatheringTools.Any();
    }
}