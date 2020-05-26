Welcome to SadConsole. This is a test release of the next version of SadConsole. Things have changed since the preivous version. 

Questions? Join the Discord channel
https://discord.gg/mttxqAs

You must add a host NuGet package to your project, either SadConsole.Host.SFML or SadConsole.Host.MonoGame.

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
            SadConsole.Game.OnStart = Init;
                        
            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            // Any startup code for your game. We will use an example console for now
            var startingConsole = GameHost.Instance.Screen;
            startingConsole.FillWithRandomGarbage();
            startingConsole.Fill(new Rectangle(3, 3, 27, 5), null, Color.Black, 0, Mirror.None);
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
        SadConsole.Game.OnStart = AddressOf Init

        ' Start the game.
        SadConsole.Game.Instance.Run()
        SadConsole.Game.Instance.Dispose()

    End Sub

    Sub Init()

        Dim Console = GameHost.Instance.Screen

        Console.FillWithRandomGarbage()
        Console.Fill(New Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 0, 0)
        Console.Print(4, 4, "Hello from SadConsole")

    End Sub

End Module
