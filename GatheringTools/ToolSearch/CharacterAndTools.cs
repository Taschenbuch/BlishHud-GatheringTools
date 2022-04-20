using System.Collections.Generic;
using System.Linq;

namespace GatheringTools.ToolSearch
{
    public class CharacterAndTools
    {
        public string CharacterName { get; set; } = string.Empty;
        public List<GatheringTool> GatheringTools { get; set; } = new List<GatheringTool>();
        public List<GatheringTool> UnlimitedGatheringTools => GatheringTools.Where(g => g.IsUnlimited)
                                                                            .ToList();

        public bool HasNoTools() => GatheringTools.Any() == false;
        public bool HasUnlimitedTools() => GatheringTools.Any(g => g.IsUnlimited);
    }
}