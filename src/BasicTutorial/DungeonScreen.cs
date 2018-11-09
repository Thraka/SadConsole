using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Actions;

namespace BasicTutorial
{
    class DungeonScreen : ScreenObject
    {
        public static readonly Rectangle ScreenRegionMap = new Rectangle(0, 0, Program.ScreenWidth - 10, Program.ScreenHeight - 5);

        public SadConsole.Actions.ActionStack ActionProcessor;

        public bool RunLogicFrame;

        public SadConsole.Maps.SimpleMap Map { get; }

        public DungeonScreen(SadConsole.Maps.SimpleMap map)
        {
            Map = map;
            
            // Setup actions
            ActionProcessor = new SadConsole.Actions.ActionStack();
            ActionProcessor.Push(new SadConsole.Actions.ActionDelegate(ActionKeyboardProcessor));

            Children.Add(Map);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            if (SadConsole.Global.FocusedConsoles.Console != null)
                return;

            while (ActionProcessor.Peek().IsFinished)
                ActionProcessor.Pop();

            ActionProcessor.Peek().Run(timeElapsed);

            if (ActionProcessor.Peek().IsFinished)
                ActionProcessor.Pop();

            // Center view on player
            Map.Surface.CenterViewPortOnPoint(Map.ControlledGameObject.Position);

            // Run logic if valid move made by player
            if (RunLogicFrame)
                RunGameLogicFrame();

            //if (RedrawMap)
            //{
            //    DungeonScreen.RedrawMap = true;
            //    RedrawMap = false;
            //}
            //point.X = Math.Max(0, point.X);
            //point.Y = Math.Max(0, point.Y);
            //point.X = Math.Min(point.X, map.Width - DungeonScreen.Width);
            //point.Y = Math.Min(point.Y, map.Height - DungeonScreen.Height);

            //MapViewPoint = point;

            base.Update(timeElapsed);
        }

        private void ActionKeyboardProcessor(TimeSpan timeElapsed)
        {
            // Handle keyboard when this screen is being run
            if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.Left))
            {
                ActionProcessor.PushAndRun(Move.MoveBy(Map.ControlledGameObject, Directions.West, Map));
                RunLogicFrame = true;
            }

            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.Right))
            {
                ActionProcessor.PushAndRun(Move.MoveBy(Map.ControlledGameObject, Directions.East, Map));
                RunLogicFrame = true;
            }

            if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.Up))
            {
                ActionProcessor.PushAndRun(Move.MoveBy(Map.ControlledGameObject, Directions.North, Map));
                RunLogicFrame = true;
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Keys.Down))
            {
                ActionProcessor.PushAndRun(Move.MoveBy(Map.ControlledGameObject, Directions.South, Map));
                RunLogicFrame = true;
            }
        }

        private void RunGameLogicFrame()
        {
            foreach (var ent in Map.GameObjects)
                ent.Item.ProcessGameFrame();

            // Process player (though it was proc in the previous loop) to make sure they are last to be processed
            Map.ControlledGameObject.ProcessGameFrame();

            // Redraw the map
            //RedrawMap = true;

            RunLogicFrame = false;
        }
    }
}
