using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using Microsoft.Xna.Framework;

namespace SadRogueSharp.Consoles
{
    class Status: SadConsole.Consoles.Console
    {
        public Status(): base(30, 30)
        {
            // Setup some fake status
            CellData.Print(1, 0, "SadConsole + RogueSharp".Align(System.Windows.HorizontalAlignment.Center, 29).CreateGradient(Color.Moccasin, Color.Gray, null));

            CellData.Print(2, 2, "Health", Color.GreenYellow);
            CellData.Print(10, 2, new string((char)176, 19).CreateGradient(Color.Red, Color.GreenYellow, null));

            CellData.Print(2, 4, "Energy", Color.GreenYellow);
            CellData.Print(10, 4, new string((char)176, 19).CreateGradient(Color.Blue, Color.GreenYellow, null));


            // Draw a line down the side
            SadConsole.Shapes.Line line = new SadConsole.Shapes.Line();
            line.StartingLocation = new Point(0, 0);
            line.EndingLocation = new Point(0, 29);
            new CellAppearance(Color.LightGray, Color.Black, 186, Microsoft.Xna.Framework.Graphics.SpriteEffects.None).CopyAppearanceTo(line.CellAppearance);
            line.UseEndingCell = false;
            line.UseStartingCell = false;
            line.Draw(this.CellData);


            // Position the console to the right of the map view
            Position = new Microsoft.Xna.Framework.Point(30, 0);
        }
    }
}
