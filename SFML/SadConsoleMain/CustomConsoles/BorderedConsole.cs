namespace StarterProject.CustomConsoles
{
    using System;
    using SadConsole;
    using SadConsole.Consoles;
    using Console = SadConsole.Consoles.Console;
    using SFML.System;
    using SFML.Graphics;

    class BorderedConsole: Console
    {
        public BorderedConsole()
            : base(80, 25)
        {
            this.IsVisible = false;

            // Get the default box shape definition. Defines which characters to use for the box.
            SadConsole.Shapes.Box box = SadConsole.Shapes.Box.GetDefaultBox();

            // Customize the box
            box.Foreground = Color.Blue;
            box.BorderBackground = Color.White;
            box.FillColor = Color.White;
            box.Fill = true;
            box.Width = textSurface.Width;
            box.Height = textSurface.Height;
            
            // Draw the box shape onto the CellSurface that this console is displaying.
            box.Draw(this);

            this.Print(3, 1, "Shapes are easily created with only a few lines of code");

            // Get a circle
            SadConsole.Shapes.Circle circle = new SadConsole.Shapes.Circle();
            circle.BorderAppearance = new CellAppearance(Color.Yellow, Color.White, 57);
            circle.Center = new Vector2i(60, 13);
            circle.Radius = 10;

            circle.Draw(this);

            // Now time to make a line
            SadConsole.Shapes.Line line = new SadConsole.Shapes.Line();
            line.StartingLocation = new Vector2i(10, 10);
            line.EndingLocation = new Vector2i(45, 18);
            line.UseEndingCell = false;
            line.UseStartingCell = false;
            line.CellAppearance = new Cell { Foreground = Color.Magenta, Background = Color.White, GlyphIndex = 88 };

            line.Draw(this);

        }

    }
}
