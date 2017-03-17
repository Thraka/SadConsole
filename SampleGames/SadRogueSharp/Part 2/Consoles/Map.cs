using Microsoft.Xna.Framework;

namespace SadRogue.Consoles
{
    class Map : SadConsole.Console
    {
        public Map(int width, int height): base(new SadConsole.Surfaces.BasicSurface(width, height, new Rectangle(0, 0, Program.ScreenWidth, Program.ScreenHeight)))
        {

        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SadConsole.Input;
//using Microsoft.Xna.Framework.Input;
//using SadConsole.Effects;
//using Microsoft.Xna.Framework;
//using SadConsole;

//namespace SadRogue.Consoles
//{
//    class MapConsole : SadConsole.Console
//    {
//        protected IMap map;
//        protected List<SadConsole.GameHelpers.GameObject> entities;
//        protected Entities.Player player;
//        protected RogueSharp.Random.IRandom random = new RogueSharp.Random.DotNetRandom();

//        Recolor explored;
//        SadConsole.Cell[,] mapData;

//        public MapConsole(int width, int height) : base(width, height)
//        {
//            // Create the map
//            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(width, height, 100, 15, 4);
//            map = Map.Create(mapCreationStrategy);

//            mapData = new SadConsole.Cell[width, height];

            

//            foreach (var cell in map.GetAllCells())
//            {
//                if (cell.IsWalkable)
//                {
//                    // Our local information about each map square
//                    mapData[cell.X, cell.Y] = new MapObjects.Floor();
//                }
//                else
//                {
//                    mapData[cell.X, cell.Y] = new MapObjects.Wall();
//                }

//                // Copy the appearance we've defined for Floor or Wall or whatever, to the actual console data that is rendered
//                mapData[cell.X, cell.Y].CopyAppearanceTo(this[cell.X, cell.Y]);
//            }

//            // Create map effects
//            explored = new SadConsole.Effects.Recolor();
//            explored.Background = Color.White * 0.3f;
//            explored.Foreground = Color.White * 0.3f;
//            explored.Update(10d); // Trickery to force the fade to complete to the destination color.
//            explored.RemoveOnFinished = true;
//            explored.CloneOnApply = true;
//            explored.Permanent = true;
//            explored.KeepStateOnFinished = true;

//            // Entities
//            entities = new List<SadConsole.GameHelpers.GameObject>();
            
//            // Create the player
//            player = new Entities.Player();
//            var tempCell = GetRandomEmptyCell();
//            player.Position = new Microsoft.Xna.Framework.Point(tempCell.X, tempCell.Y);
//            player.RenderOffset = this.Position;
//            entities.Add(player);
            
//            // Create a hound
//            GenerateHound();
//            GenerateHound();
//            GenerateHound();

//            // Initial view
//            UpdatePlayerView();

//            // Keyboard setup
//            SadConsole.Global.KeyboardState.RepeatDelay = 0.07f;
//            SadConsole.Global.KeyboardState.InitialRepeatDelay = 0.1f;
//        }

//        private void GenerateHound()
//        {
//            var hound = new Entities.Hound();
//            hound.RenderOffset = this.Position;
//            var tempCell = GetRandomEmptyCell();
//            hound.Position = new Microsoft.Xna.Framework.Point(tempCell.X, tempCell.Y);
//            entities.Add(hound);
//        }

//        public override void Update(TimeSpan delta)
//        {
//            // Normally just the console data for this console is "updated" each frame, 
//            // but we want to also update all entities.
//            foreach (var entity in entities)
//                entity.Update();

//            base.Update(delta);
//        }


//        public override void Draw(TimeSpan delta)
//        {
            
//            base.Draw(delta);

//            // Normally only the console data is rendered through this call, but after
//            // that has finished, we want to render our entities on top of that.
//            foreach (var entity in entities)
//                entity.Draw(delta);
//        }

//        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
//        {
//            bool keyHit = false;

//            var newPosition = player.Position;

//            if (info.KeysPressed.Contains(AsciiKey.Get(Keys.Up)))
//            {
//                newPosition.Y -= 1;
//                keyHit = true;
//            }
//            else if (info.KeysPressed.Contains(AsciiKey.Get(Keys.Down)))
//            {
//                newPosition.Y += 1;
//                keyHit = true;
//            }

//            if (info.KeysPressed.Contains(AsciiKey.Get(Keys.Left)))
//            {
//                newPosition.X -= 1;
//                keyHit = true;
//            }
//            else if (info.KeysPressed.Contains(AsciiKey.Get(Keys.Right)))
//            {
//                newPosition.X += 1;
//                keyHit = true;
//            }

//            // Test location
//            if (map.IsWalkable(newPosition.X, newPosition.Y))
//            {
//                player.Position = newPosition;

//                UpdatePlayerView();

//            }

//            return keyHit || base.ProcessKeyboard(info);

//        }

//        private void UpdatePlayerView()
//        {
//            // Find out what the player can see
//            map.ComputeFov(player.Position.X, player.Position.Y, 20, true);

//            // Calculate the view area and sync it with our player location
//            textSurface.RenderArea = new Microsoft.Xna.Framework.Rectangle(player.Position.X - 15, player.Position.Y - 15, 30, 30);
//            player.RenderOffset = this.Position - textSurface.RenderArea.Location;

//            // Mark all render points as visible or not
//            for (int x = 0; x < textSurface.RenderArea.Width; x++)
//            {
//                for (int y = 0; y < textSurface.RenderArea.Height; y++)
//                {
//                    var point = new Microsoft.Xna.Framework.Point(x + textSurface.RenderArea.Left, y + textSurface.RenderArea.Top);
//                    var i = textSurface.GetIndexFromPoint(point);
//                    var currentCell = map.GetCell(point.X, point.Y);

//                    if (currentCell.IsInFov)
//                    {
//                        if (this[i].State != null)
//                        {
//                            explored.Clear(this[i]);
//                            this[i].RestoreState();
//                        }

//                        this[i].IsVisible = true;
//                        map.SetCellProperties(point.X, point.Y, currentCell.IsTransparent, currentCell.IsWalkable, true);
//                    }
//                    else if (currentCell.IsExplored)
//                    {
//                        this[i].IsVisible = true;
//                        explored.Apply(this[i]);
//                    }
//                    else
//                    {
//                        this[i].IsVisible = false;
//                    }
//                }
//            }
            
//            // Check for entities.
//            foreach (var entity in entities)
//            {
//                if (entity != player)
//                {
//                    entity.IsVisible = map.IsInFov(entity.Position.X, entity.Position.Y);
                    
//                    // Entity is in our view, but it may not be within the viewport.
//                    if (entity.IsVisible)
//                    {
//                        entity.RenderOffset = this.Position - textSurface.RenderArea.Location;

//                        // If the entity is not in our view area we don't want to show it.
//                        if (!textSurface.RenderArea.Contains(entity.Position))
//                            entity.IsVisible = false;
//                    }

//                }
//            }

//            textSurface.IsDirty = true;
//        }

//        private RogueSharp.Cell GetRandomEmptyCell()
//        {

//            while (true)
//            {
//                int x = random.Next(Width - 1);
//                int y = random.Next(Height - 1);
//                if (map.IsWalkable(x, y))
//                {
//                    return map.GetCell(x, y);
//                }
//            }

//        }

//    }
//}
