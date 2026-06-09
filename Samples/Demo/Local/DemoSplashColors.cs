namespace SadConsole.Examples;

internal class DemoSplashColors : IDemo
{
    public string Title => "Splash Colors Grid";

    public string Description => "Displays color using the Splash color system. The Splash color system uses a 3 digit systen: '[c:r f:Yellow]000[c:u]'" +
                                 " with each digit being 0-9. Alpha channel support can be used as fourth digit, but isn't required.\r\n\n" +
                                 "Examples:\r\n" +
                                 "[c:r b:999][c:r f:000][999][c:u 2][c:r b:000][c:r f:999][000][c:u 2][c:r b:900][c:r f:999][900][c:u 2][c:r b:090][c:r f:999][090][c:u 2][c:r b:009][c:r f:999][009][c:u 2][c:r b:990][c:r f:000][990][c:u 2][c:r b:909][c:r f:000][909][c:u 2][c:r b:099][c:r f:000][099][c:u 2]\r\n" +
                                 "[c:r b:357][c:r f:999][357][c:u 2][c:r b:680][c:r f:000][680][c:u 2][c:r b:803][c:r f:000][803][c:u 2][c:r b:975][c:r f:000][975][c:u 2][c:r b:340][c:r f:999][340][c:u 2][c:r b:274][c:r f:999][274][c:u 2][c:r b:618][c:r f:000][618][c:u 2][c:r b:531][c:r f:999][531][c:u 2]\r\n";

    public string CodeFile => "DemoSplashColors.cs";

    public IScreenSurface CreateDemoScreen() =>
        new DemoSplashColorsSurface();

    public override string ToString() =>
        Title;
}

internal class DemoSplashColorsSurface : ScreenSurface
{
    private const int Columns = 10; // R values 0-9, one per column
    private const int CellWidth = 8; // 80 / 10, fills the surface exactly

    public DemoSplashColorsSurface() : base(80, 25)
    {
        UseKeyboard = false;
        UseMouse = false;

        int rows = Surface.Height;

        for (int row = 0; row < rows; row++)
        {
            // Step the (G,B) pair evenly from 00 (top) to 99 (bottom).
            int gbIndex = (int)Math.Round((double)row * 99 / (rows - 1));
            int g = gbIndex / 10;
            int b = gbIndex % 10;

            for (int r = 0; r < Columns; r++)
            {
                Color background = Color.Black.FromSplash(r, g, b);
                Color foreground = background.GetLuma() >= 128f ? Color.Black : Color.White;

                // " RGB " with extra padding so the colored block fills the cell exactly.
                Surface.Print(r * CellWidth, row, $"  {r}{g}{b}   ", foreground, background);
            }
        }
    }
}
