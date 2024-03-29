﻿using Newtonsoft.Json;

namespace GatheringTools.ToolSearch.Model
{
    public class GatheringTool
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsUnlimited { get; set; }
        public int IconAssetId { get; set; } // e.g. 1998933 from IconUrl https://render.guildwars2.com/file/A329CF6D582CB8D4A3B5250B9CC2F67335F77AB0/1998933.png

        [JsonIgnore] public ToolType ToolType { get; set; } = ToolType.Normal;
    }
}