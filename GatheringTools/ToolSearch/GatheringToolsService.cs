using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2.Models;

namespace GatheringTools.ToolSearch
{
    public class GatheringToolsService
    {
        public static async Task<List<CharacterAndTools>> GetCharactersAndTools(List<GatheringTool> allGatheringTools, Gw2ApiManager gw2ApiManager)
        {
            var charactersResponse = await gw2ApiManager.Gw2ApiClient.V2.Characters.AllAsync();
            var charactersAndTools = new List<CharacterAndTools>();

            foreach (var characterResponse in charactersResponse)
            {
                var gatheringToolIds = GetEquippedGatheringToolIds(characterResponse.Equipment).ToList();
                var gatheringTools   = new List<GatheringTool>();

                foreach (var gatheringToolId in gatheringToolIds)
                {
                    var matchingGatheringTool = allGatheringTools.SingleOrDefault(a => a.Id == gatheringToolId);
                    var gatheringTool         = matchingGatheringTool ?? CreateUnknownGatheringTool(gatheringToolId);
                    gatheringTools.Add(gatheringTool);
                }

                var characterAndTools = new CharacterAndTools();
                characterAndTools.CharacterName = characterResponse.Name;
                characterAndTools.GatheringTools.AddRange(gatheringTools);

                charactersAndTools.Add(characterAndTools);
            }

            await UpdateUnknownGatheringTools(charactersAndTools, gw2ApiManager);

            return charactersAndTools;
        }

        private static GatheringTool CreateUnknownGatheringTool(int gatheringToolId)
        {
            return new GatheringTool
            {
                Id = gatheringToolId, 
                Name = UNKNOWN_GATHERING_TOOL_NAME
            };
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

        private static async Task UpdateUnknownGatheringTools(List<CharacterAndTools> characterAndTools, Gw2ApiManager gw2ApiManager)
        {
            var unknownGatheringToolIds = characterAndTools.SelectMany(c => c.GatheringTools)
                                                           .Where(g => g.Name == UNKNOWN_GATHERING_TOOL_NAME)
                                                           .Select(g => g.Id)
                                                           .Distinct()
                                                           .ToList();

            if (unknownGatheringToolIds.Any() == false)
                return;

            var gatheringToolItems = await gw2ApiManager.Gw2ApiClient.V2.Items.ManyAsync(unknownGatheringToolIds);

            foreach (var unknownGatheringTool in characterAndTools.SelectMany(c => c.GatheringTools).Where(g => g.Name == UNKNOWN_GATHERING_TOOL_NAME))
            {
                var matchingGatheringToolItem = gatheringToolItems.Single(i => i.Id == unknownGatheringTool.Id);
                unknownGatheringTool.Name        = matchingGatheringToolItem.Name;
                unknownGatheringTool.IconUrl     = matchingGatheringToolItem.Icon.Url.ToString();
                unknownGatheringTool.IsUnlimited = matchingGatheringToolItem.Rarity == ItemRarity.Rare;
            }
        }

        private const string UNKNOWN_GATHERING_TOOL_NAME = "???";
    }
}