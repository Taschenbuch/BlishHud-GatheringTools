using Blish_HUD;
using Blish_HUD.Controls;
using GatheringTools.Services;
using GatheringTools.ToolSearch.Services;
using Microsoft.Xna.Framework;

namespace GatheringTools.LogoutOverlay
{
    public class ReminderContainer : Container
    {
        public ReminderContainer(TextureService textureService, SettingService settingService)
        {
            Parent          = GameService.Graphics.SpriteScreen;
            _settingService = settingService;

            _reminderBackgroundImage = new Image(textureService.ReminderBackgroundTexture)
            {
                Size   = Size,
                Parent = this
            };

            _reminderIconImage = new Image(textureService.ReminderIconTexture)
            {
                ClipsBounds = false,
                Parent      = this
            };

            _reminderTextLabel = new Label()
            {
                TextColor      = Color.Red,
                ShowShadow     = true,
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
                ClipsBounds    = false,
                Parent         = this
            };

            UpdateText(settingService.ReminderTextSetting.Value);
            UpdateTextFontSize(settingService.ReminderTextFontSizeIndexSetting.Value);
            UpdateIconSize(settingService.ReminderIconSizeSetting.Value);
            UpdateIconsVisibility(settingService.ReminderIconsAreVisibleSetting.Value);
            UpdateContainerSizeAndMoveAboveLogoutDialog(settingService.ReminderWindowSizeSetting.Value);

            settingService.ReminderTextFontSizeIndexSetting.SettingChanged += (s, e) => UpdateTextFontSize(e.NewValue);
            settingService.ReminderTextSetting.SettingChanged              += (s, e) => UpdateText(e.NewValue);
            settingService.ReminderWindowSizeSetting.SettingChanged        += (s, e) => UpdateContainerSizeAndMoveAboveLogoutDialog(e.NewValue);
            settingService.ReminderIconSizeSetting.SettingChanged          += (s, e) => UpdateIconSize(e.NewValue);
            settingService.ReminderIconsAreVisibleSetting.SettingChanged  += (s, e) => UpdateIconsVisibility(e.NewValue);
            settingService.ReminderWindowOffsetXSetting.SettingChanged     += (s, e) => MoveAboveLogoutDialogAndApplyOffsetFromSettings();
            settingService.ReminderWindowOffsetYSetting.SettingChanged     += (s, e) => MoveAboveLogoutDialogAndApplyOffsetFromSettings();
            settingService.ReminderIconOffsetYSetting.SettingChanged       += (s, e) => UpdateLabelAndIconLocations();
            GameService.Graphics.SpriteScreen.Resized                      += OnSpriteScreenResized;
        }

        private void OnSpriteScreenResized(object sender, ResizedEventArgs e)
        {
            MoveAboveLogoutDialogAndApplyOffsetFromSettings();
        }

        private void MoveAboveLogoutDialogAndApplyOffsetFromSettings()
        {
            var logoutDialogTextCenter               = GetLogoutDialogTextCenter(GameService.Graphics.SpriteScreen.Size.X, GameService.Graphics.SpriteScreen.Size.Y);
            var containerCenterToTopLeftCornerOffset = new Point(Size.X / 2, Size.Y / 2);
            var offsetFromSettings                   = new Point(_settingService.ReminderWindowOffsetX, _settingService.ReminderWindowOffsetY);
            Location = logoutDialogTextCenter - containerCenterToTopLeftCornerOffset + offsetFromSettings;
        }

        private void UpdateText(string reminderText)
        {
            _reminderTextLabel.Text = reminderText;
            UpdateLabelAndIconLocations();
        }

        private void UpdateContainerSizeAndMoveAboveLogoutDialog(int size)
        {
            Size = new Point(670 * (5 + size) / 40, 75 * (5 + size) / 40);

            _reminderBackgroundImage.Size = Size;
            UpdateLabelAndIconLocations();
            MoveAboveLogoutDialogAndApplyOffsetFromSettings();
        }

        private void UpdateTextFontSize(int fontSizeIndex)
        {
            _reminderTextLabel.Font = FontService.Fonts[fontSizeIndex];
            UpdateLabelAndIconLocations();
        }

        private void UpdateIconSize(int iconSize)
        {
            _reminderIconImage.Size = new Point(iconSize * 13 / 10, iconSize);
            UpdateLabelAndIconLocations();
        }

        private void UpdateIconsVisibility(bool areVisible)
        {
            _reminderIconImage.Visible = areVisible;
            UpdateLabelAndIconLocations();
        }

        private void UpdateLabelAndIconLocations()
        {
            var labelLocationOffsetX = (Width - _reminderTextLabel.Width) / 2;
            var labelLocationOffsetY = (Height - _reminderTextLabel.Height) / 2;
            var iconOffsetY          = Height / 2 + _reminderTextLabel.Height / 2 - _reminderIconImage.Height + _settingService.ReminderIconOffsetY;
            var iconOffsetX          = (Width - _reminderIconImage.Width) / 2;

            _reminderTextLabel.Location = new Point(labelLocationOffsetX, labelLocationOffsetY);
            _reminderIconImage.Location = new Point(iconOffsetX, iconOffsetY);
        }

        private static Point GetLogoutDialogTextCenter(int screenWidth, int screenHeight)
        {
            var x = (int)(screenWidth * 0.5f);
            var y = (int)(screenHeight * (0.5f + RELATIVE_Y_OFFSET_FROM_SCREEN_CENTER));

            return new Point(x, y);
        }

        protected override void DisposeControl()
        {
            GameService.Graphics.SpriteScreen.Resized -= OnSpriteScreenResized;
            base.DisposeControl();
        }

        private readonly SettingService _settingService;
        private const float RELATIVE_Y_OFFSET_FROM_SCREEN_CENTER = 0.005f;
        private readonly Image _reminderBackgroundImage;
        private readonly Label _reminderTextLabel;
        private readonly Image _reminderIconImage;
    }
}