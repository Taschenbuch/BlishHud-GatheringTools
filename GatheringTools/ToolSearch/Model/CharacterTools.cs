using System.Collections.Generic;
using System.Linq;

namespace GatheringTools.ToolSearch.Model
{
    public class CharacterTools // name because Gw2Api has "Character" type
    {
        public string CharacterName { get; set; } = string.Empty;
        public List<GatheringTool> EquippedGatheringTools { get; } = new List<GatheringTool>();
        public List<GatheringTool> InventoryGatheringTools { get; } = new List<GatheringTool>();
        public List<GatheringTool> UnlimitedEquippedGatheringTools => EquippedGatheringTools.Where(g => g.IsUnlimited).ToList();
        public bool HasTools() => EquippedGatheringTools.Any() || InventoryGatheringTools.Any();
    }
}