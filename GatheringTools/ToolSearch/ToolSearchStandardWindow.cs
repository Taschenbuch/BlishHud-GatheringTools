using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace GatheringTools.ToolSearch
{
    public class ToolSearchStandardWindow : StandardWindow
    {
        public ToolSearchStandardWindow(SettingEntry<bool> showOnlyUnlimitedToolsSetting,
                                        Texture2D windowBackgroundTexture,
                                        Gw2ApiManager gw2ApiManager,
                                        Logger logger)
            : base(windowBackgroundTexture, new Rectangle(10, 30, 235, 610), new Rectangle(30, 30, 230, 600))
        {
            _gw2ApiManager                 = gw2ApiManager;
            _logger                        = logger;

            _infoLabel = new Label()
            {
                Location       = new Point(0, 30),
                TextColor      = Color.White,
                ShowShadow     = true,
                Size           = new Point(200, 0),
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

            _showOnlyUnlimitedToolsCheckbox.CheckedChanged += (s, e) =>
            {
                showOnlyUnlimitedToolsSetting.Value = e.Checked;
                ShowWindowAndGetToolsFromApi();
            };

            _charactersFlowPanel = new FlowPanel()
            {
                Location      = new Point(0, 30),
                Size          = new Point(200, 500),
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                CanScroll     = true,
                Parent        = this,
            };
        }

        public void ToggleVisibility()
        {
            if (Visible)
                Hide();
            else
                ShowWindowAndGetToolsFromApi();
        }

        public void HideAndShow()
        {
            Hide();
            ShowWindowAndGetToolsFromApi();
        }

        public void ShowWindowAndGetToolsFromApi()
        {
            Show();

            if (_uiIsUpdating)
                return;

            _uiIsUpdating = true;

            Task.Run(async () =>
                {
                    var charactersAndTools = await GetToolsFromApi();

                    if(_apiAccessFailed)
                        _infoLabel.Text = API_KEY_ERROR_MESSAGE;
                    else
                        ShowToolsInUi(charactersAndTools);

                    _uiIsUpdating = false;
                }
            );
        }

        public async Task<List<CharacterAndTools>> GetToolsFromApi()
        {
            _apiAccessFailed = false;
            _infoLabel.Text = string.Empty;
            _charactersFlowPanel.ClearChildren();

            if (_gw2ApiManager.HasPermissions(_gw2ApiManager.Permissions) == false)
            {
                _apiAccessFailed = true;
                _loadingSpinner.Hide();
                return new List<CharacterAndTools>();
            }

            _loadingSpinner.Show();
            _infoLabel.Text = "Getting API data...";

            try
            {
                var charactersAndTools = await GatheringToolsService.GetCharactersAndTools(_gw2ApiManager);
                _infoLabel.Text = string.Empty;
                return charactersAndTools;
            }
            catch (Exception e)
            {
                _apiAccessFailed = true;
                _logger.Error("Could not get gathering tools from API", e);
                return new List<CharacterAndTools>();
            }
            finally
            {
                _loadingSpinner.Hide();
            }
        }

        private void ShowToolsInUi(List<CharacterAndTools> charactersAndTools)
        {
            var filteredCharactersAndTools = charactersAndTools.Where(c => c.HasTools()).ToList();

            if (_showOnlyUnlimitedToolsCheckbox.Checked)
                filteredCharactersAndTools = filteredCharactersAndTools.Where(c => c.HasUnlimitedTools()).ToList();

            if (filteredCharactersAndTools.Any() == false)
            {
                _infoLabel.Text = "No tools found with current search filter or no character has tools equipped!";
                return;
            }

            foreach (var characterAndTools in filteredCharactersAndTools)
            {
                var characterAndToolsFlowPanel = new CharacterAndToolsFlowPanel(characterAndTools, _showOnlyUnlimitedToolsCheckbox.Checked, _logger)
                {
                    FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                    WidthSizingMode  = SizingMode.AutoSize,
                    HeightSizingMode = SizingMode.AutoSize,
                    ShowBorder       = true,
                    Parent           = _charactersFlowPanel
                };
            }
        }

        private readonly Gw2ApiManager _gw2ApiManager;
        private readonly Logger _logger;
        private readonly Label _infoLabel;
        private readonly FlowPanel _charactersFlowPanel;
        private readonly LoadingSpinner _loadingSpinner;
        private readonly Checkbox _showOnlyUnlimitedToolsCheckbox;
        private bool _uiIsUpdating;
        private bool _apiAccessFailed;

        private const string API_KEY_ERROR_MESSAGE = "Error: API key problem.\nPossible Reasons:\n" +
                                                     "- After starting GW2 you have to log into a character once for Blish to know which API key to use.\n" +
                                                     "- Blish needs a few more seconds to give an API token to the module. You may have to reopen window to update.\n" +
                                                     "- API key is missing in Blish. Add API key.\n" +
                                                     "- API key exists but is missing permissions. Add new API key to Blish with necessary permissions.\n" +
                                                     "- Something else went wrong. Check BlishHUD log file.";
    }
}