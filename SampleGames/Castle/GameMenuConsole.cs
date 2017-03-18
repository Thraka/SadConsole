using System;
using System.Text;
using SadConsole;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole.Instructions;

namespace Castle
{
    internal class GameMenuConsole : Console
    {
        public event EventHandler PlayGame;
        public GameMenuConsole()
            : base(40, 25)
        {
            DrawTitle();
            DrawCastle();
            DrawCommands();
        }

        private void DrawTitle()
        {
            Print(11, 1, "Castle Adventure", Color.White);
        }

        private void DrawCastle()
        {
            // Draw Tower 1
            for (int x = 4; x < 11; x++)
            {
                SetGlyph(x, 5, 177, Color.White);
            }
            for (int x = 5; x < 10; x++)
            {
                for (int y = 6; y < 8; y++)
                {
                    SetGlyph(x, y, 177, Color.White);
                }
            }
            SetGlyph(4, 4, 219, Color.White);
            SetGlyph(6, 4, 219, Color.White);
            SetGlyph(8, 4, 219, Color.White);
            SetGlyph(10, 4, 219, Color.White);

            // Draw Tower 2
            for (int x = 28; x < 35; x++)
            {
                SetGlyph(x, 5, 177, Color.White);
            }
            for (int x = 29; x < 34; x++)
            {
                for (int y = 6; y < 8; y++)
                {
                    SetGlyph(x, y, 177, Color.White);
                }
            }
            SetGlyph(28, 4, 219, Color.White);
            SetGlyph(30, 4, 219, Color.White);
            SetGlyph(32, 4, 219, Color.White);
            SetGlyph(34, 4, 219, Color.White);

            // Draw tops of walls
            SetGlyph(11, 7, 220, Color.White);
            SetGlyph(12, 7, 220, Color.White);
            SetGlyph(14, 7, 220, Color.White);
            SetGlyph(15, 7, 220, Color.White);
            SetGlyph(17, 7, 220, Color.White);
            SetGlyph(18, 7, 220, Color.White);
            SetGlyph(20, 7, 220, Color.White);
            SetGlyph(21, 7, 220, Color.White);
            SetGlyph(23, 7, 220, Color.White);
            SetGlyph(24, 7, 220, Color.White);
            SetGlyph(26, 7, 220, Color.White);
            SetGlyph(27, 7, 220, Color.White);

            // Draw Walls
            for (int x = 5; x < 34; x++)
            {
                for (int y = 8; y < 11; y++)
                {
                    SetGlyph(x, y, 177, Color.White);
                }
            }
            for (int x = 7; x < 32; x++)
            {
                for (int y = 11; y <= 21; y++)
                {
                    SetGlyph(x, y, 177, Color.White);
                }
            }

            // Draw Gate Wall
            for (int x = 17; x < 22; x++)
            {
                for (int y = 17; y <= 21; y++)
                {
                    SetGlyph(x, y, 219, Color.White);
                }
            }


            // Draw Gate
            for (int x = 18; x < 21; x++)
            {
                for (int y = 18; y <= 21; y++)
                {
                    SetGlyph(x, y, 197, Color.White);
                }
            }
        }

        private void DrawCommands()
        {
            for (int x = 0; x < 39; x++)
            {
                SetGlyph(x, 22, 196, Color.White);
            }
            Print(2, 24, "P - to play", Color.White);
            Print(17, 24, "I - for instructions", Color.White);
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            if(info.IsKeyReleased(Keys.P))
            {
                if (PlayGame != null)
                {
                    PlayGame(this, new EventArgs());
                }
                return true;
            }
            if (info.IsKeyReleased(Keys.I))
            {
                PrintInstructions();
                return true;
            }
            return false;
        }

        private void PrintInstructions()
        {
            // Clear screen
            for (int x = 0; x < 40; x++)
            {
                for (int y = 0; y < 23; y++)
                {
                    SetGlyph(x, y, 0, Color.White);
                }
            }
            Print(0, 2, "   You are trapped in a deserted Castle", Color.White);
            Print(0, 3, "and you must escape. It is rumored that", Color.White);
            Print(0, 4, "the castle is  full of treasures.  Can", Color.White);
            Print(0, 5, "you find them all?", Color.White);
            Print(0, 7, " Use the cursor keypad to move your man", Color.White);
            Print(0, 8, "around the rooms.  To pick up a visible", Color.White);
            Print(0, 9, "item,  just run into it.  To pick up an", Color.White);
            Print(0, 10, "item not displayed  on  the screen, use", Color.White);
            Print(0, 11, "the command  GET.  To  attack  monsters", Color.White);
            Print(0, 12, "just  run  into  them,  but only if you", Color.White);
            Print(0, 13, "have a weapon.The computer accepts most", Color.White);
            Print(0, 14, "two word commands. There are 83 rooms &", Color.White);
            Print(0, 15, "13 treaures.", Color.White);
            Print(0, 18, " one hint:Look at everything carefully!", Color.White);
        }
    }
}

