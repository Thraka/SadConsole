using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;
using SadConsole.Input;
using FeatureDemo.CustomConsoles;
using Console = SadConsole.Console;
using SadRogue.Primitives;

namespace FeatureDemo
{
    [System.Diagnostics.DebuggerDisplay("Demo Container")]
    class Container : ScreenObject
    {
        private int currentConsoleIndex = -1;
        private IScreenSurface selectedConsole;
        private HeaderConsole headerConsole;

        CustomConsole[] consoles;

        public Container()
        {
            headerConsole = new HeaderConsole();

            //var console1 = new Console(10, 10, Serializer.Load<FontMaster>("Fonts/Cheepicus12.font").GetFont(Font.FontSizes.Two));
            //console1.Fill(Color.BlueViolet, Color.Yellow, 7);
            //var consoleReal = new StretchedConsole();
            //consoleReal.TextSurface = console1.TextSurface;

            consoles = new CustomConsole[] {
                //consoleReal,
                
                //new CustomConsole(new CustomConsoles.HexSurface(80 / 2, 23 / 2) { FontSize = SadConsole.GameHost.Instance.DefaultFont.GetFontSize(Font.Sizes.Two) }, "Hex surface", "Using a custom renderer and custom mouse logic to draw hex cells"),
                new CustomConsole(new CustomConsoles.SerializationTests(), "Serialization Tests", "Test serializing various types from SadConsole"),
                new CustomConsole(new CustomConsoles.AutoTypingConsole(), "Auto Typing", "Automatic typing to a console"),
                new CustomConsole(new CustomConsoles.SplashScreen() { SplashCompleted = MoveNextConsole }, "Splash Screen - Using instructions", "Chains multiple SadConsole.Instruction types to create an animation."),

                new CustomConsole(new CustomConsoles.ControlsTest(), "Controls Test", "Interact with SadConsole controls"),
                new CustomConsole(new CustomConsoles.DOSConsole(), "Prompt Console", "Emulates a command prompt"),
                new CustomConsole(new CustomConsoles.MultiCursor(), "Multiple Cursors", "Consoles can have multiple cursors. Press F3 to change the active cursor."),
                new CustomConsole(new CustomConsoles.BorderedConsole(), "Border Component", "A component that draws a border around a console"),
                new CustomConsole(new CustomConsoles.AnsiConsole(), "Ansi parsing", "Read in old DOS ANSI files."),
                new CustomConsole(new CustomConsoles.StringParsingConsole(), "String Parser", "Examples of using the string parser"),
                new CustomConsole(new CustomConsoles.ScrollableConsole(20, 10, 60), "Text scrolling", "Renders a tiny console with a cursor along with a scroll bar"),
                new CustomConsole(new CustomConsoles.ShapesConsole(), "Shape Drawing & Text Mouse Cursor", "Examples of drawing shapes and displaying a mouse cursor"),
                new CustomConsole(new CustomConsoles.StretchedConsole(), "Font Zoom", "Console where font has been zoomed x2"),
                new CustomConsole(new CustomConsoles.EntityConsole(), "Game object", "Use the cursor keys to move the little character"),
                new CustomConsole(new CustomConsoles.RandomScrollingConsole(), "Scrolling", "2000x2000 scrollable console. Use the cursor keys."),
                
                //new CustomConsoles.AutoTypingConsole(),
                //new CustomConsole(new CustomConsoles.MouseRenderingDebug(), "SadConsole.Instructions", "Automatic typing to a console."),
                ////new CustomConsole(new CustomConsoles.WorldGenerationConsole(), "Random world generator", "Generates a random world, displaying it at half-font size."),
                //new CustomConsole(new CustomConsoles.SubConsoleCursor(), "Subconsole Cursor", "Two consoles with a single backing TextSurface"),
                //new CustomConsole(new CustomConsoles.ViewsAndSubViews(), "Sub Views", "Single text surface with two views into it. Click on either view."),
                //new CustomConsole(new CustomConsoles.TextCursorConsole(), "Text Mouse Cursor", "Draws a game object where ever the mouse cursor is."),
                //new CustomConsole(new CustomConsoles.SceneProjectionConsole(), "Scene Projection", "Translating a 3D scene to a TextSurface T=Toggle B=Block Mode"),
            };

            MoveNextConsole();
        }

        public void MoveNextConsole()
        {
            currentConsoleIndex++;

            if (currentConsoleIndex >= consoles.Length)
                currentConsoleIndex = 0;

            selectedConsole = consoles[currentConsoleIndex].Console;

            Children.Clear();
            Children.Add(selectedConsole);
            Children.Add(headerConsole);

            selectedConsole.IsVisible = true;
            selectedConsole.IsFocused = true;
            selectedConsole.Position = new Point(0, 2);

            GameHost.Instance.FocusedScreenObjects.Set(selectedConsole);
            headerConsole.SetConsole(consoles[currentConsoleIndex].Title, consoles[currentConsoleIndex].Summary);
        }
        
        //public override bool ProcessKeyboard(Keyboard state)
        //{
        //    return selectedConsole.ProcessKeyboard(state);
        //}

        //public override bool ProcessMouse(MouseConsoleState state)
        //{
        //    return selectedConsole.ProcessMouse(state);
        //}
    }
}
