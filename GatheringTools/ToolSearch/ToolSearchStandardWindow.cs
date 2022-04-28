using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                                        List<GatheringTool> allGatheringTools,
                                        Gw2ApiManager gw2ApiManager,
                                        Logger logger)
            : base(windowBackgroundTexture, new Rectangle(10, 30, 235, 610), new Rectangle(30, 30, 230, 600))
        {
            _allGatheringTools = allGatheringTools;
            _gw2ApiManager     = gw2ApiManager;
            _logger            = logger;

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

            _showOnlyUnlimitedToolsCheckbox.CheckedChanged += async (s, e) => 
            {
                showOnlyUnlimitedToolsSetting.Value = e.Checked;
                await ShowWindowAndUpdateToolsInUi();
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
            _charactersFlowPanel.ClearChildren();
            _infoLabel.Text = "Getting API data...";
            _loadingSpinner.Show();

            var charactersAndTools = await GetToolsFromApi(_allGatheringTools, _gw2ApiManager);

            _infoLabel.Text = string.Empty;
            _loadingSpinner.Hide();

            if (_apiAccessFailed)
            {
                _infoLabel.Text = API_KEY_ERROR_MESSAGE;
                return;
            }

            var filteredCharactersAndTools = FilterCharacters(charactersAndTools, _showOnlyUnlimitedToolsCheckbox.Checked);

            if (filteredCharactersAndTools.Any())
                ShowToolsInUi(filteredCharactersAndTools);
            else
                _infoLabel.Text = "No tools found with current search filter or no character has tools equipped!";
        }

        private async Task<List<CharacterAndTools>> GetToolsFromApi(List<GatheringTool> allGatheringTools, Gw2ApiManager gw2ApiManager)
        {
            _apiAccessFailed = false;

            if (gw2ApiManager.HasPermissions(gw2ApiManager.Permissions) == false)
            {
                _apiAccessFailed = true;
                return new List<CharacterAndTools>();
            }

            try
            {
                return await GatheringToolsService.GetCharactersAndTools(allGatheringTools, gw2ApiManager);
            }
            catch (Exception e)
            {
                _apiAccessFailed = true;
                _logger.Error(e, "Could not get gathering tools from API");
                return new List<CharacterAndTools>();
            }
        }

        private static List<CharacterAndTools> FilterCharacters(List<CharacterAndTools> charactersAndTools, bool showOnlyUnlimitedTools)
        {
            var filteredCharactersAndTools = charactersAndTools.Where(c => c.HasTools()).ToList();

            if (showOnlyUnlimitedTools)
                filteredCharactersAndTools = filteredCharactersAndTools.Where(c => c.HasUnlimitedTools()).ToList();

            return filteredCharactersAndTools;
        }

        private void ShowToolsInUi(List<CharacterAndTools> charactersAndTools)
        {
            foreach (var characterAndTools in charactersAndTools)
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

        private readonly List<GatheringTool> _allGatheringTools;
        private readonly Gw2ApiManager _gw2ApiManager;
        private readonly Logger _logger;
        private readonly Label _infoLabel;
        private readonly FlowPanel _charactersFlowPanel;
        private readonly LoadingSpinner _loadingSpinner;
        private readonly Checkbox _showOnlyUnlimitedToolsCheckbox;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private bool _apiAccessFailed;

        private const string API_KEY_ERROR_MESSAGE = "Error: API key problem.\nPossible Reasons:\n" +
                                                     "- After starting GW2 you have to log into a character once for Blish to know which API key to use.\n" +
                                                     "- Blish needs a few more seconds to give an API token to the module. You may have to reopen window to update.\n" +
                                                     "- API key is missing in Blish. Add API key to Blish.\n" +
                                                     "- API key exists but is missing permissions. Add API key with necessary permissions to Blish.\n" +
                                                     "- API is down or has issues or something else went wrong. Check Blish log file.";
    }
}