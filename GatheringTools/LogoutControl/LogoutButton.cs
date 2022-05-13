using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Controls.Extern;
using Blish_HUD.Input;
using GatheringTools.Settings;
using GatheringTools.ToolSearch.Services;
using Microsoft.Xna.Framework;

namespace GatheringTools.LogoutControl
{
    public class LogoutButton : GlowButton
    {
        public LogoutButton(SettingService settingService, TextureService textureService)
        {
            _settingService = settingService;

            Icon       = textureService.LogoutButtonTexture;
            ActiveIcon = textureService.LogoutButtonActiveTexture;
            Location   = new Point(settingService.LogoutButtonPositionXSetting.Value, settingService.LogoutButtonPositionYSetting.Value);
            Size       = CreateImageSize(settingService.LogoutButtonSizeSetting.Value);
            Visible    = settingService.LogoutButtonIsVisible.Value;
            Parent     = GameService.Graphics.SpriteScreen;

            settingService.LogoutButtonIsVisible.SettingChanged        += (s, e) => Visible  = e.NewValue;
            settingService.LogoutButtonSizeSetting.SettingChanged      += (s, e) => Size     = CreateImageSize(e.NewValue);
            settingService.LogoutButtonPositionXSetting.SettingChanged += (s, e) => Location = new Point(e.NewValue, Location.Y);
            settingService.LogoutButtonPositionYSetting.SettingChanged += (s, e) => Location = new Point(Location.X, e.NewValue);

            Click += (s, o) => Blish_HUD.Controls.Intern.Keyboard.Stroke((VirtualKeyShort) _settingService.LogoutKeyBindingSetting.Value.PrimaryKey);

            GameService.Input.Mouse.LeftMouseButtonReleased += OnLeftMouseButtonReleased;
        }

        public void ShowOrHide()
        {
            var shouldBeVisible = ShouldBeVisible();

            if (Visible == false && shouldBeVisible)
                Show();
            else if (Visible && shouldBeVisible == false)
                Hide();
        }

        private bool ShouldBeVisible()
        {
            var hideEverywhere              = _settingService.LogoutButtonIsVisible.Value == false;
            var showOnMap                   = _settingService.LogoutButtonIsVisibleOnWorldMap.Value;
            var showOnCharSelectAndCutScene = _settingService.LogoutButtonIsVisibleOnCutScenesAndCharacterSelect.Value;
            var isInGame                    = GameService.GameIntegration.Gw2Instance.IsInGame;
            var mapIsClosed                 = GameService.Gw2Mumble.UI.IsMapOpen == false;

            if (hideEverywhere)
                return false;

            if (showOnMap && showOnCharSelectAndCutScene)
                return true;

            if (showOnCharSelectAndCutScene && showOnMap == false && mapIsClosed)
                return true;

            if (showOnCharSelectAndCutScene == false && showOnMap && isInGame)
                return true;

            if (showOnCharSelectAndCutScene == false && showOnMap == false && isInGame && mapIsClosed)
                return true;

            return false;
        }

        protected override void DisposeControl()
        {
            GameService.Input.Mouse.LeftMouseButtonReleased -= OnLeftMouseButtonReleased;
            base.DisposeControl();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (_isDraggedByMouse && _settingService.LogoutButtonDragWithMouseIsEnabledSetting.Value)
            {
                // done via settings instead of directly updating location
                // because otherwise the reset position button would stop working.
                _settingService.LogoutButtonPositionXSetting.Value = Input.Mouse.Position.X - _mousePressedLocationInsideControl.X;
                _settingService.LogoutButtonPositionYSetting.Value = Input.Mouse.Position.Y - _mousePressedLocationInsideControl.Y;
            }
        }

        protected override void OnLeftMouseButtonPressed(MouseEventArgs e)
        {
            if (_settingService.LogoutButtonDragWithMouseIsEnabledSetting.Value)
            {
                _isDraggedByMouse                  = true;
                _mousePressedLocationInsideControl = Input.Mouse.Position - Location;
            }

            base.OnLeftMouseButtonPressed(e);
        }

        // not using the override on purpose because it does not register the release when clicking fast (workaround suggested by freesnow)
        private void OnLeftMouseButtonReleased(object sender, MouseEventArgs e)
        {
            if (_settingService.LogoutButtonDragWithMouseIsEnabledSetting.Value)
                _isDraggedByMouse = false;
        }

        private static Point CreateImageSize(int width)
        {
            return new Point(width, width * 65 / 60);
        }

        private readonly SettingService _settingService;
        private bool _isDraggedByMouse;
        private Point _mousePressedLocationInsideControl = Point.Zero;
    }
}