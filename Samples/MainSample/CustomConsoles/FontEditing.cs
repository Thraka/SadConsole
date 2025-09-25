#if MONOGAME
using SadConsole;
using SadConsole.Quick;
using SadConsole.StringParser;
using SadRogue.Primitives;
using SadConsole.FontEditing;

namespace FeatureDemo.CustomConsoles
{
    internal class FontEditing : ScreenSurface
    {
        public FontEditing() : base(80, 23)
        {
            var sadFont = ((SadFont)Font).Clone(Font.Name + "2");
            Font = sadFont;

            //sadFont.AddRows(1);

            Surface.Print(1, 1, "Hello");

            Color[] cachedPixels = null;

            var pixels = sadFont.Edit_GetGlyph_Pixel((int)'H', ref cachedPixels);

            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i] == Color.Transparent)
                    pixels[i] = Color.Green;
            }

            sadFont.Edit_SetGlyph_Pixel((int)'e', pixels, false, ref cachedPixels);

            //sadFont.Edit_CopyGlyph_Texture((int)'P', (int)'e');
            sadFont.Edit_EraseGlyph_Pixel((int)'l', false, ref cachedPixels);

            sadFont.Image.SetPixels(cachedPixels);

            this.WithKeyboard((s, k) => {
                if (k.IsKeyPressed(SadConsole.Input.Keys.Space))
                    this.Font = SadConsole.Game.Instance.DefaultFont;

                return true;
            });
        }
    }
}
#endif
