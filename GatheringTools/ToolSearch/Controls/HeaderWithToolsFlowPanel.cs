using System;
using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Controls;
using GatheringTools.ToolSearch.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GatheringTools.ToolSearch.Controls
{
    public class HeaderWithToolsFlowPanel : FlowPanel
    {
        public HeaderWithToolsFlowPanel(string headerText, Texture2D headerTexture, List<GatheringTool> gatheringTools, Logger logger)
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
                ShowGatheringToolImageOrFallbackLabel(logger, gatheringTool, toolsFlowPanel);
        }

        private static void ShowGatheringToolImageOrFallbackLabel(Logger logger, GatheringTool gatheringTool, FlowPanel toolsFlowPanel)
        {
            try
            {
                var gatheringToolTexture = GameService.Content.GetRenderServiceTexture(gatheringTool.IconUrl);
                new Image(gatheringToolTexture)
                {
                    BasicTooltipText = gatheringTool.Name,
                    Size             = new Point(ICON_WIDTH_HEIGHT, ICON_WIDTH_HEIGHT),
                    Parent           = toolsFlowPanel,
                };
            }
            catch (Exception e)
            {
                var gatheringToolLabel = new Label
                {
                    Text             = gatheringTool.Name,
                    BasicTooltipText = gatheringTool.Name,
                    Font             = GameService.Content.DefaultFont18,
                    ShowShadow       = true,
                    AutoSizeHeight   = true,
                    AutoSizeWidth    = true,
                    Parent           = toolsFlowPanel,
                };

                logger.Error(e, "Could not get gathering tool icon from API. Show gathering tool name instead.");
            }
        }

        private const int ICON_WIDTH_HEIGHT = 50;
        private const int WIDTH_OF_3_GATHERING_TOOLS = 3 * ICON_WIDTH_HEIGHT + PADDING_TO_PREVENT_LINE_BREAK;
        private const int PADDING_TO_PREVENT_LINE_BREAK = 5;
    }
}