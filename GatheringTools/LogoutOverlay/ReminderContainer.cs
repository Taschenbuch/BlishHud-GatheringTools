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

            _reminderBackgroundImage = new Image(textureService.ReminderBackgroundTexture) { Parent = this, Size = Size };

            _tool1Image = new Image(textureService.Tool1Texture) { Parent = this, ClipsBounds = false };
            _tool2Image = new Image(textureService.Tool2Texture) { Parent = this, ClipsBounds = false };
            _tool3Image = new Image(textureService.Tool3Texture) { Parent = this, ClipsBounds = false };

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
            UpdateIconsVisibility(settingService.ReminderIconsAreVisibleSettings.Value);
            UpdateContainerSizeAndMoveAboveLogoutDialog(settingService.ReminderWindowSizeSetting.Value);

            settingService.ReminderTextFontSizeIndexSetting.SettingChanged += (s, e) => UpdateTextFontSize(e.NewValue);
            settingService.ReminderTextSetting.SettingChanged              += (s, e) => UpdateText(e.NewValue);
            settingService.ReminderWindowSizeSetting.SettingChanged        += (s, e) => UpdateContainerSizeAndMoveAboveLogoutDialog(e.NewValue);
            settingService.ReminderIconSizeSetting.SettingChanged          += (s, e) => UpdateIconSize(e.NewValue);
            settingService.ReminderIconsAreVisibleSettings.SettingChanged  += (s, e) => UpdateIconsVisibility(e.NewValue);
            settingService.ReminderWindowOffsetXSetting.SettingChanged     += (s, e) => MoveAboveLogoutDialogAndApplyOffsetFromSettings();
            settingService.ReminderWindowOffsetYSetting.SettingChanged     += (s, e) => MoveAboveLogoutDialogAndApplyOffsetFromSettings();
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
            UpdateChildLocations();
        }

        private void UpdateContainerSizeAndMoveAboveLogoutDialog(int size)
        {
            Size = new Point(670 * (5 + size) / 40, 75 * (5 + size) / 40);

            _reminderBackgroundImage.Size = Size;
            UpdateChildLocations();
            MoveAboveLogoutDialogAndApplyOffsetFromSettings();
        }

        private void UpdateTextFontSize(int fontSizeIndex)
        {
            _reminderTextLabel.Font = FontService.Fonts[fontSizeIndex];
            UpdateChildLocations();
        }

        private void UpdateIconSize(int iconSize)
        {
            var size = new Point(iconSize, iconSize);
            _tool1Image.Size = size;
            _tool2Image.Size = size;
            _tool3Image.Size = size;
            UpdateChildLocations();
        }

        private void UpdateIconsVisibility(bool areVisible)
        {
            _tool1Image.Visible = areVisible;
            _tool2Image.Visible = areVisible;
            _tool3Image.Visible = areVisible;
            UpdateChildLocations();
        }

        private void UpdateChildLocations()
        {
            var labelAndToolWidth = _tool1Image.Visible
                ? _reminderTextLabel.Width + 3 * _tool1Image.Width
                : _reminderTextLabel.Width;

            var labelLocationOffsetX = (Width - labelAndToolWidth) / 2;
            var labelLocationOffsetY = (Height - _reminderTextLabel.Height) / 2;
            var toolOffsetY          = (Height - _tool1Image.Height) / 2;
            var tool1OffsetX         = labelLocationOffsetX + _reminderTextLabel.Width;
            var tool2OffsetX         = tool1OffsetX + _tool1Image.Width;
            var tool3OffsetX         = tool1OffsetX + 2 * _tool1Image.Width;

            _reminderTextLabel.Location = new Point(labelLocationOffsetX, labelLocationOffsetY);
            _tool1Image.Location        = new Point(tool1OffsetX, toolOffsetY);
            _tool2Image.Location        = new Point(tool2OffsetX, toolOffsetY);
            _tool3Image.Location        = new Point(tool3OffsetX, toolOffsetY);
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
        private readonly Image _tool1Image;
        private readonly Image _tool2Image;
        private readonly Image _tool3Image;
    }
}