namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using Microsoft.Xna.Framework;

    class BorderedConsole: Console
    {
        public BorderedConsole(int width, int height)
            : base(width, height)
        {
            this.IsVisible = false;

            // Get the default box shape definition. Defines which characters to use for the box.
            SadConsole.Shapes.Box box = SadConsole.Shapes.Box.GetDefaultBox();

            // Customize the box
            box.Foreground = Color.Blue;
            box.BorderBackground = Color.White;
            box.FillColor = Color.White;
            box.Fill = true;
            box.Width = width;
            box.Height = height;
            
            // Draw the box shape onto the CellSurface that this console is displaying.
            box.Draw(this.CellData);

            this.CellData.Print(3, 1, "Box drawn using the Shapes.Box class");
        }

    }
}
