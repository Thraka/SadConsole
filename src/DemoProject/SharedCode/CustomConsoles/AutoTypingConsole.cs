using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SadConsole.Surfaces;
using System;
using Console = SadConsole.Console;
using SadConsole.Input;

namespace StarterProject.CustomConsoles
{

    // Using a ConsoleList which lets us group multiple consoles 
    // into a single processing entity
    class AutoTypingConsole : Console, IConsoleMetadata
    {
        SadConsole.Instructions.DrawString typingInstruction;

        public ConsoleMetadata Metadata
        {
            get
            {
                return new ConsoleMetadata() { Title = "SadConsole.Instructions", Summary = "Automatic typing to a console." };
            }
        }

        public AutoTypingConsole(): base(80, 23)
        {

            string[] text = new string[] { 
                                           "[c:r f:ansiwhite]Welcome to the [c:g f:black:red:black:10][c:r b:White]SadConsole[c:u] starter project!",
                                           "",
                                           "Not only does the app demonstrate some of the things you can do with ",
                                           "[c:g f:black:red:black:10][c:r b:White]SadConsole[c:u], but it also provides code examples.",
                                           "",
                                           "To use this app, press [c:r f:red:2]F1 to to cycle to the next console. Here is a list of",
                                           "examples you will cycle through:",
                                           "",
                                           "[c:r f:LightGreen]1[c:r f:green])[c:u 2] Splash Screen     -- [c:r f:white]Sample splash screen advertising [c:g f:black:red:black:10][c:r b:White]SadConsole[c:u 2]",
                                           "[c:r f:LightGreen]2[c:r f:green])[c:u 2] Stretched Console -- [c:r f:white]Shows how to print with a font that x2 in size[c:u]",
                                           "[c:r f:LightGreen]3[c:r f:green])[c:u 2] String Parser     -- [c:r f:white]Examples of using the string parser functions[c:u]",
                                           "[c:r f:LightGreen]4[c:r f:green])[c:u 2] Controls test     -- [c:r f:white]Displays all SadConsole.Controls for testing[c:u]",
                                           "[c:r f:LightGreen]5[c:r f:green])[c:u 2] DOS Prompt        -- [c:r f:white]A fake dos prompt example[c:u]",
                                           "[c:r f:LightGreen]6[c:r f:green])[c:u 2] Views & Sub Views -- [c:r f:white]Display two consoles both a view into the same data[c:u]",
                                           "[c:r f:LightGreen]7[c:r f:green])[c:u 2] Ansi console      -- [c:r f:white]Uses SadConsole.Ansi to parse an ansi.sys art file[c:u]",
                                         };

            // We want this to print on a sub region of the main console, so we'll create a sub view and use that
            typingInstruction = new SadConsole.Instructions.DrawString(
                                                                       new SurfaceEditor(
                                                                            new SurfaceView(this.TextSurface, new Rectangle(2, 1, 76, 21)) { IsDirty = false }
                                                                       ));

            typingInstruction.Text = SadConsole.ColoredString.Parse(string.Join("\r\n", text));
            typingInstruction.TotalTimeToPrint = 8; // 0.5 seconds per line of text

            //VirtualCursor.IsVisible = true;
            IsVisible = false;
        }

        public override void Update(TimeSpan elapsed)
        {
            if (IsVisible)
            {
                if (!typingInstruction.IsFinished)
                    typingInstruction.Run();

                base.Update(elapsed);
            }
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            return true;
        }

    }
}
