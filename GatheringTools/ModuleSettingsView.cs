using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings;
using Blish_HUD.Settings.UI.Views;
using GatheringTools.Services;
using Microsoft.Xna.Framework;

namespace GatheringTools
{
    public class ModuleSettingsView : View
    {
        public ModuleSettingsView(SettingService settingService)
        {
            _settingService = settingService;
        }

        protected override void Build(Container buildPanel)
        {
            _rootFlowPanel = new FlowPanel
            {
                FlowDirection       = ControlFlowDirection.SingleTopToBottom,
                CanScroll           = true,
                OuterControlPadding = new Vector2(10, 20),
                ControlPadding      = new Vector2(0, 10),
                WidthSizingMode     = SizingMode.Fill,
                HeightSizingMode    = SizingMode.Fill,
                Parent              = buildPanel
            };

            var toolSearchFlowPanel = CreateSettingsGroupFlowPanel("Tool Search", _rootFlowPanel);
            CreateSettingViewContainer(_settingService.ToolSearchKeyBindingSetting, toolSearchFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ShowToolSearchCornerIconSetting, toolSearchFlowPanel, buildPanel.Width);

            var logoutButtonFlowPanel = CreateSettingsGroupFlowPanel("Logout Button", _rootFlowPanel);
            CreateSettingViewContainer(_settingService.LogoutButtonIsVisible, logoutButtonFlowPanel, buildPanel.Width);
            _logoutSetting2 = CreateSettingViewContainer(_settingService.LogoutButtonIsVisibleOnCutScenesAndCharacterSelect, logoutButtonFlowPanel, buildPanel.Width);
            _logoutSetting3 = CreateSettingViewContainer(_settingService.LogoutButtonIsVisibleOnWorldMap, logoutButtonFlowPanel, buildPanel.Width);
            _logoutSetting4 = CreateSettingViewContainer(_settingService.LogoutButtonDragWithMouseIsEnabledSetting, logoutButtonFlowPanel, buildPanel.Width);
            _logoutSetting5 = CreateSettingViewContainer(_settingService.LogoutButtonSizeSetting, logoutButtonFlowPanel, buildPanel.Width);
            _logoutSetting6 = CreateResetLogoutPositionButton(logoutButtonFlowPanel);

            ShowOrHideLogoutButtonSettings(_settingService.LogoutButtonIsVisible.Value);
            _settingService.LogoutButtonIsVisible.SettingChanged += (s, e) => ShowOrHideLogoutButtonSettings(e.NewValue);

            var reminderFlowPanel = CreateSettingsGroupFlowPanel("Logout Reminder", _rootFlowPanel);
            CreateSettingViewContainer(_settingService.LogoutKeyBindingSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderIsVisibleForSetupSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderDisplayDurationInSecondsSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderTextSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderTextFontSizeIndexSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderBackgroundIsVisibleSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderWindowSizeSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderWindowOffsetXSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderWindowOffsetYSetting, reminderFlowPanel, buildPanel.Width);
            CreateResetReminderPositionButton(reminderFlowPanel);
            CreateSettingViewContainer(_settingService.ReminderImageIsVisibleSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderImageOutlineSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderImageSizeSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderImageOffsetXSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderImageOffsetYSetting, reminderFlowPanel, buildPanel.Width);
            CreateResetIconPositionButton(reminderFlowPanel);
            CreateSettingViewContainer(_settingService.ReminderScreenNotificationIsEnabledSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.EscIsHidingReminderSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.EnterIsHidingReminderSetting, reminderFlowPanel, buildPanel.Width);
        }

        private void ShowOrHideLogoutButtonSettings(bool isVisible)
        {
            _logoutSetting2.Visible = isVisible;
            _logoutSetting3.Visible = isVisible;
            _logoutSetting4.Visible = isVisible;
            _logoutSetting5.Visible = isVisible;
            _logoutSetting6.Visible = isVisible;
        }

        private StandardButton CreateResetLogoutPositionButton(Container parent)
        {
            var button = new StandardButton
            {
                Text             = "Reset logout button position",
                BasicTooltipText = "Reset the logout button position back to the default position.",
                Width            = 300,
                Parent           = parent,
            };

            button.Click += (s, e) =>
            {
                _settingService.LogoutButtonPositionXSetting.Value = 0;
                _settingService.LogoutButtonPositionYSetting.Value = SettingService.DEFAULT_LOGOUT_BUTTON_POSITION_Y;
            };

            return button;
        }

        private void CreateResetIconPositionButton(Container parent)
        {
            var button = new StandardButton
            {
                Text             = "Reset image position",
                BasicTooltipText = "Reset the icon position back to the default position in the reminder.",
                Width            = 200,
                Parent           = parent,
            };

            button.Click += (s, e) =>
            {
                _settingService.ReminderImageOffsetXSetting.Value = 0;
                _settingService.ReminderImageOffsetYSetting.Value = 0;
            };
        }

        private void CreateResetReminderPositionButton(Container parent)
        {
            var button = new StandardButton
            {
                Text             = "Reset reminder position",
                BasicTooltipText = "Reset the logout dialog reminder position back to the screen center.",
                Width            = 200,
                Parent           = parent,
            };

            button.Click += (s, e) =>
            {
                _settingService.ReminderWindowOffsetXSetting.Value = 0;
                _settingService.ReminderWindowOffsetYSetting.Value = 0;
            };
        }

        private static FlowPanel CreateSettingsGroupFlowPanel(string title, Container parent)
        {
            return new FlowPanel
            {
                Title               = title,
                FlowDirection       = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(10, 10),
                ShowBorder          = true,
                WidthSizingMode     = SizingMode.Fill,
                HeightSizingMode    = SizingMode.AutoSize,
                Parent              = parent
            };
        }

        private static ViewContainer CreateSettingViewContainer(SettingEntry settingEntry, Container parent, int width)
        {
            var view = new ViewContainer { Parent = parent };
            view.Show(SettingView.FromType(settingEntry, width));
            return view;
        }

        private readonly SettingService _settingService;
        private FlowPanel _rootFlowPanel;
        private StandardButton _logoutSetting6;
        private ViewContainer _logoutSetting5;
        private ViewContainer _logoutSetting3;
        private ViewContainer _logoutSetting4;
        private ViewContainer _logoutSetting2;
    }
}