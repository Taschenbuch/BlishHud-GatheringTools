using System.Collections.Generic;
using System.Linq;

namespace GatheringTools.ToolSearch.Model
{
    public class Account // name because gw2sharp has "Account" type
    {
        public Account() // for api error handling cases
        {
        }

        public Account(List<GatheringTool> bankGatheringTools, List<GatheringTool> sharedInventoryGatheringTools)
        {
            BankGatheringTools            = bankGatheringTools;
            SharedInventoryGatheringTools = sharedInventoryGatheringTools;
        }

        public List<GatheringTool> SharedInventoryGatheringTools { get; } = new List<GatheringTool>();
        public List<GatheringTool> BankGatheringTools { get; } = new List<GatheringTool>();
        public List<Character> Characters { get; } = new List<Character>();

        public bool HasTools()
        {
            return SharedInventoryGatheringTools.Any()
                   || BankGatheringTools.Any()
                   || Characters.Any(c => c.HasTools());
        }
    }
}