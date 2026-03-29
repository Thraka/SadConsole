using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;

internal class DemoBinaryFont : IDemo
{
    public string Title => "Binary Fonts";

    public string Description => "This demo loads binary VGA BIOS font files and displays all 256 glyphs.\r\n\r\nSelect a font file from the list on the right, and change the display size with the combo box.";

    public string CodeFile => "DemoBinaryFont.cs";

    public IScreenSurface CreateDemoScreen() =>
        new BinaryFontViewer();

    public override string ToString() =>
        Title;
}

internal class BinaryFontViewer : ControlsConsole
{
    ScreenSurface _fontPreviewSurface;

    public BinaryFontViewer() : base(80, 25)
    {
        // Fill up the surface with each glyph so we can examine a font. 16x16.
        _fontPreviewSurface = new ScreenSurface(16, 16);
        int glyph = 0;
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                _fontPreviewSurface.SetGlyph(x, y, glyph);
                glyph++;
            }
        }

        Children.Add(_fontPreviewSurface);

        Surface.Print(Width - 21, 0, "Font Size");

        ComboBox fontSizes = new(10, 6, 5, ["One", "Two", "Four"])
        {
            Position = (Width - 11, 0)
        };
        fontSizes.SelectedItemChanged += (sender, args) =>
        {
            _fontPreviewSurface.FontSize = GetFontSize();
        };


        FileDirectoryListbox fonts = new(20, Height - 2)
        {
            Position = (Width - 21, 1),
            DrawBorder = true,
            CurrentFolder = "Res/Fonts/Binary",
            OnlyRootAndSubDirs = true,
            FileFilter = "*.*"
        };

        fonts.SelectedItemChanged += (sender, args) =>
        {
            if (fonts.SelectedItem is System.IO.FileInfo item)
            {
                try
                {
                    IFont? font = SadFont.ImportVGABiosFont(item.Name, Path.GetFullPath(item.ToString()));

                    if (font != null)
                    {
                        if (_fontPreviewSurface.Font is SadFont f && f.FilePath == "")
                            _fontPreviewSurface.Font.Dispose();

                        _fontPreviewSurface.Font = font;
                        _fontPreviewSurface.FontSize = GetFontSize();
                    }
                }
                catch (Exception) { }
            }
        };

        Controls.Add(fontSizes);
        Controls.Add(fonts);

        Point GetFontSize()
        {
            if (fontSizes.SelectedIndex == 0)
                return _fontPreviewSurface.Font.GetFontSize(IFont.Sizes.One);
            else if (fontSizes.SelectedIndex == 1)
                return _fontPreviewSurface.Font.GetFontSize(IFont.Sizes.Two);
            else if (fontSizes.SelectedIndex == 2)
                return _fontPreviewSurface.Font.GetFontSize(IFont.Sizes.Four);
            else
                return _fontPreviewSurface.Font.GetFontSize(IFont.Sizes.One);
        }
    }
}
