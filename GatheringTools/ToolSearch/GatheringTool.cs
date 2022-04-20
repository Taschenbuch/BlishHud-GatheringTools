using Gw2Sharp.WebApi.V2.Models;

namespace GatheringTools.ToolSearch
{
    public class GatheringTool
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsUnlimited { get; set; }
        public ItemEquipmentSlotType Type { get; set; } = ItemEquipmentSlotType.Unknown;
        public string IconUrl { get; set; }
    }
}