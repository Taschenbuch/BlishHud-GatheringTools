using System.Collections.Generic;

namespace GatheringTools.ToolSearch.Services.RemoteFiles
{
    public class FileConstants
    {
        public string RemoteBaseUrl = "https://bhm.blishhud.com/ecksofa.gatheringtools";
        public string ModuleFolderName = "gathering-tools-module-data";
        public string FormatVersion = "1";
        public const string GatheringToolsFromV2ItemsApiFileName = "gatheringToolsFromV2ItemsApi.json";
        public const string GatheringToolsMissingInV2ItemsApiFileName = "gatheringToolsMissingInV2ItemsApi.json";
        public List<string> RemotelyUpdatableDataFileNames { get; } = new List<string>
        {
            GatheringToolsFromV2ItemsApiFileName,
            GatheringToolsMissingInV2ItemsApiFileName
        };
    }
}
