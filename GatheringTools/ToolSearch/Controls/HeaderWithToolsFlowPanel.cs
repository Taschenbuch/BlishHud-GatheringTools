using System;
using System.Collections.Generic;
using System.IO;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using GatheringTools.ToolSearch.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GatheringTools.ToolSearch.Controls
{
    public class HeaderWithToolsFlowPanel : FlowPanel
    {
        public HeaderWithToolsFlowPanel(string headerText,
                                        List<GatheringTool> gatheringTools,
                                        Texture2D headerTexture,
                                        Texture2D unknownToolTexture,
                                        Logger logger)
        {
            FlowDirection = ControlFlowDirection.SingleLeftToRight;

            new Image(headerTexture)
            {
                BasicTooltipText = headerText,
                Size             = new Point(30, 30),
                Parent           = this,
            };

            var toolsFlowPanel = new FlowPanel()
            {
                FlowDirection    = ControlFlowDirection.LeftToRight,
                Size             = new Point(WIDTH_OF_3_GATHERING_TOOLS, 0),
                HeightSizingMode = SizingMode.AutoSize,
                Parent           = this,
            };

            foreach (var gatheringTool in gatheringTools)
                ShowGatheringToolImageOrFallbackControl(gatheringTool, unknownToolTexture, toolsFlowPanel, logger);
        }

        private static void ShowGatheringToolImageOrFallbackControl(GatheringTool gatheringTool,
                                                                    Texture2D unknownToolTexture,
                                                                    FlowPanel toolsFlowPanel,
                                                                    Logger logger)
        {
            var gatheringToolTexture = DetermineToolTexture(gatheringTool, unknownToolTexture, logger);
            var tooltipText          = DetermineTooltipText(gatheringTool);

            new Image(gatheringToolTexture)
            {
                BasicTooltipText = tooltipText,
                Size             = new Point(ICON_WIDTH_HEIGHT, ICON_WIDTH_HEIGHT),
                Parent           = toolsFlowPanel,
            };
        }

        private static string DetermineTooltipText(GatheringTool gatheringTool)
        {
            switch (gatheringTool.ToolType)
            {
                case ToolType.Normal:
                    return gatheringTool.Name;
                case ToolType.UnknownId:
                    return $"No data in the API for this item ID: {gatheringTool.Id}. :(\n" +
                           $"Could be a very new or very old item. Or a bug in the API.";
                case ToolType.InventoryCanNotBeAccessedPlaceHolder:
                    return "Can not access inventory for this character. :(\n" +
                           "Could be an API error or API key has no 'inventories' permission.";
                default:
                    return $"Bug in module code. ToolType {gatheringTool.ToolType} is not handled. :(";
            }
        }

        private static AsyncTexture2D DetermineToolTexture(GatheringTool gatheringTool, Texture2D unknownToolTexture, Logger logger)
        {
            if (gatheringTool.ToolType != ToolType.Normal)
                return unknownToolTexture;

            try
            {
                return GameService.Content.DatAssetCache.TryGetTextureFromAssetId(gatheringTool.IconAssetId, out AsyncTexture2D texture)
                    ? texture
                    : throw new Exception($"DatAssetCache is missing texture for '{gatheringTool.Name}', itemId: {gatheringTool.Id}, assetId: {gatheringTool.IconAssetId} ");
            }
            catch (Exception e)
            {
                logger.Warn(e, "Could not get gathering tool icon from API. Show placeholder icon instead.");
                return unknownToolTexture;
            }
        }

        private const int ICON_WIDTH_HEIGHT = 50;
        private const int WIDTH_OF_3_GATHERING_TOOLS = 3 * ICON_WIDTH_HEIGHT + PADDING_TO_PREVENT_LINE_BREAK;
        private const int PADDING_TO_PREVENT_LINE_BREAK = 5;
    }
}