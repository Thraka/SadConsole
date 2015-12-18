using System;
using SadConsole;
using System.Collections.Generic;
using SadConsole.Consoles;
using Console = SadConsole.Consoles.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole.Instructions;

namespace Snake
{
    internal enum GameState
    {
        MenuScreen,
        Running,
        GameOver
    }
    internal class SnakeConsole : Console {

        private const int gameSpeedMs = 100;
        private TimeSpan gameSpeed;
        private DateTime lastUpdate;

        private GameState gameState;

        private InstructionSet astrickKeyAnimation;
        private InstructionSet upKeyAnimation;
        private InstructionSet downKeyAnimation;
        private InstructionSet leftKeyAnimation;
        private InstructionSet rightKeyAnimation;

        private const int phoneButtonColumn1 = 16;
        private const int phoneButtonColumn2 = 23;
        private const int phoneButtonColumn3 = 30;
        private const int phoneButtonRow1 = 32;
        private const int phoneButtonRow2 = 37;
        private const int phoneButtonRow3 = 42;
        private const int phoneButtonRow4 = 47;

        private int currentScore = 0;
        private Player player;
        private Point wafer;
        private Random random;
        
        public SnakeConsole()
            : base(50, 60)
        {

            gameSpeed = new TimeSpan(0, 0, 0, 0, gameSpeedMs);
            lastUpdate = DateTime.Now;

            random = new Random();
            this.IsVisible = false;
            gameState = GameState.MenuScreen;


            // Draw outline of phone
            _cellData.SetCharacter(11, 55, 200, Color.White);
            _cellData.SetCharacter(39, 55, 188, Color.White);
            for(int i = 12; i < 39; i++)
            {
                _cellData.SetCharacter(i, 55, 205, Color.White);
            }
            for (int i = 30; i < 55; i++)
            {
                _cellData.SetCharacter(11, i, 186, Color.White);
                _cellData.SetCharacter(39, i, 186, Color.White);
            }
            _cellData.SetCharacter(11, 29, 187, Color.White);
            _cellData.SetCharacter(39, 29, 201, Color.White);
            _cellData.SetCharacter(10, 29, 200, Color.White);
            _cellData.SetCharacter(40, 29, 188, Color.White);
            for (int i = 5; i < 29; i++)
            {
                _cellData.SetCharacter(10, i, 186, Color.White);
                _cellData.SetCharacter(40, i, 186, Color.White);
            }
            _cellData.SetCharacter(10, 4, 201, Color.White);
            _cellData.SetCharacter(40, 4, 187, Color.White);
            for (int i = 11; i < 40; i++)
            {
                _cellData.SetCharacter(i, 4, 205, Color.White);
            }

            // Draw Phone Buttons
            DrawPhoneButton(phoneButtonColumn1, phoneButtonRow1, "1");
            DrawPhoneButton(phoneButtonColumn2, phoneButtonRow1, "2");
            DrawPhoneButton(phoneButtonColumn3, phoneButtonRow1, "3");

            DrawPhoneButton(phoneButtonColumn1, phoneButtonRow2, "4");
            DrawPhoneButton(phoneButtonColumn2, phoneButtonRow2, "5");
            DrawPhoneButton(phoneButtonColumn3, phoneButtonRow2, "6");

            DrawPhoneButton(phoneButtonColumn1, phoneButtonRow3, "7");
            DrawPhoneButton(phoneButtonColumn2, phoneButtonRow3, "8");
            DrawPhoneButton(phoneButtonColumn3, phoneButtonRow3, "9");

            DrawPhoneButton(phoneButtonColumn1, phoneButtonRow4, "*");
            DrawPhoneButton(phoneButtonColumn2, phoneButtonRow4, "0");
            DrawPhoneButton(phoneButtonColumn3, phoneButtonRow4, "#");

            // Create Animations  AstrickKeyAnimationCb
            astrickKeyAnimation = new InstructionSet();
            CodeInstruction astrickCallback = new CodeInstruction();
            astrickCallback.CodeCallback = AstrickKeyAnimationCb;
            astrickKeyAnimation.Instructions.AddLast(astrickCallback);

            upKeyAnimation = new InstructionSet();
            CodeInstruction upCallback = new CodeInstruction();
            upCallback.CodeCallback = UpKeyAnimationCb;
            upKeyAnimation.Instructions.AddLast(upCallback);

            downKeyAnimation = new InstructionSet();
            CodeInstruction downCallback = new CodeInstruction();
            downCallback.CodeCallback = DownKeyAnimationCb;
            downKeyAnimation.Instructions.AddLast(downCallback);

            leftKeyAnimation = new InstructionSet();
            CodeInstruction leftCallback = new CodeInstruction();
            leftCallback.CodeCallback = LeftKeyAnimationCb;
            leftKeyAnimation.Instructions.AddLast(leftCallback);

            rightKeyAnimation = new InstructionSet();
            CodeInstruction rightCallback = new CodeInstruction();
            rightCallback.CodeCallback = RightKeyAnimationCb;
            rightKeyAnimation.Instructions.AddLast(rightCallback);

            // Draw Microphone
            for (int i = 24; i < 27; i++)
            {
                _cellData.SetCharacter(i, 53, 240, Color.DarkGray);
            }

            // Draw Plate
            for (int x = 12; x < 39; x++)
            {
                for (int y = 6; y < 28; y++)
                {
                    _cellData.SetCharacter(x, y, 178, Color.Silver);
                }
            }
            for (int x = 13; x < 38; x++)
            {
                for (int y = 28; y < 30; y++)
                {
                    _cellData.SetCharacter(x, y, 178, Color.Silver);
                }
            }

            // Draw End and CAll Button
            for (int x = phoneButtonColumn1; x < phoneButtonColumn1 + 7; x++)
            {
                for (int y = 26; y < 29; y++)
                {
                    _cellData.SetCharacter(x, y, 219, Color.Gray);
                }
            }
            this.CellData.Print(phoneButtonColumn1 + 2, 27, "End", Color.Red, Color.Gray);

            for (int x = phoneButtonColumn3 - 2; x < phoneButtonColumn3 - 2 + 7; x++)
            {
                for (int y = 26; y < 29; y++)
                {
                    _cellData.SetCharacter(x, y, 219, Color.Gray);
                }
            }
            this.CellData.Print(phoneButtonColumn3 - 1, 27, "Call", Color.Green, Color.Gray);


            // Draw Speaker
            for (int i = 22; i < 29; i++)
            {
                _cellData.SetCharacter(i, 7, 240, Color.Black, Color.Silver);
            }

            // Draw Brand
            this.CellData.Print(23, 10, "NOKIA", Color.White, Color.Silver);

            // Draw Screen
            ClearScreen();

            // Draw Game Name
            CreateStartMessage();

            player = null;


        }

        private SadConsole.Effects.Fade CreateFadeAnimation()
        {
            SadConsole.Effects.Fade fadeEffect = new SadConsole.Effects.Fade();
            fadeEffect.AutoReverse = true;
            fadeEffect.DestinationForeground = Color.Turquoise;
            fadeEffect.FadeForeground = true;
            fadeEffect.Repeat = false;
            fadeEffect.FadeDuration = 0.1f;
            return fadeEffect;
        }
        private void AstrickKeyAnimationCb(CodeInstruction codeInstruction)
        {
            SadConsole.Effects.Fade fadeEffect = CreateFadeAnimation();
            List<Cell> cells = new List<Cell>();
            for (int x = phoneButtonColumn1; x < phoneButtonColumn1 + 5; x++)
            {
                for (int y = phoneButtonRow4; y < phoneButtonRow4 + 3; y++)
                {
                    cells.Add(_cellData[x, y]);
                }
            }
            _cellData.SetEffect(cells, fadeEffect);
        }
        private void UpKeyAnimationCb(CodeInstruction codeInstruction)
        {
            SadConsole.Effects.Fade fadeEffect = CreateFadeAnimation();
            List<Cell> cells = new List<Cell>();
            for (int x = phoneButtonColumn2; x < phoneButtonColumn2 + 5; x++)
            {
                for (int y = phoneButtonRow1; y < phoneButtonRow1 + 3; y++)
                {
                    cells.Add(_cellData[x, y]);
                }
            }
            _cellData.SetEffect(cells, fadeEffect);
        }
        private void DownKeyAnimationCb(CodeInstruction codeInstruction)
        {
            SadConsole.Effects.Fade fadeEffect = CreateFadeAnimation();
            List<Cell> cells = new List<Cell>();
            for (int x = phoneButtonColumn2; x < phoneButtonColumn2 + 5; x++)
            {
                for (int y = phoneButtonRow3; y < phoneButtonRow3 + 3; y++)
                {
                    cells.Add(_cellData[x, y]);
                }
            }
            _cellData.SetEffect(cells, fadeEffect);
        }
        private void LeftKeyAnimationCb(CodeInstruction codeInstruction)
        {
            SadConsole.Effects.Fade fadeEffect = CreateFadeAnimation();
            List<Cell> cells = new List<Cell>();
            for (int x = phoneButtonColumn1; x < phoneButtonColumn1 + 5; x++)
            {
                for (int y = phoneButtonRow2; y < phoneButtonRow2 + 3; y++)
                {
                    cells.Add(_cellData[x, y]);
                }
            }
            _cellData.SetEffect(cells, fadeEffect);
        }
        private void RightKeyAnimationCb(CodeInstruction codeInstruction)
        {
            SadConsole.Effects.Fade fadeEffect = CreateFadeAnimation();
            List<Cell> cells = new List<Cell>();
            for (int x = phoneButtonColumn3; x < phoneButtonColumn3 + 5; x++)
            {
                for (int y = phoneButtonRow2; y < phoneButtonRow2 + 3; y++)
                {
                    cells.Add(_cellData[x, y]);
                }
            }
            _cellData.SetEffect(cells, fadeEffect);
        }

        

        private void DrawPhoneButton(int x, int y, String text)
        {
            for (int a = x; a < x + 5; a++)
            {
                for (int b = y; b < y + 3; b++)
                {
                    _cellData.SetCharacter(a, b, 219, Color.Gray);
                }
            }
            this.CellData.Print(x + 2, y + 1, text, Color.Black, Color.Gray);
        }

        private void ClearScreen()
        {
            for (int x = 14; x < 37; x++)
            {
                for (int y = 12; y < 24; y++)
                {
                    _cellData.SetCharacter(x, y, 219, Color.DarkSeaGreen);
                }
            }
        }

        private void CreateStartMessage()
        {
            this.CellData.Print(23, 14, "ASCII", Color.Black, Color.DarkSeaGreen);
            this.CellData.Print(23, 16, "SNAKE", Color.Black, Color.DarkSeaGreen);
            this.CellData.Print(21, 20, "Press * to", Color.Black, Color.DarkSeaGreen);
            this.CellData.Print(23, 21, "Start", Color.Black, Color.DarkSeaGreen);
        }

        public override bool ProcessKeyboard(SadConsole.Input.KeyboardInfo info)
        {
            switch(gameState)
            {
                case GameState.MenuScreen:
                    return ProcessKeyboardMenu(info);
                case GameState.Running:
                    return ProcessKeyboardRunning(info);
                case GameState.GameOver:
                    return ProcessKeyboardGameOver(info);
                default:
                    return false;
            }
        }

        private bool ProcessKeyboardMenu(SadConsole.Input.KeyboardInfo info)
        {
            bool processedKeyboard = false;
            if (info.IsKeyReleased(Keys.Multiply))
            {
                astrickKeyAnimation.Run();
                ResetGame();
                processedKeyboard = true;
            }
            return processedKeyboard;
        }

        private bool ProcessKeyboardRunning(SadConsole.Input.KeyboardInfo info)
        {
            bool processedKeyboard = false;
            
            if (player.ProcessKeyboard(info))
            {
                processedKeyboard = true;
                if (info.IsKeyDown(Keys.Up))
                {
                    upKeyAnimation.Run();
                }
                else if (info.IsKeyDown(Keys.Down))
                {
                    downKeyAnimation.Run();
                }
                else if (info.IsKeyDown(Keys.Left))
                {
                    leftKeyAnimation.Run();
                }
                else if (info.IsKeyDown(Keys.Right))
                {
                    rightKeyAnimation.Run();
                }
            }     
            
            return processedKeyboard;
        }
        private bool ProcessKeyboardGameOver(SadConsole.Input.KeyboardInfo info)
        {
            bool processedKeyboard = false;
            if (info.IsKeyReleased(Keys.Multiply))
            {
                astrickKeyAnimation.Run();
                CreateMenu();
                processedKeyboard = true;
            }
            return processedKeyboard;
        }

        private void ResetGame()
        {
            ClearScreen();
            currentScore = 0;
            DrawScore(currentScore);
            DrawGameBorder();
            player = new Player();
            player.Position = new Microsoft.Xna.Framework.Point(18, 18);
            player.SetStartingPosition(player.Position);
            _cellData.SetCharacter(player.Position.X, player.Position.Y, 1, Color.Black, Color.DarkSeaGreen);

            CreateRandomWaferLocation();
            _cellData.SetCharacter(wafer.X, wafer.Y, 249, Color.Black, Color.DarkSeaGreen);

            gameState = GameState.Running;
        }

        private void EndGame()
        {
            //ClearScreen();
            this.CellData.Print(23, 15, "GAME", Color.Black, Color.DarkSeaGreen);
            this.CellData.Print(23, 17, "OVER", Color.Black, Color.DarkSeaGreen);

            this.CellData.Print(21, 20, "Press * to", Color.Black, Color.DarkSeaGreen);
            this.CellData.Print(23, 21, "Exit", Color.Black, Color.DarkSeaGreen);

            gameState = GameState.GameOver;
        }
        private void CreateMenu()
        {
            ClearScreen();
            CreateStartMessage();
            gameState = GameState.MenuScreen;
        }

        private void DrawGameBorder()
        {
            _cellData.SetCharacter(14, 13, 218, Color.Black, Color.DarkSeaGreen);
            _cellData.SetCharacter(36, 13, 191, Color.Black, Color.DarkSeaGreen);
            _cellData.SetCharacter(14, 23, 192, Color.Black, Color.DarkSeaGreen);
            _cellData.SetCharacter(36, 23, 217, Color.Black, Color.DarkSeaGreen);
            for (int x = 15; x < 36; x++)
            {
                _cellData.SetCharacter(x, 13, 196, Color.Black, Color.DarkSeaGreen);
                _cellData.SetCharacter(x, 23, 196, Color.Black, Color.DarkSeaGreen);
            }
            for (int y = 14; y < 23; y++)
            {
                _cellData.SetCharacter(14, y, 179, Color.Black, Color.DarkSeaGreen);
                _cellData.SetCharacter(36, y, 179, Color.Black, Color.DarkSeaGreen);
            }
        }
        private void DrawScore(int score)
        {
            String scoreString = Convert.ToString(score);
            while(scoreString.Length <= 5)
            {
                scoreString = String.Format("0{0}", scoreString);
            }

            this.CellData.Print(14, 12, scoreString, Color.Black, Color.DarkSeaGreen);
        }

        private void CreateRandomWaferLocation()
        {
            bool retry = true;
            while (retry)
            {
                wafer = new Point(random.Next(15, 35), random.Next(14, 22));
                retry = CollisionDetection(wafer);
            }


        }

        private bool CollisionDetection(Point target)
        {
            if (CollisionDetectionWall(target))
            {
                return true;
            }
            else if (CollisionDetectionSelf(target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CollisionDetectionWall(Point target)
        {
            if (target.X == 14)
            {
                return true;
            }
            else if (target.X == 36)
            {
                return true;
            }
            else if (target.Y == 13)
            {
                return true;
            }
            else if (target.Y == 23)
            {
                return true;
            }

            return false;
        }
        private bool CollisionDetectionSelf(Point target)
        {
            bool first = true;
            foreach (Point self in player.TailNodes)
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                if (target.X == self.X && target.Y == self.Y)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Update()
        {
            
            switch(gameState)
            {
                case GameState.Running:
                    UpdateGame();
                    break;
            }

            base.Update();
        }


        private void UpdateGame()
        {
            if (lastUpdate.Add(gameSpeed) < DateTime.Now)
            {
                lastUpdate = DateTime.Now;
                RunGameLogic();
            }
        }
        private void RunGameLogic()
        {
            // Move the player to the next stop
            player.Move();

            // Update the tail
            player.ProcessTail(player.Position);

            // Erase any points that are old
            foreach (Point removePoint in player.RemoveNodes)
            {
                _cellData.SetCharacter(removePoint.X, removePoint.Y, 0);
            }

            // Draw the new spot
            _cellData.SetCharacter(player.Position.X, player.Position.Y, 1, Color.Black, Color.DarkSeaGreen);

            // Detect if we hit something
            if (CollisionDetection(player.Position))
            {
                EndGame();
            }

            // Detect if we hit the wafer
            if (player.Position.X == wafer.X && player.Position.Y == wafer.Y)
            {
                currentScore += 3;
                DrawScore(currentScore);
                player.MaxTailLength++;

                CreateRandomWaferLocation();

                _cellData.SetCharacter(wafer.X, wafer.Y, 249, Color.Black, Color.DarkSeaGreen);

            }
        }
    }
}
