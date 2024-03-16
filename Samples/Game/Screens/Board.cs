﻿using ZReader;

namespace ZZTGame.Screens;

class Board : ScreenSurface
{
    private List<GameObject> _gameObjects;
    private GameObject _playerControlledObject;
    private SadConsole.Entities.EntityManager _entities;

    public string Name { get; protected set; }

    public GameObject PlayerControlledObject
    {
        get => _playerControlledObject;
        set
        {
            _playerControlledObject?.RemoveComponent(_playerControlledObject.GetComponent<ObjectComponents.PlayerControlled>());

            _playerControlledObject = value;

            if (value != null && !value.HasComponent<ObjectComponents.Movable>())
                throw new Exception("Cannot assign object as player controlled when it doesn't have a movable component.");

            _playerControlledObject.AddComponent(new ObjectComponents.PlayerControlled());
        }
    }

    public Board(int width, int height) : base(width, height)
    {
        var surface = (CellSurface)Surface;

        for (int i = 0; i < Surface.Count; i++)
        {
            var tile = ZZTGame.Factories.TileFactory.Instance.Create("empty", Factories.TileBlueprint.Config.Empty);
            tile.Position = SadRogue.Primitives.Point.FromIndex(i, width);
            surface.Cells[i] = tile;
        }

        _entities = new();
        _gameObjects = new();
        SadComponents.Add(_entities);
    }

    internal static Board ImportZZT(ZBoard board)
    {
        var newBoard = new Board(60, 25);
        var cellIndex = 0;

        var boardElementDataByPosition = new Dictionary<Point, ZStatusElement>();

        foreach (var item in board.Elements)
            boardElementDataByPosition.Add(new Point(item.LocationX, item.LocationY), item);

        newBoard.Name = board.BoardName;

        foreach (byte[] item in board.BoardData)
        {
            int count = item[0] == 0 ? 256 : item[0];
            var type = (ZReader.ZElement.Types)item[1];
            byte data = item[2];
            
            for (int i = 0; i < count; i++)
            {
                var position = Point.FromIndex(cellIndex, 60);
                boardElementDataByPosition.TryGetValue(position, out ZStatusElement status);

                if (ZReader.ZElement.IsTerrain(type))
                    newBoard.SetZTerrain(type, position, data, status);
                else
                    newBoard.ImportZGameObject(type, position, data, status);

                cellIndex++;
            }
        }

        return newBoard;
    }

    private void SetZTerrain(ZReader.ZElement.Types type, Point position, byte data, ZStatusElement status = null)
    {
        // We treat some of the normal ZZT terrain types as game objects. Exit out for those.
        if (type == ZElement.Types.Boulder || type == ZElement.Types.SliderEW || type == ZElement.Types.SliderNS || type == ZElement.Types.Pusher)
        {
            ImportZGameObject(type, position, data);
            return;
        }

        var element = new ZElement(type, data);
        var config = new Factories.TileBlueprint.Config(ZReaderExtensions.GetColor(element.ForeColor), ZReaderExtensions.GetColor(element.BackColor), element.Glyph);

        (string blueprint, Factories.TileBlueprint.Config config) factoryDef = type switch
        {
            ZElement.Types.Empty => ("empty", Factories.TileBlueprint.Config.Empty),
            ZElement.Types.SolidWall => ("wall-solid", config),
            ZElement.Types.BlinkWall => ("wall-blink", config),
            ZElement.Types.BreakableWall => ("wall-breakable", config),
            ZElement.Types.FakeWall => ("wall-fake", config),
            ZElement.Types.NormalWall => ("wall-normal", config),
            ZElement.Types.Line => ("line", config),
            ZElement.Types.TextBlack => ("wall-solid", new Factories.TileBlueprint.Config(Color.AnsiWhiteBright, Color.AnsiBlack, data)),
            ZElement.Types.TextBlue => ("wall-solid", new Factories.TileBlueprint.Config(Color.AnsiWhiteBright, Color.AnsiBlue, data)),
            ZElement.Types.TextBrown => ("wall-solid", new Factories.TileBlueprint.Config(Color.AnsiWhiteBright, Color.AnsiYellow, data)),
            ZElement.Types.TextCyan => ("wall-solid", new Factories.TileBlueprint.Config(Color.AnsiWhiteBright, Color.AnsiCyan, data)),
            ZElement.Types.TextGreen => ("wall-solid", new Factories.TileBlueprint.Config(Color.AnsiWhiteBright, Color.AnsiGreen, data)),
            ZElement.Types.TextPurple => ("wall-solid", new Factories.TileBlueprint.Config(Color.AnsiWhiteBright, Color.AnsiMagenta, data)),
            ZElement.Types.TextRed => ("wall-solid", new Factories.TileBlueprint.Config(Color.AnsiWhiteBright, Color.AnsiRed, data)),
            _ => ("dead", Factories.TileBlueprint.Config.Empty)
        };

        if (factoryDef.blueprint == "dead")
            System.Diagnostics.Debug.WriteLine("Missing Tile: " + Enum.GetName(typeof(ZReader.ZElement.Types), type));

        SetTerrain(position, factoryDef.blueprint, factoryDef.config);

        Surface.IsDirty = true;
    }

    public void SetTerrain(Point position, string blueprint, Factories.TileBlueprint.Config config)
    {
        var tile = (Tiles.BasicTile)Surface[position.X, position.Y];
        tile.SendMessage(new Messages.TileDestroyed(tile, this));

        var producedTile = Factories.TileFactory.Instance.Create(blueprint, config);

        producedTile.CopyAppearanceTo(tile);

        foreach (var component in producedTile.GetComponents<object>())
            tile.AddComponent(component);

        tile.SendMessage(new Messages.TileCreated(tile, this));
    }

    public GameObject ImportZGameObject(ZReader.ZElement.Types type, Point position, byte data, ZStatusElement status = null)
    {
        var element = new ZReader.ZElement(type, data);
        var config = new Factories.GameObjectConfig(ZReaderExtensions.GetColor(element.ForeColor), ZReaderExtensions.GetColor(element.BackColor), element.Glyph);
        var configColors = new Factories.GameObjectConfig(ZReaderExtensions.GetColor(element.ForeColor), ZReaderExtensions.GetColor(element.BackColor), null);

        if (type == ZElement.Types.Pusher)
            status = new ZStatusElement() { StepX = 1 };

        (string blueprint, Factories.GameObjectConfig config) factoryDef = type switch
        {
            ZElement.Types.Player => ("player", config),
            ZElement.Types.Boulder => ("boulder", config),
            ZElement.Types.SliderNS => ("slider-vertical", config),
            ZElement.Types.SliderEW => ("slider-horizontal", config),
            ZElement.Types.Pusher => ((status.StepX, status.StepY) switch
                                      {
                                          (0, -1) => "pusher-north",
                                          (0, 1) => "pusher-south",
                                          (1, 0) => "pusher-east",
                                          (-1, 0) => "pusher-west",
                                          _ => "pusher-idle",
                                      }, configColors),

            ZElement.Types.Ammo => ("ammo", config),
            ZElement.Types.Key => ("key", config),
            ZElement.Types.Door => ("door", config),
            ZElement.Types.Gem => ("gem", config),
            ZElement.Types.Torch => ("torch", config),
            _ => ("dead", Factories.GameObjectConfig.Empty)
        };

        if (factoryDef.blueprint == "dead")
            System.Diagnostics.Debug.WriteLine("Missing GameObject: " + Enum.GetName(typeof(ZReader.ZElement.Types), type));

        var gameObject = CreateGameObject(position, factoryDef.blueprint, factoryDef.config);

        if (type == ZElement.Types.Player)
            PlayerControlledObject = gameObject;

        return gameObject;
    }

    public GameObject CreateGameObject(Point position, string blueprint, Factories.GameObjectConfig config)
    {
        var gameObject = Factories.GameObjectFactory.Instance.Create(blueprint, config);
        gameObject.Position = position;

        gameObject.IsAlive = true;
        _entities.Add(gameObject);
        _gameObjects.Add(gameObject);
        gameObject.SendMessage(new Messages.ObjectCreated(gameObject, this));
        return gameObject;
    }

    public void DestroyGameObject(GameObject obj)
    {
        obj.IsAlive = false;
        _entities.Remove(obj);
        _gameObjects.Remove(obj);
        obj.SendMessage(new Messages.ObjectDestroyed(obj, this));
    }

    public IEnumerable<GameObject> GetObjects()
    {
        for (int i = 0; i < _gameObjects.Count; i++)
            yield return _gameObjects[i];
    }

    public bool IsObjectAtPosition(Point position)
    {
        foreach (var item in GetObjects())
        {
            if (item.Position == position)
                return true;
        }

        return false;
    }

    public bool GetObjectsAtPosition(Point position, out GameObject[] obj)
    {
        List<GameObject> objects = new List<GameObject>(2);

        foreach (var item in GetObjects())
        {
            if (item.Position == position)
            {
                objects.Add(item);
            }
        }
        if (objects.Count != 0)
        {
            obj = objects.ToArray();
            return true;
        }
        else
            obj = null;

        return false;
    }
}
