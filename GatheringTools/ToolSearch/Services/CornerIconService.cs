using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using GatheringTools.ToolSearch.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace GatheringTools.ToolSearch.Services
{
    public class CornerIconService
    {
        public CornerIconService(SettingEntry<bool> showToolSearchCornerIconSetting,
                                 ToolSearchStandardWindow toolSearchStandardWindow,
                                 TextureService textureService)
        {
            _toolSearchStandardWindow = toolSearchStandardWindow;
            _cornerIconTexture        = textureService.CornerIconTexture;
            _hoverCornerIconTexture   = textureService.HoverCornerIconTexture;

            if (showToolSearchCornerIconSetting.Value)
                CreateCornerIcon();

            showToolSearchCornerIconSetting.SettingChanged += (s, e) =>
            {
                if (e.NewValue)
                    CreateCornerIcon();
                else
                    RemoveCornerIcon();
            };
        }

        public void RemoveCornerIcon()
        {
            _toolSearchCornerIcon.Click -= OnToolSearchCornerIconClick;
            _toolSearchCornerIcon?.Dispose();
            _toolSearchCornerIcon = null;
        }

        private void CreateCornerIcon()
        {
            _toolSearchCornerIcon = new CornerIcon
            {
                Icon             = _cornerIconTexture,
                HoverIcon        = _hoverCornerIconTexture,
                BasicTooltipText = "Click to show/hide which character has gathering tools equipped.\nIcon can be hidden by module settings.",
                Parent           = GameService.Graphics.SpriteScreen,
            };

            _toolSearchCornerIcon.Click += OnToolSearchCornerIconClick;
        }

        private async void OnToolSearchCornerIconClick(object sender, Blish_HUD.Input.MouseEventArgs e)
        {
            await _toolSearchStandardWindow.ToggleVisibility();
        }

        private readonly ToolSearchStandardWindow _toolSearchStandardWindow;
        private readonly Texture2D _cornerIconTexture;
        private readonly Texture2D _hoverCornerIconTexture;
        private CornerIcon _toolSearchCornerIcon;
    }
}