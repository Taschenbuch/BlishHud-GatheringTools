using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework.Graphics;

namespace GatheringTools.ToolSearch
{
    public class CornerIconService
    {
        public CornerIconService(SettingEntry<bool> showToolSearchCornerIconSetting,
                                 ToolSearchStandardWindow toolSearchStandardWindow,
                                 Texture2D cornerIconTexture)
        {
            _toolSearchStandardWindow = toolSearchStandardWindow;
            _cornerIconTexture        = cornerIconTexture;

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
            _toolSearchCornerIcon?.Dispose();
            _toolSearchCornerIcon = null;
        }

        private void CreateCornerIcon()
        {
            _toolSearchCornerIcon = new CornerIcon
            {
                Icon             = _cornerIconTexture,
                BasicTooltipText = "Click to show/hide gathering tool search window. Can be hidden in module settings.",
                Parent           = GameService.Graphics.SpriteScreen,
            };

            _toolSearchCornerIcon.Click += (s, e) => _toolSearchStandardWindow.ToggleVisibility();
        }

        private readonly ToolSearchStandardWindow _toolSearchStandardWindow;
        private readonly Texture2D _cornerIconTexture;
        private CornerIcon _toolSearchCornerIcon;
    }
}