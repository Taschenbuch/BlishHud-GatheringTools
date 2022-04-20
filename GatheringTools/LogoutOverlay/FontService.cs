using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Settings;
using MonoGame.Extended.BitmapFonts;

namespace GatheringTools.LogoutOverlay
{
    public class FontService
    {
        public static SettingEntry<int> CreateFontSizeIndexSetting(SettingCollection settings)
        {
            var fontSizeIndex = settings.DefineSetting(
                "font size index (logout overlay)",
                Fonts.Count - 1,
                () => "font size",
                () => "Change font size of the reminder text");

            fontSizeIndex.SetRange(0, Fonts.Count - 1);

            return fontSizeIndex;
        }

        public static readonly List<BitmapFont> Fonts = new List<BitmapFont>
        {
            GetFont(ContentService.FontSize.Size11),
            GetFont(ContentService.FontSize.Size12),
            GetFont(ContentService.FontSize.Size14),
            GetFont(ContentService.FontSize.Size16),
            GetFont(ContentService.FontSize.Size18),
            GetFont(ContentService.FontSize.Size20),
            GetFont(ContentService.FontSize.Size22),
            GetFont(ContentService.FontSize.Size24),
            GetFont(ContentService.FontSize.Size32),
            GetFont(ContentService.FontSize.Size34),
            GetFont(ContentService.FontSize.Size36),
        };

        private static BitmapFont GetFont(ContentService.FontSize fontSize)
        {
            return GameService.Content.GetFont(ContentService.FontFace.Menomonia, fontSize, ContentService.FontStyle.Regular);
        }
    }
}