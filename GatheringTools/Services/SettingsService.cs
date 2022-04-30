using Blish_HUD.Input;
using Blish_HUD.Settings;
using GatheringTools.LogoutOverlay;
using Microsoft.Xna.Framework.Input;

namespace GatheringTools.Services
{
    public class SettingService // singular because Setting"s"Service already exists in Blish
    {
        public SettingService(SettingCollection settings)
        {
            LogoutKeyBindingSetting = settings.DefineSetting(
                "Logout key binding",
                new KeyBinding(Keys.F12),
                () => "Logout (must match ingame key)",
                () => "Double-click to change it. Logout key has to match the ingame logout key (default F12).");

            ToolSearchKeyBindingSetting = settings.DefineSetting(
                "tool search key binding",
                new KeyBinding(ModifierKeys.Alt, Keys.C),
                () => "tool search window",
                () => "Double-click to change it. Will show or hide the tool search window.");

            ReminderTextSetting = settings.DefineSetting(
                "text (logout overlay)",
                "shared inventory",
                () => "reminder text",
                () => "text shown inside the reminder window");

            ReminderDisplayDurationInSecondsSetting = settings.DefineSetting(
                "display duration (logout overlay)",
                DisplayDuration.Seconds3,
                () => "reminder display duration",
                () => "The reminder will disappear automatically after this time has expired");

            ReminderWindowSizeSetting = settings.DefineSetting(
                "window size (logout overlay)",
                34,
                () => "reminder size",
                () => "Change reminder window size to fit to the size of the logout dialog with your current screen settings");
            ReminderWindowSizeSetting.SetRange(1, 100);

            ReminderTextFontSizeIndexSetting = FontService.CreateFontSizeIndexSetting(settings);

            ReminderIconSizeSetting = settings.DefineSetting(
                "reminder icon size (logout overlay)",
                60,
                () => "icon size",
                () => "Change size of the icons in the reminder window");
            ReminderIconSizeSetting.SetRange(10, 300);

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
                () => "show reminder for easier setup of position etc. This will ignore display duration and ESC or ENTER being pressed. Do not forget to uncheck after you set up everything.");

            ShowToolSearchCornerIconSetting = settings.DefineSetting(
                "show tool search corner icon",
                true,
                () => "show sickle icon",
                () => "Show sickle icon at the top left of GW2 next to other menu icons. Icon can be clicked to show/hide the gathering tool search window");

            var internalSettingSubCollection = settings.AddSubCollection("internal settings (not visible in UI)");
            ShowOnlyUnlimitedToolsSetting   = internalSettingSubCollection.DefineSetting("only unlimited tools", true);
            ShowBankToolsSetting            = internalSettingSubCollection.DefineSetting("show bank tools", true);
            ShowSharedInventoryToolsSetting = internalSettingSubCollection.DefineSetting("show shared inventory tools", true);
        }

        public SettingEntry<int> ReminderWindowSizeSetting { get; }
        public SettingEntry<DisplayDuration> ReminderDisplayDurationInSecondsSetting { get; }
        public SettingEntry<string> ReminderTextSetting { get; }
        public SettingEntry<int> ReminderTextFontSizeIndexSetting { get; }
        public SettingEntry<int> ReminderIconSizeSetting { get; }
        public SettingEntry<bool> EscIsHidingReminderSetting { get; }
        public SettingEntry<bool> EnterIsHidingReminderSetting { get; }
        public SettingEntry<bool> ReminderIsVisibleForSetupSetting { get; }
        public SettingEntry<bool> ShowToolSearchCornerIconSetting { get; }
        public SettingEntry<bool> ShowOnlyUnlimitedToolsSetting { get; }
        public SettingEntry<bool> ShowSharedInventoryToolsSetting { get; }
        public SettingEntry<bool> ShowBankToolsSetting { get; }
        public SettingEntry<KeyBinding> ToolSearchKeyBindingSetting { get; }
        public SettingEntry<KeyBinding> LogoutKeyBindingSetting { get; }
    }
}