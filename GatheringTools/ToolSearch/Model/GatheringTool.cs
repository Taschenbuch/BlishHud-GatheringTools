using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;

namespace GatheringTools.ToolSearch.Model
{
    public class GatheringTool
    {
        public int Id { get; set; }
        [JsonIgnore]
        public bool IdIsUnknown { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public bool IsUnlimited { get; set; }
        public string IconUrl { get; set; }
    }
}