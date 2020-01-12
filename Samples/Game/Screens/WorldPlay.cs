using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;

namespace Game.Screens
{
    class WorldPlay : ScreenObject
    {
        Direction _tempPositionMovementValue;

        public Board GameBoard { get; private set; }

        public Dictionary<string, Board> GameBoards { get; private set; }

        SadConsoleComponents.TickManager _tickManager;

        public bool IsTickerPaused { get; set; }

        public WorldPlay()
        {
            _tickManager = new SadConsoleComponents.TickManager(TimeSpan.FromSeconds(0.05));

            SadComponents.Add(_tickManager);
            SadComponents.Add(new ChangeElementTypeMouseHandler());

            GameBoards = new Dictionary<string, Board>();

            UseMouse = true;
        }

        public Board ImportZZTBoard(ZReader.ZBoard board)
        {
            var newBoard = Board.ImportZZT(board);
            newBoard.IsVisible = false;
            newBoard.IsEnabled = false;
            Children.Add(newBoard);

            GameBoards.Add(newBoard.Name.ToLower().Trim(), newBoard);
            
            newBoard.UseMouse = false;
            newBoard.Surface.ConnectLines();
            newBoard.Surface.IsDirty = true;
            return newBoard;
        }

        public void SetActiveBoard(string name)
        {
            if (GameBoard != null)
            {
                GameBoard.IsVisible = false;
                GameBoard.IsEnabled = false;
            }

            GameBoard = GameBoards[name.ToLower().Trim()];
            GameBoard.IsEnabled = true;
            GameBoard.IsVisible = true;
            GameBoard.IsDirty = true;
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            //return true;
            return base.ProcessKeyboard(keyboard);
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            return base.ProcessMouse(state);
        }

        public override void Update()
        {
            if (GameBoard == null) return;

            // Update any tick/component on this world object.
            foreach (var component in ComponentsUpdate.ToArray())
                component.Update(this);

            if (_tempPositionMovementValue == Direction.None)
            {
                if (GameBoard.PlayerControlledObject != null)
                {
                    //if (GameBoard.PlayerControlledObject.HasComponent<>)
                    if (Global.Keyboard.IsKeyPressed(Keys.Left))
                        _tempPositionMovementValue = Direction.Left;
                    else if (Global.Keyboard.IsKeyPressed(Keys.Right))
                        _tempPositionMovementValue = Direction.Right;
                    else if (Global.Keyboard.IsKeyPressed(Keys.Up))
                        _tempPositionMovementValue = Direction.Up;
                    else if (Global.Keyboard.IsKeyPressed(Keys.Down))
                        _tempPositionMovementValue = Direction.Down;
                }
            }

            // Move the character if requested
            if (_tempPositionMovementValue != Direction.None)
            {
                ((GoRogue.IHasComponents)GameBoard.PlayerControlledObject).GetComponent<ObjectComponents.Movable>()
                    .RequestMove(_tempPositionMovementValue, GameBoard, GameBoard.PlayerControlledObject);

                _tempPositionMovementValue = Direction.None;
            }

            // We have a tick this frame; process objects
            if (_tickManager.TickThisFrame)
            {
                // All game objects
                foreach (var item in GameBoard.GetObjects())
                {
                    item.ProcessTickComponents(this);
                }
            }

            foreach (IScreenObject child in new List<IScreenObject>(Children))
                child.Update();
        }
    }

    class ChangeElementTypeMouseHandler : SadConsole.Components.MouseConsoleComponent
    {
        public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
        {
            handled = false;

            WorldPlay world = (WorldPlay)host;
            state = new MouseScreenObjectState(world.GameBoard, state.Mouse);

            if (state.IsOnScreenObject)
            {
                if (state.Mouse.LeftButtonDown)
                {
                    if (!world.GameBoard.IsObjectAtPosition(state.CellPosition, out _))
                    {
                        //world.GameBoard.PlayerControlledObject = obj;
                        world.GameBoard.CreateGameObject(state.CellPosition, "pusher-east", Factories.GameObjectBlueprint.Config.Empty);
                    }
                }

                handled = true;
            }
        }
    }
}
