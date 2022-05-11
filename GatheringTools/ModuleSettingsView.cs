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

            var reminderFlowPanel = CreateSettingsGroupFlowPanel("Logout Reminder", _rootFlowPanel);
            CreateSettingViewContainer(_settingService.LogoutKeyBindingSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderDisplayDurationInSecondsSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderTextSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderTextFontSizeIndexSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderImageSizeSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderWindowSizeSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderWindowOffsetXSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderWindowOffsetYSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderImageOffsetXSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderImageOffsetYSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderImageOutlineSetting, reminderFlowPanel, buildPanel.Width);
            CreateResetReminderPositionButton(reminderFlowPanel);
            CreateResetIconPositionButton(reminderFlowPanel);
            CreateSettingViewContainer(_settingService.ReminderImageIsVisibleSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderBackgroundIsVisibleSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderScreenNotificationIsEnabledSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.EscIsHidingReminderSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.EnterIsHidingReminderSetting, reminderFlowPanel, buildPanel.Width);
            CreateSettingViewContainer(_settingService.ReminderIsVisibleForSetupSetting, reminderFlowPanel, buildPanel.Width);
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

        private static void CreateSettingViewContainer(SettingEntry settingEntry, Container parent, int width)
        {
            var view = new ViewContainer { Parent = parent };
            view.Show(SettingView.FromType(settingEntry, width));
        }

        private readonly SettingService _settingService;
        private FlowPanel _rootFlowPanel;
    }
}