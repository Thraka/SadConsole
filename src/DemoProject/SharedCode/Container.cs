using System;
using Console = SadConsole.Console;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using StarterProject.CustomConsoles;

namespace StarterProject
{
    class Container : ConsoleContainer
    {
        private int currentConsoleIndex = -1;
        private IConsoleMetadata selectedConsole;
        private HeaderConsole headerConsole;

        IConsoleMetadata[] consoles;

        public Container()
        {
            headerConsole = new HeaderConsole();

            consoles = new IConsoleMetadata[] {
                new CustomConsoles.AutoTypingConsole(),
                new CustomConsoles.DOSConsole(),
                new CustomConsoles.ScrollableConsole(25, 6, 70),
                new CustomConsoles.CursorConsole(),
                new CustomConsoles.GameObjectConsole(),
                new CustomConsoles.ViewsAndSubViews(),
                new CustomConsoles.ControlsTest(),
                //new CustomConsoles.SubConsoleCursor(),     //Virtual cursor not working correctly yet
                new CustomConsoles.SceneProjectionConsole(),
                new CustomConsoles.StringParsingConsole(),
                new CustomConsoles.AnsiConsole(),
                new CustomConsoles.StretchedConsole(),
                new CustomConsoles.RandomScrollingConsole(),
            };

            MoveNextConsole();
        }

        public void MoveNextConsole()
        {
            currentConsoleIndex++;

            if (currentConsoleIndex >= consoles.Length)
                currentConsoleIndex = 0;

            selectedConsole = consoles[currentConsoleIndex];

            Children.Clear();
            Children.Add(selectedConsole);
            Children.Add(headerConsole);

            selectedConsole.IsVisible = true;
            selectedConsole.IsFocused = true;
            selectedConsole.Position = new Point(0, 2);

            Console.ActiveConsoles.Set(selectedConsole);
            headerConsole.SetConsole(selectedConsole);
        }

        public override bool ProcessKeyboard(KeyboardInfo info)
        {
            return selectedConsole.ProcessKeyboard(info);
        }

        public override bool ProcessMouse(MouseInfo info)
        {
            return selectedConsole.ProcessMouse(info);
        }
    }
}