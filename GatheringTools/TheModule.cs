using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Controls.Extern;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using GatheringTools.LogoutControl;
using GatheringTools.LogoutOverlay;
using GatheringTools.Settings;
using GatheringTools.ToolSearch.Controls;
using GatheringTools.ToolSearch.Model;
using GatheringTools.ToolSearch.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GatheringTools
{
    [Export(typeof(Module))]
    public class TheModule : Module
    {
        private static readonly Logger Logger = Logger.GetLogger<TheModule>();
        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

        [ImportingConstructor]
        public TheModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingService = new SettingService(settings);
        }

        public override IView GetSettingsView()
        {
            return new ModuleSettingsView(_settingService);
        }

        protected override async Task LoadAsync()
        {
            _textureService    = new TextureService(ContentsManager);
            _reminderContainer = new ReminderContainer(_textureService, _settingService);

            if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                ShowReminderAndResetRunningTime();
            else
                HideReminderAndResetRunningTime();

            _settingService.ReminderIsVisibleForSetupSetting.PropertyChanged += (s, e) =>
            {
                if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                    ShowReminderAndResetRunningTime();
                else
                    HideReminderAndResetRunningTime();
            };

            _logoutButton       =  new LogoutButton(_settingService, _textureService);
            _logoutButton.Click += OnLogoutButtonClicked;

            _escKeyBinding           =  new KeyBinding(Keys.Escape);
            _escKeyBinding.Activated += OnEscKeyBindingActivated;
            _escKeyBinding.Enabled   =  true;

            _enterKeyBinding           =  new KeyBinding(Keys.Enter);
            _enterKeyBinding.Activated += OnEnterKeyBindingActivated;
            _enterKeyBinding.Enabled   =  true;

            _settingService.LogoutKeyBindingSetting.Value.Activated += OnLogoutKeyBindingActivated;
            _settingService.LogoutKeyBindingSetting.Value.Enabled   =  true;

            var allGatheringTools = await FileService.GetAllGatheringToolsFromFiles(ContentsManager, Logger);
            _allGatheringTools.AddRange(allGatheringTools);

            _toolSearchStandardWindow = new ToolSearchStandardWindow(_textureService, _settingService, _allGatheringTools, Gw2ApiManager, Logger)
            {
                Emblem        = _textureService.ToolSearchWindowEmblem, // hack: has to be first to prevent bug of emblem not being visible
                Title         = "Tools",
                Location      = new Point(300, 300),
                SavesPosition = true,
                Id            = "tool search window 6f48189f-0a38-4fad-bc6a-10d323e7f1c4",
                Parent        = GameService.Graphics.SpriteScreen,
            };

            _settingService.ToolSearchKeyBindingSetting.Value.Activated += async (s, e) => await _toolSearchStandardWindow.ToggleVisibility();
            _settingService.ToolSearchKeyBindingSetting.Value.Enabled   =  true;

            _cornerIconService = new CornerIconService(
                _settingService.ShowToolSearchCornerIconSetting,
                "Click to show/hide which character has gathering tools equipped.\nIcon can be hidden by module settings.",
                (s, e) => _toolSearchStandardWindow.ToggleWindow(),
                _textureService);
        }

        protected override void Unload()
        {
            _escKeyBinding.Activated                                -= OnEscKeyBindingActivated;
            _settingService.LogoutKeyBindingSetting.Value.Activated -= OnLogoutKeyBindingActivated;

            _logoutButton.Click -= OnLogoutButtonClicked;
            _logoutButton?.Dispose();

            _textureService?.Dispose();
            _toolSearchStandardWindow?.Dispose();
            _reminderContainer?.Dispose();
            _cornerIconService?.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            _logoutButton?.ShowOrHide();
            HideReminderWhenDurationEnds(gameTime);
        }

        private void HideReminderWhenDurationEnds(GameTime gameTime)
        {
            if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                return;

            if (_reminderContainer.Visible)
            {
                _runningTime += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_runningTime > 1000 * (int)_settingService.ReminderDisplayDurationInSecondsSetting.Value)
                    HideReminderAndResetRunningTime();
            }
        }

        private void OnLogoutButtonClicked(object sender, MouseEventArgs e)
        {
            Blish_HUD.Controls.Intern.Keyboard.Stroke((VirtualKeyShort)_settingService.LogoutKeyBindingSetting.Value.PrimaryKey);
            OnLogoutKeyBindingActivated(null, EventArgs.Empty);
        }

        private void OnEscKeyBindingActivated(object sender, EventArgs e)
        {
            if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                return;

            if (_reminderContainer.Visible == false)
                return;

            if (_settingService.EscIsHidingReminderSetting.Value)
                HideReminderAndResetRunningTime();
        }

        private void OnEnterKeyBindingActivated(object sender, EventArgs e)
        {
            if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                return;

            if (_reminderContainer.Visible == false)
                return;

            if (_settingService.EnterIsHidingReminderSetting.Value)
                HideReminderAndResetRunningTime();
        }

        private void OnLogoutKeyBindingActivated(object sender, EventArgs e)
        {
            if (_reminderContainer.Visible == false)
                ShowReminderAndResetRunningTime();
        }

        private void HideReminderAndResetRunningTime()
        {
            _runningTime = 0;
            _reminderContainer.Hide();
        }

        private void ShowReminderAndResetRunningTime()
        {
            _runningTime = 0;
            _reminderContainer.Show();

            if (_settingService.ReminderScreenNotificationIsEnabledSetting.Value)
                ScreenNotification.ShowNotification(
                    _settingService.ReminderTextSetting.Value,
                    ScreenNotification.NotificationType.Error,
                    null,
                    (int)_settingService.ReminderDisplayDurationInSecondsSetting.Value);
        }

        private double _runningTime;
        private ToolSearchStandardWindow _toolSearchStandardWindow;
        private ReminderContainer _reminderContainer;
        private KeyBinding _escKeyBinding;
        private KeyBinding _enterKeyBinding;
        private SettingService _settingService;
        private TextureService _textureService;
        private CornerIconService _cornerIconService;
        private readonly List<GatheringTool> _allGatheringTools = new List<GatheringTool>();
        private LogoutButton _logoutButton;
    }
}