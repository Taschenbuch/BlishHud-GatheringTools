using Blish_HUD.Input;
using Blish_HUD.Settings;
using GatheringTools.LogoutOverlay;
using Microsoft.Xna.Framework.Input;

namespace GatheringTools.Settings
{
    public class SettingService // singular because Setting"s"Service already exists in Blish
    {
        public SettingService(SettingCollection settings)
        {
            LogoutKeyBindingSetting = settings.DefineSetting(
                "Logout key binding",
                new KeyBinding(Keys.F12),
                () => "Logout (must match ingame key)",
                () => "Double-click to change it. Logout key has to match the ingame logout key (default F12).\n" +
                      "Is used for the custom logout button and for the reminder.\n" +
                      "The custom logout button does not support modifier keys (CTRL, ALT, ...).");

            ToolSearchKeyBindingSetting = settings.DefineSetting(
                "tool search key binding",
                new KeyBinding(ModifierKeys.Alt, Keys.C),
                () => "tool search window",
                () => "Double-click to change it. Will show or hide the tool search window.");

            ReminderTextSetting = settings.DefineSetting(
                "text (logout overlay)",
                "Don't forget Tools!",
                () => "reminder text",
                () => "text shown inside the reminder window");

            ReminderDisplayDurationInSecondsSetting = settings.DefineSetting(
                "display duration (logout overlay)",
                DisplayDuration.Seconds3,
                () => "reminder display duration",
                () => "The reminder will disappear automatically after this time has expired.\n" +
                      "This setting is ignored when the 'hide on ESC/ENTER' setting is enabled");

            ReminderWindowSizeSetting = settings.DefineSetting(
                "window size (logout overlay)",
                34,
                () => "reminder background size",
                () => "Change reminder background size to fit to the size of the logout dialog " +
                      "with your current screen settings");
            ReminderWindowSizeSetting.SetRange(1, 100);

            ReminderWindowOffsetXSetting = settings.DefineSetting(
                "reminder window offset x",
                0,
                () => "reminder X position",
                () => "Change the reminder window position relatively to the logout dialog.\n" +
                      "Position of the additional reminder hint is not affected.");
            ReminderWindowOffsetXSetting.SetRange(-1000, 1000);

            ReminderWindowOffsetYSetting = settings.DefineSetting(
                "reminder window offset y",
                0,
                () => "reminder Y position",
                () => "Change the reminder window position relatively to the logout dialog.\n" +
                      "Position of the additional reminder hint is not affected.");
            ReminderWindowOffsetYSetting.SetRange(-1000, 1000);

            ReminderTextFontSizeIndexSetting = FontService.CreateFontSizeIndexSetting(settings);

            ReminderImageSizeSetting = settings.DefineSetting(
                "reminder icon size (logout overlay)",
                600,
                () => "image size",
                () => "Change size of the image in the reminder window.");
            ReminderImageSizeSetting.SetRange(10, 2000);

            ReminderImageOffsetXSetting = settings.DefineSetting(
                "reminder icon offset x",
                0,
                () => "image X position",
                () => "Change the position of the reminder image relatively to reminder position.");
            ReminderImageOffsetXSetting.SetRange(-1000, 1000);

            ReminderImageOffsetYSetting = settings.DefineSetting(
                "reminder icon offset y",
                0,
                () => "image Y position",
                () => "Change the position of the reminder image relatively to reminder position.");
            ReminderImageOffsetYSetting.SetRange(-1000, 1000);

            ReminderBackgroundIsVisibleSetting = settings.DefineSetting(
                "show reminder background",
                true,
                () => "show reminder background",
                () => "Show reminder background to hide logout text.");

            ReminderImageOutlineSetting = settings.DefineSetting(
                "reminder image outline",
                Outline.Small,
                () => "image outline",
                () => "Change the outline around the reminder image.");

            ReminderImageIsVisibleSetting = settings.DefineSetting(
                "show reminder icons",
                true,
                () => "show image in reminder",
                () => "Show image in the reminder.");

            ReminderScreenNotificationIsEnabledSetting = settings.DefineSetting(
                "reminder screen notification is enabled",
                true,
                () => "additional reminder hint",
                () => "Show an additional floating reminder hint above the logout dialog.\n" +
                      "This is hint is not affected by 'hide on ESC/ENTER'.");

            EscIsHidingReminderSetting = settings.DefineSetting(
                "hide on ESC",
                true,
                () => "hide on ESC",
                () => "When you press ESC to close the logout dialog, the reminder will be hidden, too");

            EnterIsHidingReminderSetting = settings.DefineSetting(
                "hide on ENTER",
                true,
                () => "hide on ENTER",
                () => "When you press ENTER to switch to the character selection, the reminder will be hidden");

            ReminderIsVisibleForSetupSetting = settings.DefineSetting(
                "show reminder for setup",
                false,
                () => "show reminder permanently for setup",
                () => "show reminder for easier setup of position etc. This will ignore " +
                      "display duration and ESC or ENTER being pressed. Do not forget to " +
                      "uncheck after you set up everything.");

            ShowToolSearchCornerIconSetting = settings.DefineSetting(
                "show tool search corner icon",
                true,
                () => "show sickle icon",
                () => "Show sickle icon at the top left of GW2 next to other menu icons. Icon " +
                      "can be clicked to show/hide the gathering tool search window");

            LogoutButtonIsVisible = settings.DefineSetting(
                "show logout button",
                false,
                () => "show",
                () => "Show a logout button which can be pressed to open the logout dialog");

            LogoutButtonDragWithMouseIsEnabledSetting = settings.DefineSetting(
                "dragging logout button is allowed",
                true,
                () => "allow dragging with mouse",
                () => "Allow dragging the button by moving the mouse while left mouse button is pressed.");

            LogoutButtonIsVisibleOnWorldMap = settings.DefineSetting(
                "logout button visible on world map",
                true,
                () => "show on world map",
                () => "logout button visible when world map is open.\n" +
                      "But only when 'show logout button' is enabled.");

            LogoutButtonIsVisibleOnCutScenesAndCharacterSelect = settings.DefineSetting(
                "logout button visible when not ingame",
                true,
                () => "show on character selection / cut scenes",
                () => "logout button visible when on character selection and cut scenes.\n" +
                      "But only when 'show logout button' is enabled.");

            LogoutButtonSizeSetting = settings.DefineSetting(
                "logout button size",
                100,
                () => "logout button size",
                () => "Change the logout button size.");
            LogoutButtonSizeSetting.SetRange(10, 500);

            var internalSettingSubCollection = settings.AddSubCollection("internal settings (not visible in UI)");
            ShowOnlyUnlimitedToolsSetting   = internalSettingSubCollection.DefineSetting("only unlimited tools", true);
            ShowBankToolsSetting            = internalSettingSubCollection.DefineSetting("show bank tools", true);
            ShowSharedInventoryToolsSetting = internalSettingSubCollection.DefineSetting("show shared inventory tools", true);
            LogoutButtonPositionXSetting    = internalSettingSubCollection.DefineSetting("logout button x position", 0);
            LogoutButtonPositionYSetting    = internalSettingSubCollection.DefineSetting("logout button y position", DEFAULT_LOGOUT_BUTTON_POSITION_Y);

            LogoutButtonPositionXSetting.SetRange(0, 4000);
            LogoutButtonPositionYSetting.SetRange(0, 4000);
        }

        public SettingEntry<bool> LogoutButtonIsVisible { get; }
        public SettingEntry<bool> LogoutButtonDragWithMouseIsEnabledSetting { get; }
        public SettingEntry<int> LogoutButtonPositionYSetting { get; }
        public SettingEntry<int> LogoutButtonPositionXSetting { get; }
        public SettingEntry<bool> LogoutButtonIsVisibleOnCutScenesAndCharacterSelect { get; }
        public SettingEntry<bool> LogoutButtonIsVisibleOnWorldMap { get; }
        public SettingEntry<int> LogoutButtonSizeSetting { get; }
        public SettingEntry<bool> ReminderScreenNotificationIsEnabledSetting { get; }
        public SettingEntry<int> ReminderWindowSizeSetting { get; }
        public SettingEntry<int> ReminderWindowOffsetXSetting { get; }
        public SettingEntry<int> ReminderWindowOffsetYSetting { get; }
        public SettingEntry<int> ReminderImageOffsetXSetting { get; }
        public SettingEntry<int> ReminderImageOffsetYSetting { get; }
        public int ReminderWindowOffsetX => ReminderWindowOffsetXSetting.Value;
        public int ReminderWindowOffsetY => ReminderWindowOffsetYSetting.Value;
        public int ReminderImageOffsetX => ReminderImageOffsetXSetting.Value;
        public int ReminderImageOffsetY => ReminderImageOffsetYSetting.Value;
        public SettingEntry<Outline> ReminderImageOutlineSetting { get; }
        public SettingEntry<DisplayDuration> ReminderDisplayDurationInSecondsSetting { get; }
        public SettingEntry<string> ReminderTextSetting { get; }
        public SettingEntry<int> ReminderTextFontSizeIndexSetting { get; }
        public SettingEntry<int> ReminderImageSizeSetting { get; }
        public SettingEntry<bool> ReminderImageIsVisibleSetting { get; }
        public SettingEntry<bool> ReminderBackgroundIsVisibleSetting { get; }
        public SettingEntry<bool> EscIsHidingReminderSetting { get; }
        public SettingEntry<bool> EnterIsHidingReminderSetting { get; }
        public SettingEntry<bool> ReminderIsVisibleForSetupSetting { get; }
        public SettingEntry<bool> ShowToolSearchCornerIconSetting { get; }
        public SettingEntry<bool> ShowOnlyUnlimitedToolsSetting { get; }
        public SettingEntry<bool> ShowSharedInventoryToolsSetting { get; }
        public SettingEntry<bool> ShowBankToolsSetting { get; }
        public SettingEntry<KeyBinding> ToolSearchKeyBindingSetting { get; }
        public SettingEntry<KeyBinding> LogoutKeyBindingSetting { get; }

        public const int DEFAULT_LOGOUT_BUTTON_POSITION_Y = 50;
    }
}