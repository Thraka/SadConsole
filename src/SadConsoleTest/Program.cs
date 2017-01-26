using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using Console = SadConsole.Console;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.StringParser;
using SadConsole.Surfaces;

namespace SadConsoleTest
{
    class Program
    {
        public static AnimatedSurface CreateStatic(int width, int height, int frames, double blankChance)
        {
            var animation = new AnimatedSurface("default", width, height);
            var editor = new SurfaceEditor(new BasicSurface(1, 1));

            for (int f = 0; f < frames; f++)
            {
                var frame = animation.CreateFrame();
                editor.TextSurface = frame;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int character = Global.Random.Next(48, 168);

                        if (Global.Random.NextDouble() <= blankChance)
                            character = 32;

                        editor.SetGlyph(x, y, character);
                        editor.SetForeground(x, y, Color.White * (float)(Global.Random.NextDouble() * (1.0d - 0.5d) + 0.5d));
                    }
                }

            }

            animation.AnimationDuration = 1;
            animation.Repeat = true;

            animation.Start();

            return animation;
        }

        static void Main(string[] args)
        {
            SadConsole.Console con = null;
            SadConsole.Settings.ResizeMode = Settings.WindowResizeOptions.Scale;
            Settings.AllowWindowResize = true;
            
            SadConsole.Game.Create("IBM.font", 80, 25);
            AnimatedSurface animation = null;

            SadConsole.Game.OnInitialize = () =>
            {

                con = new SadConsole.Console(20, 20);
                SadConsole.Global.ActiveScreen = con;
                con.FillWithRandomGarbage();

                var con2 = new SadConsole.Console(5, 5);
                con2.TextSurface = new SurfaceView(con.TextSurface, new Rectangle(5, 5, 5, 5));
                con2.Fill(new Rectangle(0, 0, 5, 5), null, Color.Yellow, 0, SpriteEffects.None);
                con.Position = new Point(30, 1);
                Global.ActiveScreen = new Screen();
                Global.ActiveScreen.Children.Add(con);
                Global.ActiveScreen.Children.Add(con2);
                con2.TextSurface = animation = CreateStatic(20, 20, 20, 0.5d);
                //var blink = new SadConsole.Effects.Blink();
                //con.SetEffect(2, 8, blink);
                //Global.ActiveScreen = new StringParsingConsole();
            };
            float r = 0;
            SadConsole.Game.OnUpdate = (time) => animation.Update();
            SadConsole.Game.OnDraw = (time) =>
            {
                r -= MathHelper.ToRadians(0.5f);
                if (r < 0.0f)
                    r += MathHelper.TwoPi;

                Matrix transform = Matrix.CreateScale(0.5f, 2.0f, 1f) * Matrix.CreateTranslation(200f, 0f, 0f) * Matrix.CreateRotationZ(r);

                SadConsole.Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, transform);
                SadConsole.Global.SpriteBatch.Draw(con.TextSurface.LastRenderResult, new Vector2(0,0));
                SadConsole.Global.SpriteBatch.End();


                //----SadConsole.Cell
                //SadConsole.Cursor
                //SadConsole.Screen
                //SadConsole.IScreen
                //----SadConsole.Font 
                //SadConsole.
                //SadConsole.Surface.Editor
                //SadConsole.Surface.IRenderable
                //----SadConsole.Surface.ISurface
                //SadConsole.Surface.BasicSurface
                //SadConsole.Surface.RenderableSurface
                //SadConsole.Surface.AnimatedSurface
                //SadConsole.Surface.LayeredSurface
                //SadConsole.
                //SadConsole.Surface.Basic
                //SadConsole.Surface.Animated
                //SadConsole.Surface.Layered
                //SadConsole.
                //SadConsole.Consoles.Basic
                //SadConsole.Consoles.ControlHost
                //SadConsole.Consoles.Window
                //SadConsole.
                //SadConsole.
                //SadConsole.
                //SadConsole.
                //SadConsole.
                //SadConsole.
            };

            SadConsole.Game.Instance.Run();
        }
    }

    class StringParsingConsole : Console
    {
        public StringParsingConsole() : base(80, 25)
        {
            textSurface.DefaultForeground = ColorAnsi.White;
            Clear();
            int c = 59;
            int r = 1;

            ColoredString.CustomProcessor = CustomParseCommand;


            Print(1, r, "[c:r f:ansibluebright][c:r b:ansiblue]String parsing supports...                                                    ");
            SetGlyph(0, r, 221, Color.Black, ColorAnsi.Blue);

            r = 2;
            Color a = Color.Green;
            Print(1, ++r, "[c:r f:DodgerBlue]1.[c:r f:White] Multi-colored [c:r f:yellow]str[c:r f:red]ings");
            r++;
            Print(1, ++r, "[c:r f:DodgerBlue]2.[c:r f:White] You color the backgrounds: [c:r f:black][c:r b:Purple] Purple [c:u] [c:r b:Cyan] Cyan ");
            r++;
            Print(1, ++r, "[c:r f:DodgerBlue]3.[c:r f:White] [c:g f:LimeGreen:Orange:9]Gradients are [c:g b:Black:Red:Yellow:Red:Black:15]easily supported");
            r++;
            Print(1, ++r, "[c:r f:DodgerBlue]4.[c:r f:White] You [c:b 3]can apply [c:b 5:0.17]blink effects.");
            r++;
            Print(1, ++r, "[c:r f:DodgerBlue]5.[c:r f:White] You set [c:r f:greenyellow:9]mirroring [c:m 1]on any [c:m 2]text [c:u][c:u]you want.");

            r += 2;
            Print(1, r, "[c:r f:ansibluebright][c:r b:ansiblue]Examples                                                                      ");
            SetGlyph(0, r, 221, Color.Black, ColorAnsi.Blue);

            r += 2;

            SadConsole.Shapes.Line line = new SadConsole.Shapes.Line();
            line.StartingLocation = new Point(c, r);
            line.EndingLocation = new Point(c, r + 5);
            line.UseEndingCell = false;
            line.UseStartingCell = false;
            line.Cell.Glyph = 179;
            line.Draw(this);

            Print(1, r, "Some `[c:r f:red]text`[c:u] to print                       ");
            Print(c + 2, r, $"Some [c:r f:{Color.Red.ToParser()}]text[c:u] to print");

            Print(1, ++r, "Some `[c:r f:100,100,33]text`[c:u] to print");
            Print(c + 2, r, "Some [c:r f:255,255,0]text[c:u] to print");

            Print(1, ++r, "Some `[c:r b:ansiblackbright]text to `[c:r f:white]print");
            Print(c + 2, r, "Some [c:r b:ansiblackbright]text to [c:r f:white]print");

            Print(1, ++r, "`[c:m 1]Some `[c:r f:purple]text to`[c:u] pr`[c:u]int");
            Print(c + 2, r, "[c:m 1]Some [c:r f:purple]text to[c:u] pr[c:u]int");

            Print(1, ++r, "[c:r f:default]`[c:g f:red:green:18]`[c:g b:green:red:18]Some text to print");
            Print(c + 2, r, "[c:g f:red:green:18][c:g b:green:red:18]Some text to print");
        }

        ParseCommandBase CustomParseCommand(string command, string parameters, ColoredGlyph[] glyphString,
                                                          ISurface surface, SurfaceEditor editor, ParseCommandStacks commandStacks)
        {
            switch (command)
            {
                case "t":
                    return new ParseCommandRetext(parameters);
                default:
                    return null; ;
            }
        }

        class ParseCommandRetext : ParseCommandBase
        {
            public int Counter;
            public char Glyph;

            public ParseCommandRetext(string parameters)
            {
                string[] parts = parameters.Split(new char[] { ':' }, 2);

                // Count and glyph type provided
                if (parts.Length == 2)
                    Counter = int.Parse(parts[1]);
                else
                    Counter = -1;

                // Get character
                Glyph = parts[0][0];

                // No exceptions, set the type
                CommandType = CommandTypes.Glyph;
            }

            public override void Build(ref ColoredGlyph glyphState, ColoredGlyph[] glyphString, int surfaceIndex, ISurface surface, SurfaceEditor editor, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
            {
                glyphState.Glyph = Glyph;

                if (Counter != -1)
                {
                    Counter--;

                    if (Counter == 0)
                        commandStack.RemoveSafe(this);
                }
            }
        }
    }

}
