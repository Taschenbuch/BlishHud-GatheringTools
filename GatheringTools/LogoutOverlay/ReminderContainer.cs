using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GatheringTools.LogoutOverlay
{
    public class ReminderContainer : Container
    {
        public ReminderContainer(ContentsManager contentsManager)
        {
            Parent = GameService.Graphics.SpriteScreen;

            _reminderBackgroundTexture = contentsManager.GetTexture("logoutDialogTextArea.png");
            _tool1Texture              = contentsManager.GetTexture("1998933.png");
            _tool2Texture              = contentsManager.GetTexture("1998934.png");
            _tool3Texture              = contentsManager.GetTexture("1998935.png");

            _reminderBackgroundImage = new Image(_reminderBackgroundTexture) { Parent = this, Size = Size };

            _tool1Image = new Image(_tool1Texture) { Parent = this, ClipsBounds = false };
            _tool2Image = new Image(_tool2Texture) { Parent = this, ClipsBounds = false };
            _tool3Image = new Image(_tool3Texture) { Parent = this, ClipsBounds = false };

            _reminderTextLabel = new Label() 
            {
                TextColor      = Color.Red,
                ShowShadow     = true,
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
                ClipsBounds    = false,
                Parent         = this
            };

            Resized += (s, e) => _reminderBackgroundImage.Size = Size;
        }

        protected override void DisposeControl()
        {
            _tool1Texture?.Dispose();
            _tool3Texture?.Dispose();
            _tool2Texture?.Dispose();
            _reminderBackgroundTexture?.Dispose();

            base.DisposeControl();
        }

        public void MoveAboveLogoutDialog()
        {
            var logoutDialogTextCenter               = GetLogoutDialogTextCenter(GameService.Graphics.SpriteScreen.Size.X, GameService.Graphics.SpriteScreen.Size.Y);
            var containerCenterToTopLeftCornerOffset = new Point(Size.X / 2, Size.Y / 2);
            Location = logoutDialogTextCenter - containerCenterToTopLeftCornerOffset;
        }

        public void UpdateReminderText(string reminderText)
        {
            _reminderTextLabel.Text = reminderText;
            UpdateChildLocations();
        }

        public void UpdateContainerSizeAndMoveAboveLogoutDialog(int size)
        {
            Size = new Point(670 * (5 + size) / 40, 75 * (5 + size) / 40);
            UpdateChildLocations();
            MoveAboveLogoutDialog();
        }

        public void UpdateReminderTextFontSize(int fontSizeIndex)
        {
            _reminderTextLabel.Font = FontService.Fonts[fontSizeIndex];
            UpdateChildLocations();
        }

        public void UpdateIconSize(int iconSize)
        {
            var size = new Point(iconSize, iconSize);
            _tool1Image.Size = size;
            _tool2Image.Size = size;
            _tool3Image.Size = size;
            UpdateChildLocations();
        }

        private void UpdateChildLocations()
        {
            var labelAndToolWidth = _reminderTextLabel.Width + 3 * _tool1Image.Width;

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

        public static Point GetLogoutDialogTextCenter(int screenWidth, int screenHeight)
        {
            var x = (int)(screenWidth * 0.5f);
            var y = (int)(screenHeight * (0.5f + RELATIVE_Y_OFFSET_FROM_SCREEN_CENTER));

            return new Point(x, y);
        }


        private const float RELATIVE_Y_OFFSET_FROM_SCREEN_CENTER = 0.005f;
        private readonly Image _reminderBackgroundImage;
        private readonly Texture2D _reminderBackgroundTexture;
        private readonly Texture2D _tool1Texture;
        private readonly Texture2D _tool2Texture;
        private readonly Texture2D _tool3Texture;
        private readonly Label _reminderTextLabel;
        private readonly Image _tool1Image;
        private readonly Image _tool2Image;
        private readonly Image _tool3Image;
    }
}