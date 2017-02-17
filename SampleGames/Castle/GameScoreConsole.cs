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
    internal class GameScoreConsole : Console
    {
        public event EventHandler RestartGame;
        public event EventHandler QuitGame;
        private int releaseCount;
        private bool playAgain;
        private Collection<CastleItem> scoringItems;
        private Collection<Monster> scoringMonsters;
        private bool win;

        public GameScoreConsole(CastleConsole castleConsole) : base(40, 25)
        {
            if(castleConsole.GameResult == GameResult.Win)
            {
                win = true;
            }
            else
            {
                win = false;
            }

            releaseCount = 2;
            playAgain = false;
            PrintResult(castleConsole.GameResult);
            PrintTreasures(castleConsole);
            PrintMonsters(castleConsole);
            PrintScore();
            Print(11, 24, "- Press any key -", Color.White);
        }

        private void PrintResult(GameResult result)
        {
            switch(result)
            {
                case GameResult.Quit:
                    Print(8, 1, "You've ended the game!", Color.White);
                    break;
                case GameResult.Failed:
                    Print(5, 1, "Your're have failed to Escape!", Color.White);
                    break;
                case GameResult.Win:
                    Print(5, 1, "You've Escaped the Castle!", Color.White);
                    break;
            }
        }

        private void PrintTreasures(CastleConsole castleConsole)
        {
            Print(1, 5, "You have collected these Treasues...", Color.White);

            scoringItems = new Collection<CastleItem>();
            foreach(var item in castleConsole.ItemManager.CastleItems)
            {
                if(item.Collected)
                {
                    if(item.Value > 0)
                    {
                        scoringItems.Add(item);
                    }
                }
            }

            if(scoringItems.Count == 0)
            {
                Print(17, 7, "NONE", Color.White);
            }
            else
            {
                int xPoint = 19;
                xPoint = xPoint -= scoringItems.Count;
                foreach (var item in this.scoringItems)
                {
                    SetGlyph(xPoint, 7, item.Character, Color.White);
                    xPoint += 2;
                }
            }

        }

        private void PrintMonsters(CastleConsole castleConsole)
        {
            Print(1, 11, "You have killed these Monsters......", Color.White);
            scoringMonsters = new Collection<Monster>();

            foreach(Monster monster in castleConsole.ItemManager.CastleMonsters)
            {
                if(monster.IsAlive == false)
                {
                    if(monster.Value > 0)
                    {
                        scoringMonsters.Add(monster);
                    }
                }
            }
            
            if (scoringMonsters.Count == 0)
            {
                Print(17, 13, "NONE", Color.White);
            }
            else
            {
                int xPoint = 19;
                xPoint = xPoint -= scoringMonsters.Count;
                foreach (var item in this.scoringMonsters)
                {
                    SetGlyph(xPoint, 13, item.Character, Color.White);
                    xPoint += 2;
                }
            }
        }

        private void PrintScore()
        {
            int score = 0;
            foreach (var item in this.scoringItems)
            {
                score += item.Value;
            }
            foreach (var item in this.scoringMonsters)
            {
                score += item.Value;
            }
            if (win)
            {
                score += 100;
            }
            Print(12, 20, String.Format("Your Score: {0}", score), Color.White);
            Print(11, 21, "(1550 is perfect)", Color.White);
        }

        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            if(info.KeysReleased.Count > 0)
            {
                if (releaseCount > 0)
                {
                    releaseCount--;
                    return true;
                }
                if (playAgain == false)
                {
                    Print(8, 24, "PLAY AGAIN (Y/N)?     ", Color.White);
                    playAgain = true;
                    return true;
                }
                if(info.IsKeyReleased(Keys.Y))
                {
                    if (RestartGame != null)
                    {
                        RestartGame(this, new EventArgs());
                    }
                    return true;
                }

                if (info.IsKeyReleased(Keys.N))
                {
                    if (QuitGame != null)
                    {
                        QuitGame(this, new EventArgs());
                    }
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

    }
}
