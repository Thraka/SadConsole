Welcome to SadConsole.

Questions? Join the Discord channel
https://discord.gg/pAFNKYjczM

You must add a host NuGet package to your project, either SadConsole.Host.SFML or SadConsole.Host.MonoGame.

Here is an example program.cs file that let's you get started with SadConsole quickly. If you're using Visual Basic, starter code is listed below the C# code.

using System;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

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
            SadConsole.Game.Instance.OnStart = Init;
                        
            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            // Any startup code for your game. We will use an example console for now
            var startingConsole = (Console)GameHost.Instance.Screen;
            startingConsole.FillWithRandomGarbage(255);
            startingConsole.Fill(new Rectangle(3, 3, 27, 5), null, Color.Black, 0, Mirror.None);
            startingConsole.Print(6, 5, "Hello from SadConsole", Color.AnsiCyanBright);
        }
    }
}


Example program.vb:

Imports SadConsole
Imports Console = SadConsole.Console
Imports SadRogue.Primitives

Module Module1

    Sub Main()

        ' Setup the engine And create the main window.
        SadConsole.Game.Create(80, 25)

        ' Hook the start event so we can add consoles to the system.
        SadConsole.Game.Instance.OnStart = AddressOf Init

        ' Start the game.
        SadConsole.Game.Instance.Run()
        SadConsole.Game.Instance.Dispose()

    End Sub

    Sub Init()

        Dim startingConsole = DirectCast(GameHost.Instance.Screen, Console)

        startingConsole.FillWithRandomGarbage(255)
        startingConsole.Fill(New Rectangle(3, 3, 27, 3), Color.Violet, Color.Black, 0, Mirror.None)
        startingConsole.Print(6, 5, "Hello from SadConsole", Color.AnsiCyanBright)

    End Sub

End Module
