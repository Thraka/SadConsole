Thank you for trying out SadConsole. Please visit the documentation:
http://sadconsole.com/

Questions? Join the Discord channel
https://discord.gg/mttxqAs

Be sure to add a MonoGame 3.7 NuGet package of your choice.

Here is an example program.cs file that let's you get started with SadConsole quickly. If you're using Visual Basic, starter code is listed below the C# code.

using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyProject
{
    class Program
    {

        public const int Width = 80;
        public const int Height = 25;

        static void Main(string[] args)
        {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(Width, Height);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;
                        
            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            // Any startup code for your game. We will use an example console for now
            var startingConsole = SadConsole.Global.CurrentScreen;
            startingConsole.FillWithRandomGarbage();
            startingConsole.Fill(new Rectangle(3, 3, 27, 5), null, Color.Black, 0, SpriteEffects.None);
            startingConsole.Print(6, 5, "Hello from SadConsole", ColorAnsi.CyanBright);
        }
    }
}


Example program.vb:

Imports Console = SadConsole.Console
Imports Microsoft.Xna.Framework

Module Module1

    Sub Main()

        ' Setup the engine And create the main window.
        SadConsole.Game.Create(80, 25)

        ' Hook the start event so we can add consoles to the system.
        SadConsole.Game.OnInitialize = AddressOf Init

        ' Start the game.
        SadConsole.Game.Instance.Run()
        SadConsole.Game.Instance.Dispose()

    End Sub

    Sub Init()

        Dim Console = SadConsole.Global.CurrentScreen

        Console.FillWithRandomGarbage()
        Console.Fill(New Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 0, 0)
        Console.Print(4, 4, "Hello from SadConsole")

    End Sub

End Module