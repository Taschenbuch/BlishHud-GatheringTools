using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Modules.Managers;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;

namespace GatheringTools.ToolSearch
{
    public class GatheringToolsService
    {
        public static async Task<List<CharacterAndTools>> GetCharactersAndTools(Gw2ApiManager gw2ApiManager)
        {
            var charactersResponse = await gw2ApiManager.Gw2ApiClient.V2.Characters.AllAsync();
            var charactersAndTools = new List<CharacterAndTools>();

            foreach (var characterResponse in charactersResponse)
            {
                var gatheringTools = GetGatheringToolsWithIdAndType(characterResponse.Equipment).ToList();

                var characterAndTools = new CharacterAndTools();
                characterAndTools.CharacterName = characterResponse.Name;
                characterAndTools.GatheringTools.AddRange(gatheringTools);

                charactersAndTools.Add(characterAndTools);
            }

            await UpdateGatheringToolsNameEtc(charactersAndTools);

            return charactersAndTools;
        }

        private static IEnumerable<GatheringTool> GetGatheringToolsWithIdAndType(IReadOnlyList<CharacterEquipmentItem> equipmentItems)
        {
            foreach (var equipmentItem in equipmentItems ?? new List<CharacterEquipmentItem>())
            {
                switch (equipmentItem.Slot.Value)
                {
                    case ItemEquipmentSlotType.Sickle:
                    case ItemEquipmentSlotType.Axe:
                    case ItemEquipmentSlotType.Pick:
                        yield return new GatheringTool
                        {
                            Id   = equipmentItem.Id,
                            Type = equipmentItem.Slot.Value
                        };
                        break;
                }
            }
        }

        private static async Task UpdateGatheringToolsNameEtc(List<CharacterAndTools> characterAndTools)
        {
            var gatheringToolIds = characterAndTools.SelectMany(c => c.GatheringTools)
                                                    .Select(g => g.Id)
                                                    .Distinct();

            using (var gw2Client = new Gw2Client(new Connection(Locale.English)))
            {
                var gatheringToolItems = await gw2Client.WebApi.V2.Items.ManyAsync(gatheringToolIds);

                foreach (var gatheringTool in characterAndTools.SelectMany(c => c.GatheringTools))
                {
                    var matchingGatheringToolItem = gatheringToolItems.First(i => i.Id == gatheringTool.Id);
                    gatheringTool.Name        = matchingGatheringToolItem.Name;
                    gatheringTool.IconUrl     = matchingGatheringToolItem.Icon.Url.ToString();
                    gatheringTool.IsUnlimited = matchingGatheringToolItem.Description?.ToLowerInvariant().Contains("unlimited use") ?? false;
                }
            }
        }
    }
}