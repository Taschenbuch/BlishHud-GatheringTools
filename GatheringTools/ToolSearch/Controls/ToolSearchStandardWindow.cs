using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules.Managers;
using GatheringTools.Settings;
using GatheringTools.ToolSearch.Model;
using GatheringTools.ToolSearch.Services;
using Microsoft.Xna.Framework;
using static Blish_HUD.ContentService;

namespace GatheringTools.ToolSearch.Controls
{
    public class ToolSearchStandardWindow : StandardWindow
    {
        public ToolSearchStandardWindow(TextureService textureService,
                                        SettingService settingService,
                                        List<GatheringTool> allGatheringTools,
                                        bool isModuleVersionDeprecated,
                                        Gw2ApiManager gw2ApiManager,
                                        Logger logger)
            : base(textureService.WindowBackgroundTexture, new Rectangle(10, 30, 235, 610), new Rectangle(30, 30, 230, 600))
        {
            _isModuleVersionDeprecated = isModuleVersionDeprecated;
            _textureService    = textureService;
            _allGatheringTools = allGatheringTools;
            _gw2ApiManager     = gw2ApiManager;
            _logger            = logger;

            if(isModuleVersionDeprecated)
            {
                _infoLabel = new Label()
                {
                    Text = "This module version is deprecated. :-(\n" +
                           "Please update to the newest module version. " +
                           "To have access to the newest module version, make sure you are also using the newest Blish HUD version.\n:-)",
                    ShowShadow = true,
                    Size = new Point(MAX_CONTENT_WIDTH, 0),
                    Font = GameService.Content.GetFont(FontFace.Menomonia, FontSize.Size20, FontStyle.Regular),
                    AutoSizeHeight = true,
                    WrapText = true,
                    Parent = this
                };
                return;
            } 

            var rootFlowPanel = new FlowPanel
            {
                FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                WidthSizingMode  = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent           = this
            };

            _showOnlyUnlimitedToolsCheckbox = new Checkbox
            {
                Text             = "Only unlimited tools",
                Checked          = settingService.ShowOnlyUnlimitedToolsSetting.Value,
                BasicTooltipText = "Show only unlimited tools",
                Parent           = rootFlowPanel,
            };

            _showBankCheckbox = new Checkbox
            {
                Text             = "Bank",
                Checked          = settingService.ShowBankToolsSetting.Value,
                BasicTooltipText = "Show gathering tools in bank",
                Parent           = rootFlowPanel,
            };

            _showSharedInventoryCheckbox = new Checkbox
            {
                Text             = "Shared inventory slots",
                Checked          = settingService.ShowSharedInventoryToolsSetting.Value,
                BasicTooltipText = "Show gathering tools in shared inventory slots",
                Parent           = rootFlowPanel,
            };

            _infoLabel = new Label()
            {
                ShowShadow     = true,
                Size           = new Point(MAX_CONTENT_WIDTH, 0),
                AutoSizeHeight = true,
                WrapText       = true,
                Parent         = rootFlowPanel
            };

            _loadingSpinnerContainer = new LoadingSpinnerContainer()
            {
                WidthSizingMode  = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Visible          = false,
                Parent           = rootFlowPanel
            };

            _toolLocationsFlowPanel = new FlowPanel()
            {
                ControlPadding = new Vector2(0, 5),
                Size           = new Point(MAX_CONTENT_WIDTH, 500),
                FlowDirection  = ControlFlowDirection.SingleTopToBottom,
                CanScroll      = true,
                Parent         = rootFlowPanel
            };

            _showOnlyUnlimitedToolsCheckbox.CheckedChanged += async (s, e) =>
            {
                settingService.ShowOnlyUnlimitedToolsSetting.Value = e.Checked;
                await ShowWindowAndUpdateToolsInUi();
            };

            _showBankCheckbox.CheckedChanged += async (s, e) =>
            {
                settingService.ShowBankToolsSetting.Value = e.Checked;
                await ShowWindowAndUpdateToolsInUi();
            };

            _showSharedInventoryCheckbox.CheckedChanged += async (s, e) =>
            {
                settingService.ShowSharedInventoryToolsSetting.Value = e.Checked;
                await ShowWindowAndUpdateToolsInUi();
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
            if (_isModuleVersionDeprecated)
                return;

            // ReSharper disable once MethodHasAsyncOverload // no need for AsyncWait because it would return instantly anyway and wont be awaited
            if (_semaphoreSlim.Wait(0) == false)
                return;

            try
            {
                await UpdateToolsInUiFromApi();
            } 
            catch(Exception e)
            {
                _logger.Error(e, "Failed to UpdateToolsInUiFromApi");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task UpdateToolsInUiFromApi()
        {
            _toolLocationsFlowPanel.ClearChildren();
            _infoLabel.Text = "Getting API data...";
            _loadingSpinnerContainer.Show();

            var (account, apiAccessFailed) = await FindGatheringToolsService.GetToolsFromApi(_allGatheringTools, _gw2ApiManager, _logger);

            _infoLabel.Text = string.Empty;
            _loadingSpinnerContainer.Hide();

            if (apiAccessFailed)
            {
                _infoLabel.Text = API_KEY_ERROR_MESSAGE;
                return;
            }

            FilterGatheringToolsService.FilterTools(
                account,
                _showOnlyUnlimitedToolsCheckbox.Checked,
                _showBankCheckbox.Checked,
                _showSharedInventoryCheckbox.Checked);

            if (account.HasTools())
                ShowToolsInUi(account, _toolLocationsFlowPanel, _textureService, _logger);
            else
                _infoLabel.Text = "No tools found with current search filter!";
        }

        private static void ShowToolsInUi(Account account,
                                          FlowPanel toolLocationsFlowPanel,
                                          TextureService textureService,
                                          Logger logger)
        {
            if (account.BankGatheringTools.Any())
            {
                new HeaderWithToolsFlowPanel(
                    "Bank",
                    account.BankGatheringTools,
                    textureService.BankTexture,
                    textureService.UnknownToolTexture,
                    logger)
                {
                    WidthSizingMode  = SizingMode.AutoSize,
                    HeightSizingMode = SizingMode.AutoSize,
                    ShowBorder       = true,
                    Parent           = toolLocationsFlowPanel
                };
            }

            if (account.SharedInventoryGatheringTools.Any())
            {
                new HeaderWithToolsFlowPanel(
                    "Shared inventory",
                    account.SharedInventoryGatheringTools,
                    textureService.SharedInventoryTexture,
                    textureService.UnknownToolTexture,
                    logger)
                {
                    WidthSizingMode  = SizingMode.AutoSize,
                    HeightSizingMode = SizingMode.AutoSize,
                    ShowBorder       = true,
                    Parent           = toolLocationsFlowPanel
                };
            }

            foreach (var character in account.Characters)
                if (character.HasTools())
                    ShowCharacter(character, toolLocationsFlowPanel, textureService, logger);
        }

        private static void ShowCharacter(Character character, FlowPanel rootFlowPanel, TextureService textureService, Logger logger)
        {
            var characterFlowPanel = new FlowPanel()
            {
                Title            = character.CharacterName,
                BasicTooltipText = character.CharacterName,
                FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                ControlPadding   = new Vector2(0, SPACING_BETWEEN_EQUIPPED_AND_INVENTORY_TOOLS),
                WidthSizingMode  = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                ShowBorder       = true,
                CanCollapse      = true,
                Parent           = rootFlowPanel
            };

            if (character.EquippedGatheringTools.Any())
            {
                new HeaderWithToolsFlowPanel(
                    $"Equipped tools",
                    character.EquippedGatheringTools,
                    textureService.EquipmentTexture,
                    textureService.UnknownToolTexture,
                    logger)
                {
                    WidthSizingMode  = SizingMode.AutoSize,
                    HeightSizingMode = SizingMode.AutoSize,
                    ShowBorder       = false,
                    Parent           = characterFlowPanel
                };
            }

            if (character.InventoryGatheringTools.Any())
            {
                new HeaderWithToolsFlowPanel(
                    $"Inventory",
                    character.InventoryGatheringTools,
                    textureService.CharacterInventoryTexture,
                    textureService.UnknownToolTexture,
                    logger)
                {
                    WidthSizingMode  = SizingMode.AutoSize,
                    HeightSizingMode = SizingMode.AutoSize,
                    ShowBorder       = false,
                    Parent           = characterFlowPanel
                };
            }
        }

        private readonly bool _isModuleVersionDeprecated;
        private readonly TextureService _textureService;
        private readonly List<GatheringTool> _allGatheringTools;
        private readonly Gw2ApiManager _gw2ApiManager;
        private readonly Logger _logger;
        private readonly Label _infoLabel;
        private readonly FlowPanel _toolLocationsFlowPanel;
        private readonly Checkbox _showOnlyUnlimitedToolsCheckbox;
        private readonly Checkbox _showBankCheckbox;
        private readonly Checkbox _showSharedInventoryCheckbox;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private readonly LoadingSpinnerContainer _loadingSpinnerContainer;

        private const int MAX_CONTENT_WIDTH = 210;
        private const int SPACING_BETWEEN_EQUIPPED_AND_INVENTORY_TOOLS = 5;

        private const string API_KEY_ERROR_MESSAGE = "Error: API problem.\nPossible Reasons:\n" +
                                                     "- After starting GW2 you have to log into a character once for Blish to know which API key to use.\n" +
                                                     "- Blish needs a few more seconds to give an API token to the module. You have to reopen window to update.\n" +
                                                     "- API key is missing in Blish. Add API key to Blish.\n" +
                                                     "- API key exists but is missing permissions. Add API key with necessary permissions to Blish.\n" +
                                                     "- If the API key has all permissions, disable and enable the module again. This can fix issues especially right after a module update.\n" +
                                                     "- API is down or has issues or something else went wrong. Check Blish log file.";
    }
}