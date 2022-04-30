using System.Collections.Generic;
using System.Linq;

namespace GatheringTools.ToolSearch.Model
{
    public class AccountTools // name because gw2sharp has "Account" type
    {
        public AccountTools() // for api error handling cases
        {
        }

        public AccountTools(List<GatheringTool> bankGatheringTools, List<GatheringTool> sharedInventoryGatheringTools)
        {

            BankGatheringTools            = bankGatheringTools;
            SharedInventoryGatheringTools = sharedInventoryGatheringTools;
        }

        public List<GatheringTool> SharedInventoryGatheringTools { get; } = new List<GatheringTool>();
        public List<GatheringTool> BankGatheringTools { get; } = new List<GatheringTool>();
        public List<CharacterTools> Characters { get; } = new List<CharacterTools>();

        public bool HasTools()
        {
            return SharedInventoryGatheringTools.Any()
                   || BankGatheringTools.Any()
                   || Characters.Any(c => c.HasTools());
        }
    }
}