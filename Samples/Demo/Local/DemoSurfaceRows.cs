using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Examples;

internal class DemoSurfaceRows : IDemo
{
    public string Title => "Surface Rows Test";

    public string Description => "Something.";

    public string CodeFile => "DemoSurfaceRows.cs";

    public IScreenSurface CreateDemoScreen() =>
        new SurfaceRows();

    public override string ToString() =>
        Title;
}

#pragma warning disable SADCON7001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
internal class SurfaceRows : RowFontSurface
#pragma warning restore SADCON7001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
{
    public SurfaceRows() : base(80, 25)
    {
        Surface.Print(0, 0, "This is the first row of text!");
        Surface.Print(0, 1, "This is the second row of text! The font is normal here.");
        Surface.Print(0, 2, "Now this row uses a bigger font!", Color.Gray);
        Surface.Print(0, 3, "And now we're back to normal size text.");
        Surface.Print(0, 4, "This one is even bigger!", Color.Gray);
        Surface.Print(0, 5, "And we're back to normal again.");
        Surface.Print(0, 6, "Why not try a smaller one!?", Color.Gray);
        Surface.Print(0, 7, "You can set the font size by row");
        Surface.Print(0, 8, "You can even change fonts!", Color.Gray);
        Surface.Print(0, 9, "Currently this is all done by changing the font+size by row on the surface");

        RowFontSizes[2] = FontSize * 2;
        RowFontSizes[4] = FontSize * 3;
        RowFontSizes[6] = FontSize / 1.8;
        RowFonts[8] = GameHost.Instance.Fonts["DINOBYTE_12x16"];
        RowFontSizes[8] = RowFonts[8].GetFontSize(IFont.Sizes.Two);
    }
}
internal class BinaryFontLoader : ControlsConsole
{
    ScreenSurface fontPreviewSurface;

    public BinaryFontLoader() : base(80, 25)
    {
        // Fill up the surface with each glyph so we can examine a font. 16x16.
        fontPreviewSurface = new ScreenSurface(16, 16);
        int glyph = 0;
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                fontPreviewSurface.SetGlyph(x, y, glyph);
                glyph++;
            }
        }

        Children.Add(fontPreviewSurface);

        ComboBox fontSizes = new ComboBox(16, 16, 5, ["One", "Two", "Four"])
        {
            Position = (Width - 21, 0)
        };
        fontSizes.SelectedItemChanged += (sender, args) =>
        {
            fontPreviewSurface.FontSize = GetFontSize();
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
                IFont? font = SadFont.ImportVGABiosFont(item.Name, Path.GetFullPath(item.ToString()));

                if (font != null)
                {
                    if (fontPreviewSurface.Font is SadFont f && f.FilePath == "")
                        fontPreviewSurface.Font.Dispose();

                    fontPreviewSurface.Font = font;
                    fontPreviewSurface.FontSize = GetFontSize();
                }
            }
        };

        Controls.Add(fontSizes);
        Controls.Add(fonts);

        Point GetFontSize()
        {
            if (fontSizes.SelectedIndex == 0)
                return fontPreviewSurface.Font.GetFontSize(IFont.Sizes.One);
            else if (fontSizes.SelectedIndex == 1)
                return fontPreviewSurface.Font.GetFontSize(IFont.Sizes.Two);
            else if (fontSizes.SelectedIndex == 2)
                return fontPreviewSurface.Font.GetFontSize(IFont.Sizes.Four);
            else
                return fontPreviewSurface.Font.GetFontSize(IFont.Sizes.One);
        }
    }

}
