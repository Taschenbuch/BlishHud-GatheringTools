using System;
using System.ComponentModel.Composition;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using GatheringTools.LogoutOverlay;
using GatheringTools.ToolSearch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GatheringTools
{
    [Export(typeof(Module))]
    public class TheModule : Module
    {
        private static readonly Logger Logger = Logger.GetLogger<TheModule>();
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;

        [ImportingConstructor]
        public TheModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            _logoutKeyBindingSetting = settings.DefineSetting(
                "Logout key binding",
                new KeyBinding(Keys.F12),
                () => "Logout (must match ingame key)",
                () => "Double-click to change it. Logout key has to match the ingame logout key (default F12).");

            _toolSearchKeyBindingSetting = settings.DefineSetting(
                "tool search key binding",
                new KeyBinding(ModifierKeys.Alt, Keys.C),
                () => "tool search window",
                () => "Double-click to change it. Will show or hide the tool search window.");

            _reminderTextSetting = settings.DefineSetting(
                "text (logout overlay)",
                "shared inventory",
                () => "reminder text",
                () => "text shown inside the reminder window");

            _reminderDisplayDurationInSecondsSetting = settings.DefineSetting(
                "display duration (logout overlay)",
                DisplayDuration.Seconds3,
                () => "reminder display duration (1-10s)", 
                () => "The reminder will disappear automatically after this time has expired"); 

            _reminderWindowSizeSetting = settings.DefineSetting(
                "window size (logout overlay)",
                34,
                () => "reminder size", 
                () => "Change reminder window size to fit to the size of the logout dialog with your current screen settings");  
            _reminderWindowSizeSetting.SetRange(1, 100);

            _reminderTextFontSizeIndexSetting = FontService.CreateFontSizeIndexSetting(settings);

            _reminderIconSizeSetting = settings.DefineSetting(
                "reminder icon size (logout overlay)",
                60,
                () => "icon size",
                () => "Change size of the icons in the reminder window");
            _reminderIconSizeSetting.SetRange(10, 300);

            _escIsHidingReminderSetting = settings.DefineSetting(
                "hide on ESC",
                true,
                () => "hide on ESC",
                () => "When you press ESC to close the logout dialog, the reminder will be hidden, too");

            _enterIsHidingReminderSetting = settings.DefineSetting(
                "hide on ENTER",
                true,
                () => "hide on ENTER",
                () => "When you press ENTER to switch to the character selection, the reminder will be hidden");

            _reminderIsVisibleForSetupSetting = settings.DefineSetting(
                "show reminder for setup",
                false,
                () => "show reminder permanently for setup",
                () => "show reminder for easier setup of position etc. This will ignore display duration and ESC or ENTER being pressed. Do not forget to uncheck after you set up everything.");

            _showToolSearchCornerIconSetting = settings.DefineSetting(
                "show tool search corner icon",
                true,
                () => "show sickle icon",
                () => "Show sickle icon at the top left of GW2 next to other menu icons. Icon can be clicked to show/hide the gathering tool search window");

            _internalSettingSubCollection = settings.AddSubCollection("internal settings (not visible in UI)");

            _showOnlyUnlimitedToolsSetting = _internalSettingSubCollection.DefineSetting(
                "only unlimited tools",
                true,
                () => "only unlimited tools",
                () => "show only unlimited tools in the tool search window.");
        }

        protected override void Initialize()
        {
            _windowBackgroundTexture = ContentsManager.GetTexture("155985.png");
            _sickleTexture           = ContentsManager.GetTexture("sickle.png");

            _toolSearchStandardWindow = new ToolSearchStandardWindow(_showOnlyUnlimitedToolsSetting, _windowBackgroundTexture, Gw2ApiManager, Logger)
            {
                Emblem           = _sickleTexture, // hack: has to be first to prevent bug of emblem not being visible
                Title            = "Tools",
                BasicTooltipText = "Shows which character has gathering tools equipped.",
                Location         = new Point(300, 300),
                SavesPosition    = true,
                Id               = "tool search window 6f48189f-0a38-4fad-bc6a-10d323e7f1c4",
                Parent           = GameService.Graphics.SpriteScreen,
            };

            _reminderContainer = new ReminderContainer(ContentsManager);
            _reminderContainer.UpdateReminderText(_reminderTextSetting.Value);
            _reminderContainer.UpdateReminderTextFontSize(_reminderTextFontSizeIndexSetting.Value);
            _reminderContainer.UpdateContainerSizeAndMoveAboveLogoutDialog(_reminderWindowSizeSetting.Value);
            _reminderContainer.UpdateIconSize(_reminderIconSizeSetting.Value);

            if (_reminderIsVisibleForSetupSetting.Value)
                ShowReminderAndResetRunningTime();
            else
                HideReminderAndResetRunningTime();

            _reminderIsVisibleForSetupSetting.PropertyChanged += (s, e) =>
            {
                if (_reminderIsVisibleForSetupSetting.Value)
                    ShowReminderAndResetRunningTime();
                else
                    HideReminderAndResetRunningTime();
            };

            _reminderTextFontSizeIndexSetting.SettingChanged += (s, e) => _reminderContainer.UpdateReminderTextFontSize(e.NewValue);
            _reminderTextSetting.SettingChanged              += (s, e) => _reminderContainer.UpdateReminderText(e.NewValue);
            _reminderWindowSizeSetting.SettingChanged        += (s, e) => _reminderContainer.UpdateContainerSizeAndMoveAboveLogoutDialog(e.NewValue);
            _reminderIconSizeSetting.SettingChanged          += (s, e) => _reminderContainer.UpdateIconSize(e.NewValue);
            GameService.Graphics.SpriteScreen.Resized        += OnSpriteScreenResized;

            _escKeyBinding           =  new KeyBinding(Keys.Escape);
            _escKeyBinding.Activated += OnEscKeyBindingActivated;
            _escKeyBinding.Enabled   =  true;

            _enterKeyBinding           =  new KeyBinding(Keys.Enter);
            _enterKeyBinding.Activated += OnEnterKeyBindingActivated;
            _enterKeyBinding.Enabled   =  true;
            
            _logoutKeyBindingSetting.Value.Activated += OnLogoutKeyBindingActivated;
            _logoutKeyBindingSetting.Value.Enabled   =  true;

            _toolSearchKeyBindingSetting.Value.Activated += (s, e) => _toolSearchStandardWindow.ToggleVisibility();
            _toolSearchKeyBindingSetting.Value.Enabled   =  true;

            _cornerIconService = new CornerIconService(_showToolSearchCornerIconSetting, _toolSearchStandardWindow, _sickleTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_reminderIsVisibleForSetupSetting.Value)
                return;

            if (_reminderContainer.Visible)
            {
                _runningTime += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_runningTime > 1000 * (int) _reminderDisplayDurationInSecondsSetting.Value)
                    HideReminderAndResetRunningTime();
            }
        }

        protected override void Unload()
        {
            GameService.Graphics.SpriteScreen.Resized    -= OnSpriteScreenResized;
            _escKeyBinding.Activated                     -= OnEscKeyBindingActivated;
            _logoutKeyBindingSetting.Value.Activated     -= OnLogoutKeyBindingActivated;
            
            _windowBackgroundTexture?.Dispose();
            _sickleTexture?.Dispose();
            
            _toolSearchStandardWindow?.Dispose();
            _reminderContainer?.Dispose();
            _cornerIconService?.RemoveCornerIcon();
        }

        private void OnSpriteScreenResized(object sender, ResizedEventArgs e)
        {
            _reminderContainer.MoveAboveLogoutDialog();
        }

        private void OnEscKeyBindingActivated(object sender, System.EventArgs e)
        {
            if (_reminderIsVisibleForSetupSetting.Value)
                return;

            if (_reminderContainer.Visible == false)
                return;

            if(_escIsHidingReminderSetting.Value)
                HideReminderAndResetRunningTime();
        }

        private void OnEnterKeyBindingActivated(object sender, EventArgs e)
        {
            if (_reminderIsVisibleForSetupSetting.Value)
                return;

            if (_reminderContainer.Visible == false)
                return;

            if (_enterIsHidingReminderSetting.Value)
                HideReminderAndResetRunningTime();
        }

        private void OnLogoutKeyBindingActivated(object sender, System.EventArgs e)
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
            ScreenNotification.ShowNotification(_reminderTextSetting.Value, ScreenNotification.NotificationType.Error);
        }

        private double _runningTime;
        private ToolSearchStandardWindow _toolSearchStandardWindow;
        private ReminderContainer _reminderContainer;
        private SettingEntry<KeyBinding> _logoutKeyBindingSetting;
        private SettingEntry<KeyBinding> _toolSearchKeyBindingSetting;
        private SettingEntry<DisplayDuration> _reminderDisplayDurationInSecondsSetting;
        private SettingEntry<string> _reminderTextSetting;
        private SettingEntry<bool> _reminderIsVisibleForSetupSetting;
        private SettingEntry<bool> _escIsHidingReminderSetting;
        private SettingEntry<bool> _enterIsHidingReminderSetting;
        private SettingEntry<bool> _showOnlyUnlimitedToolsSetting;
        private SettingEntry<bool> _showToolSearchCornerIconSetting;
        private SettingEntry<int> _reminderWindowSizeSetting;
        private SettingEntry<int> _reminderTextFontSizeIndexSetting;
        private SettingEntry<int> _reminderIconSizeSetting;
        private SettingCollection _internalSettingSubCollection;
        private KeyBinding _escKeyBinding;
        private KeyBinding _enterKeyBinding;
        private Texture2D _sickleTexture;
        private Texture2D _windowBackgroundTexture;
        private CornerIconService _cornerIconService;
    }
}