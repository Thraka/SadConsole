using SadConsole.Input;

namespace ZZTGame.Screens;

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

    public override void Update(TimeSpan delta)
    {
        if (GameBoard == null) return;

        // Update any tick/component on this world object.
        foreach (var component in ComponentsUpdate.ToArray())
            component.Update(this, delta);

        if (_tempPositionMovementValue == Direction.None)
        {
            if (GameBoard.PlayerControlledObject != null)
            {
                //if (GameBoard.PlayerControlledObject.HasComponent<>)
                if (GameHost.Instance.Keyboard.IsKeyPressed(Keys.Left))
                    _tempPositionMovementValue = Direction.Left;
                else if (GameHost.Instance.Keyboard.IsKeyPressed(Keys.Right))
                    _tempPositionMovementValue = Direction.Right;
                else if (GameHost.Instance.Keyboard.IsKeyPressed(Keys.Up))
                    _tempPositionMovementValue = Direction.Up;
                else if (GameHost.Instance.Keyboard.IsKeyPressed(Keys.Down))
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
            child.Update(delta);
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
                if (!world.GameBoard.GetObjectsAtPosition(state.CellPosition, out _))
                {
                    //world.GameBoard.PlayerControlledObject = obj;
                    world.GameBoard.CreateGameObject(state.CellPosition, "pusher-east", Factories.GameObjectConfig.Empty);
                }
            }

            if (state.Mouse.RightClicked)
            {
                if (world.GameBoard.GetObjectsAtPosition(state.CellPosition, out var objects))
                {
                    if (objects[0].HasComponent<ObjectComponents.Pusher>())
                        objects[0].RemoveComponent(objects[0].GetComponent<ObjectComponents.Pusher>());

                    var pusher = new ObjectComponents.Pusher();
                    pusher.Direction = Direction.Types.Down;

                    objects[0].AddComponent(pusher);
                }
            }

            handled = true;
        }
    }
}
