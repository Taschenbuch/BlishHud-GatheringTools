using System;
using System.Collections.Generic;
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
        public HeaderWithToolsFlowPanel(string headerText, List<GatheringTool> gatheringTools, Texture2D headerTexture, Texture2D unknownToolTexture, Logger logger)
        {
            FlowDirection = ControlFlowDirection.SingleLeftToRight;

            var headerImage = new Image(headerTexture)
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

            new Image(gatheringToolTexture)
            {
                BasicTooltipText = gatheringTool.Name,
                Size             = new Point(ICON_WIDTH_HEIGHT, ICON_WIDTH_HEIGHT),
                Parent           = toolsFlowPanel,
            };

        }

        private static AsyncTexture2D DetermineToolTexture(GatheringTool gatheringTool, Texture2D unknownToolTexture, Logger logger)
        {
            if (gatheringTool.IdIsUnknown)
                return unknownToolTexture;

            try
            {
                return GameService.Content.GetRenderServiceTexture(gatheringTool.IconUrl);
            }
            catch (Exception e)
            {
                logger.Error(e, "Could not get gathering tool icon from API. Show gathering tool name instead.");
                return unknownToolTexture;
            }
        }

        private const int ICON_WIDTH_HEIGHT = 50;
        private const int WIDTH_OF_3_GATHERING_TOOLS = 3 * ICON_WIDTH_HEIGHT + PADDING_TO_PREVENT_LINE_BREAK;
        private const int PADDING_TO_PREVENT_LINE_BREAK = 5;
    }
}