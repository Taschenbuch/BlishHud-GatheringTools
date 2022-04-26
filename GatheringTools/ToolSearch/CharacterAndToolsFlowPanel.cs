using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;

namespace GatheringTools.ToolSearch
{
    public class CharacterAndToolsFlowPanel : FlowPanel
    {
        public CharacterAndToolsFlowPanel(CharacterAndTools characterAndTools, bool onlyUnlimitedToolsAreVisible, Logger logger)
        {
            var characterNameLabel = new Label
            {
                Text           = characterAndTools.CharacterName,
                Font           = GameService.Content.DefaultFont18,
                ShowShadow     = true,
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
                Parent         = this,
            };

            var gatheringTools = onlyUnlimitedToolsAreVisible
                ? characterAndTools.UnlimitedGatheringTools
                : characterAndTools.GatheringTools;

            var toolsFlowPanel = new FlowPanel()
            {
                FlowDirection    = ControlFlowDirection.SingleLeftToRight,
                WidthSizingMode  = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent           = this,
            };

            foreach (var gatheringTool in gatheringTools)
            {
                try
                {
                    var gatheringToolImage = new Image(GameService.Content.GetRenderServiceTexture(gatheringTool.IconUrl))
                    {
                        BasicTooltipText = gatheringTool.Name,
                        Size             = new Point(50, 50),
                        Parent           = toolsFlowPanel,
                    };
                }
                catch (Exception e)
                {
                    var gatheringToolLabel = new Label
                    {
                        Text           = gatheringTool.Name,
                        Font           = GameService.Content.DefaultFont18,
                        ShowShadow     = true,
                        AutoSizeHeight = true,
                        AutoSizeWidth  = true,
                        Parent         = toolsFlowPanel,
                    };

                    logger.Error(e, "Could not get gathering tool icon from API. Show gathering tool name instead.");
                }
            }
        }
    }
}