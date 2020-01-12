using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;
using SadRogue.Primitives;
using ZReader;

namespace Game.Screens
{
    class Board : ScreenSurface
    {
        private GameObject _playerControlledObject;

        public string Name { get; protected set; }

        public GameObject PlayerControlledObject
        {
            get => _playerControlledObject;
            set
            {
                _playerControlledObject = value;

                if (value != null && !value.HasComponent<ObjectComponents.Movable>())
                    throw new Exception("Cannot assign object as player controlled when it doesn't have a movable component.");


            }
        }

        public Board(int width, int height) : base(width, height)
        {
            for (int i = 0; i < Surface.Cells.Length; i++)
            {
                var tile = Game.Factories.TileFactory.Instance.Create("empty", Factories.TileBlueprint.Config.Empty);
                tile.Position = SadRogue.Primitives.Point.FromIndex(i, width);
                Surface.Cells[i] = tile;
            }
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
                ZElement.Types.TextBlack => ("wall-solid", new Factories.TileBlueprint.Config(ColorAnsi.WhiteBright, ColorAnsi.Black, data)),
                ZElement.Types.TextBlue => ("wall-solid", new Factories.TileBlueprint.Config(ColorAnsi.WhiteBright, ColorAnsi.Blue, data)),
                ZElement.Types.TextBrown => ("wall-solid", new Factories.TileBlueprint.Config(ColorAnsi.WhiteBright, ColorAnsi.Yellow, data)),
                ZElement.Types.TextCyan => ("wall-solid", new Factories.TileBlueprint.Config(ColorAnsi.WhiteBright, ColorAnsi.Cyan, data)),
                ZElement.Types.TextGreen => ("wall-solid", new Factories.TileBlueprint.Config(ColorAnsi.WhiteBright, ColorAnsi.Green, data)),
                ZElement.Types.TextPurple => ("wall-solid", new Factories.TileBlueprint.Config(ColorAnsi.WhiteBright, ColorAnsi.Magenta, data)),
                ZElement.Types.TextRed => ("wall-solid", new Factories.TileBlueprint.Config(ColorAnsi.WhiteBright, ColorAnsi.Red, data)),
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

            var producedTile = Factories.TileFactory.Instance.Create(blueprint, config);

            producedTile.CopyAppearanceTo(tile);

            foreach (var component in producedTile.GetComponents<object>())
                tile.AddComponent(component);
        }

        public GameObject ImportZGameObject(ZReader.ZElement.Types type, Point position, byte data, ZStatusElement status = null)
        {
            var element = new ZReader.ZElement(type, data);
            var config = new Factories.GameObjectBlueprint.Config(ZReaderExtensions.GetColor(element.ForeColor), ZReaderExtensions.GetColor(element.BackColor), element.Glyph);
            var configColors = new Factories.GameObjectBlueprint.Config(ZReaderExtensions.GetColor(element.ForeColor), ZReaderExtensions.GetColor(element.BackColor));

            if (type == ZElement.Types.Pusher)
                status = new ZStatusElement() { StepX = 1 };

            (string blueprint, Factories.GameObjectBlueprint.Config config) factoryDef = type switch
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
                _ => ("dead", Factories.GameObjectBlueprint.Config.Empty)
            };

            if (factoryDef.blueprint == "dead")
                System.Diagnostics.Debug.WriteLine("Missing GameObject: " + Enum.GetName(typeof(ZReader.ZElement.Types), type));

            var gameObject = CreateGameObject(position, factoryDef.blueprint, factoryDef.config);

            if (type == ZElement.Types.Player)
                PlayerControlledObject = gameObject;

            return gameObject;
        }

        public GameObject CreateGameObject(Point position, string blueprint, Factories.GameObjectBlueprint.Config config)
        {
            var gameObject = Factories.GameObjectFactory.Instance.Create(blueprint, config);
            gameObject.Position = position;

            Children.Add(gameObject);
            return gameObject;
        }

        public void DestroyGameObject(GameObject obj) =>
            Children.Remove(obj);

        public IEnumerable<GameObject> GetObjects()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is GameObject obj)
                {
                    yield return obj;
                }
            }
        }

        public bool IsObjectAtPosition(Point position, out GameObject obj)
        {
            foreach (var item in GetObjects())
            {
                if (item.Position == position)
                {
                    obj = item;
                    return true;
                }
            }
            obj = null;
            return false;
        }
    }
}
