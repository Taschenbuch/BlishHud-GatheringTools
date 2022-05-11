using Blish_HUD;
using Blish_HUD.Controls;
using GatheringTools.Services;
using GatheringTools.ToolSearch.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GatheringTools.LogoutOverlay
{
    public class ReminderContainer : Container
    {
        public ReminderContainer(TextureService textureService, SettingService settingService)
        {
            Parent          = GameService.Graphics.SpriteScreen;
            _textureService = textureService;
            _settingService = settingService;

            _reminderBackgroundImage = new Image(textureService.ReminderBackgroundTexture)
            {
                Size   = Size,
                Parent = this
            };

            var reminderImageTexture
                = CreateReminderImageTexture(settingService.ReminderImageOutlineSetting.Value, textureService);

            _reminderImage = new Image(reminderImageTexture)
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
            UpdateImageSize(settingService.ReminderImageSizeSetting.Value);
            UpdateImageVisibility(settingService.ReminderImageIsVisibleSetting.Value);
            UpdateContainerSizeAndMoveAboveLogoutDialog(settingService.ReminderWindowSizeSetting.Value);
            UpdateBackgroundVisibility(settingService.ReminderBackgroundIsVisibleSetting.Value);

            settingService.ReminderTextFontSizeIndexSetting.SettingChanged   += (s, e) => UpdateTextFontSize(e.NewValue);
            settingService.ReminderTextSetting.SettingChanged                += (s, e) => UpdateText(e.NewValue);
            settingService.ReminderWindowSizeSetting.SettingChanged          += (s, e) => UpdateContainerSizeAndMoveAboveLogoutDialog(e.NewValue);
            settingService.ReminderImageSizeSetting.SettingChanged           += (s, e) => UpdateImageSize(e.NewValue);
            settingService.ReminderImageOutlineSetting.SettingChanged        += (s, e) => _reminderImage.Texture = CreateReminderImageTexture(e.NewValue, _textureService);
            settingService.ReminderImageIsVisibleSetting.SettingChanged      += (s, e) => UpdateImageVisibility(e.NewValue);
            settingService.ReminderBackgroundIsVisibleSetting.SettingChanged += (s, e) => UpdateBackgroundVisibility(e.NewValue);
            settingService.ReminderWindowOffsetXSetting.SettingChanged       += (s, e) => MoveAboveLogoutDialogAndApplyOffsetFromSettings();
            settingService.ReminderWindowOffsetYSetting.SettingChanged       += (s, e) => MoveAboveLogoutDialogAndApplyOffsetFromSettings();
            settingService.ReminderImageOffsetXSetting.SettingChanged        += (s, e) => UpdateLabelAndImageLocations();
            settingService.ReminderImageOffsetYSetting.SettingChanged        += (s, e) => UpdateLabelAndImageLocations();
            GameService.Graphics.SpriteScreen.Resized                        += OnSpriteScreenResized;
        }

        private static Texture2D CreateReminderImageTexture(Outline outline, TextureService textureService)
        {
            switch (outline)
            {
                case Outline.None:
                    return textureService.ReminderImageNoOutlineTexture;
                case Outline.Small:
                    return textureService.ReminderImageSmallOutlineTexture;
                case Outline.Big:
                    return textureService.ReminderImageBigOutlineTexture;
                default:
                    return textureService.ReminderImageNoOutlineTexture;
            }
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
            UpdateLabelAndImageLocations();
        }

        private void UpdateContainerSizeAndMoveAboveLogoutDialog(int size)
        {
            Size = new Point(670 * (5 + size) / 40, 75 * (5 + size) / 40);

            _reminderBackgroundImage.Size = Size;
            UpdateLabelAndImageLocations();
            MoveAboveLogoutDialogAndApplyOffsetFromSettings();
        }

        private void UpdateTextFontSize(int fontSizeIndex)
        {
            _reminderTextLabel.Font = FontService.Fonts[fontSizeIndex];
            UpdateLabelAndImageLocations();
        }

        private void UpdateImageSize(int imageSize)
        {
            _reminderImage.Size = new Point(imageSize, imageSize);
            //_reminderImage.Size = new Point(imageSize * 13 / 10, imageSize);
            UpdateLabelAndImageLocations();
        }

        private void UpdateImageVisibility(bool isVisible) => _reminderImage.Visible = isVisible;
        private void UpdateBackgroundVisibility(bool isVisible) => _reminderBackgroundImage.Visible = isVisible;

        private void UpdateLabelAndImageLocations()
        {
            var labelLocationOffsetX = (Width - _reminderTextLabel.Width) / 2;
            var labelLocationOffsetY = (Height - _reminderTextLabel.Height) / 2;
            var imageOffsetY         = Height / 2 + _reminderTextLabel.Height / 2 - _reminderImage.Height + _settingService.ReminderImageOffsetY;
            var imageOffsetX         = (Width - _reminderImage.Width) / 2 + _settingService.ReminderImageOffsetX;

            _reminderTextLabel.Location = new Point(labelLocationOffsetX, labelLocationOffsetY);
            _reminderImage.Location     = new Point(imageOffsetX, imageOffsetY);
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

        private readonly TextureService _textureService;
        private readonly SettingService _settingService;
        private const float RELATIVE_Y_OFFSET_FROM_SCREEN_CENTER = 0.005f;
        private readonly Image _reminderBackgroundImage;
        private readonly Label _reminderTextLabel;
        private readonly Image _reminderImage;
    }
}