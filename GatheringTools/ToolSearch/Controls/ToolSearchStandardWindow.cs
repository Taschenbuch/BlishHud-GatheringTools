using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using GatheringTools.ToolSearch.Model;
using GatheringTools.ToolSearch.Services;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace GatheringTools.ToolSearch.Controls
{
    public class ToolSearchStandardWindow : StandardWindow
    {
        public ToolSearchStandardWindow(TextureService textureService,
                                        SettingEntry<bool> showOnlyUnlimitedToolsSetting,
                                        List<GatheringTool> allGatheringTools,
                                        Gw2ApiManager gw2ApiManager,
                                        Logger logger)
            : base(textureService.WindowBackgroundTexture, new Rectangle(10, 30, 235, 610), new Rectangle(30, 30, 230, 600))
        {
            _textureService    = textureService;
            _allGatheringTools = allGatheringTools;
            _gw2ApiManager     = gw2ApiManager;
            _logger            = logger;

            _infoLabel = new Label()
            {
                Location       = new Point(0, 30),
                TextColor      = Color.White,
                ShowShadow     = true,
                Size           = new Point(MAX_CONTENT_WIDTH, 0),
                AutoSizeHeight = true,
                ClipsBounds    = false,
                WrapText       = true,
                Parent         = this
            };

            _loadingSpinner = new LoadingSpinner
            {
                Location = new Point(60, 60),
                Parent   = this
            };

            _showOnlyUnlimitedToolsCheckbox = new Checkbox
            {
                Text             = "Only unlimited tools",
                Checked          = showOnlyUnlimitedToolsSetting.Value,
                BasicTooltipText = "Show only unlimited tools",
                Parent           = this,
            };

            _showOnlyUnlimitedToolsCheckbox.CheckedChanged += async (s, e) =>
            {
                showOnlyUnlimitedToolsSetting.Value = e.Checked;
                await ShowWindowAndUpdateToolsInUi();
            };

            _rootFlowPanel = new FlowPanel()
            {
                Location      = new Point(0, 30),
                Size          = new Point(MAX_CONTENT_WIDTH, 500),
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                CanScroll     = true,
                Parent        = this,
            };
        }

        public async Task ToggleVisibility()
        {
            if (Visible)
                Hide();
            else
                await ShowWindowAndUpdateToolsInUi();
        }

        private async Task ShowWindowAndUpdateToolsInUi()
        {
            Show();

            // ReSharper disable once MethodHasAsyncOverload
            if (_semaphoreSlim.Wait(0) == false)
                return;

            try
            {
                await UpdateToolsInUiFromApi();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task UpdateToolsInUiFromApi()
        {
            _rootFlowPanel.ClearChildren();
            _infoLabel.Text = "Getting API data...";
            _loadingSpinner.Show();

            var (accountTools, apiAccessFailed) = await FindGatheringToolsService.GetToolsFromApi(_allGatheringTools, _gw2ApiManager, _logger);


            _infoLabel.Text = string.Empty;
            _loadingSpinner.Hide();

            if (apiAccessFailed)
            {
                _infoLabel.Text = API_KEY_ERROR_MESSAGE;
                return;
            }

            FilterGatheringToolsService.FilterTools(accountTools, _showOnlyUnlimitedToolsCheckbox.Checked);

            if (accountTools.HasTools())
                ShowToolsInUi(accountTools, _rootFlowPanel, _textureService, _logger);
            else
                _infoLabel.Text = "No tools found with current search filter or no character has tools equipped!";
        }

        

        private static void ShowToolsInUi(AccountTools accountTools, FlowPanel rootFlowPanel, TextureService textureService, Logger logger)
        {
            if (accountTools.BankGatheringTools.Any())
            {
                var bankToolsFlowPanel = new HeaderWithToolsFlowPanel(
                    "Bank", textureService.BankTexture, accountTools.BankGatheringTools, logger)
                {
                    ShowBorder = true,
                    Parent     = rootFlowPanel
                };
            }

            if (accountTools.SharedInventoryGatheringTools.Any())
            {
                var sharedInventoryFlowPanel = new HeaderWithToolsFlowPanel(
                    "Shared inventory", textureService.SharedInventoryTexture, accountTools.SharedInventoryGatheringTools, logger)
                {
                    ShowBorder = true,
                    Parent     = rootFlowPanel
                };
            }

            foreach (var character in accountTools.Characters)
            {
                if (character.HasTools() == false)
                    continue;

                var characterFlowPanel = new FlowPanel()
                {
                    FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                    WidthSizingMode  = SizingMode.AutoSize,
                    HeightSizingMode = SizingMode.AutoSize,
                    ShowBorder       = true,
                    Parent           = rootFlowPanel
                };

                var headerLabel = new Label
                {
                    Text             = character.CharacterName,
                    BasicTooltipText = character.CharacterName,
                    Font             = GameService.Content.DefaultFont18,
                    ShowShadow       = true,
                    Size             = new Point(MAX_CONTENT_WIDTH - 30, 0),
                    AutoSizeHeight   = true,
                    Parent           = characterFlowPanel
                };

                if (character.EquippedGatheringTools.Any())
                {
                    var equippedFlowPanel = new HeaderWithToolsFlowPanel(
                        $"{character.CharacterName}'s equipped tools", textureService.EquipmentTexture, character.EquippedGatheringTools, logger)
                    {
                        ShowBorder = false,
                        Parent     = characterFlowPanel
                    };
                }

                if (character.InventoryGatheringTools.Any())
                {
                    var inventoryFlowPanel2 = new HeaderWithToolsFlowPanel(
                        $"{character.CharacterName}'s inventory", textureService.CharacterInventoryTexture, character.InventoryGatheringTools, logger)
                    {
                        ShowBorder = false,
                        Parent     = characterFlowPanel
                    };
                }
            }
        }

        private readonly TextureService _textureService;
        private readonly List<GatheringTool> _allGatheringTools;
        private readonly Gw2ApiManager _gw2ApiManager;
        private readonly Logger _logger;
        private readonly Label _infoLabel;
        private readonly FlowPanel _rootFlowPanel;
        private readonly LoadingSpinner _loadingSpinner;
        private readonly Checkbox _showOnlyUnlimitedToolsCheckbox;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private const string API_KEY_ERROR_MESSAGE = "Error: API key problem.\nPossible Reasons:\n" +
                                                     "- After starting GW2 you have to log into a character once for Blish to know which API key to use.\n" +
                                                     "- Blish needs a few more seconds to give an API token to the module. You may have to reopen window to update.\n" +
                                                     "- API key is missing in Blish. Add API key to Blish.\n" +
                                                     "- API key exists but is missing permissions. Add API key with necessary permissions to Blish.\n" +
                                                     "- API is down or has issues or something else went wrong. Check Blish log file.";

        private const int MAX_CONTENT_WIDTH = 200;
    }
}