using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Actions;
using Console = SadConsole.Console;

namespace BasicTutorial
{
    class DungeonScreen : ContainerConsole
    {
        public static readonly Rectangle ScreenRegionMap = new Rectangle(0, 0, Program.ScreenWidth - 10, Program.ScreenHeight - 5);
        public static readonly Rectangle ScreenRegionMessages = new Rectangle(0, ScreenRegionMap.Bottom + 1, Program.ScreenWidth - 10, Program.ScreenHeight - ScreenRegionMap.Height - 1);
        public SadConsole.Actions.ActionStack ActionProcessor;

        public bool RunLogicFrame;
        public bool RedrawMap;

        public SadConsole.Maps.MapConsole Map { get; }

        public MessageConsole Messages { get; }

        public DungeonScreen(SadConsole.Maps.MapConsole map)
        {
            // Setup map
            Map = map;
            Map.Position = ScreenRegionMap.Location;
            Map.ViewPort = new Rectangle(0, 0, ScreenRegionMap.Width, ScreenRegionMap.Height);

            // Setup actions
            ActionProcessor = new SadConsole.Actions.ActionStack();
            ActionProcessor.Push(new SadConsole.Actions.ActionDelegate(ActionKeyboardProcessor));

            // Setup messages
            Messages = new MessageConsole(ScreenRegionMessages.Width, ScreenRegionMessages.Height);
            Messages.Position = ScreenRegionMessages.Location;
            Children.Add(Messages);
            Children.Add(Map);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            // If there is a console that is focused, this one doesn't need to do anything.
            if (Global.FocusedConsoles.Console != null)
                return;

            // Can through the list of actions and pop out the finished ones
            while (ActionProcessor.Peek().IsFinished)
                ActionProcessor.Pop();

            // Run the latest action.
            ActionProcessor.Peek().Run(timeElapsed);

            // If the action finished, pop it out.
            if (ActionProcessor.Peek().IsFinished)
                ActionProcessor.Pop();

            // Center view on player
            Map.CenterViewPortOnPoint(Map.ControlledGameObject.Position);

            // Run logic if valid move made by player
            if (RunLogicFrame)
                RunGameLogicFrame();

            if (RedrawMap)
            {
                Map.IsDirty = true;
                RedrawMap = false;
            }
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
            foreach (var ent in Map.GameObjects.Entities)
                if (ent != Map.ControlledGameObject)
                    ((SadConsole.GameObjects.GameObjectBase)ent).ProcessGameFrame();

            // Process player (though it was proc in the previous loop) to make sure they are last to be processed
            Map.ControlledGameObject.ProcessGameFrame();

            // Redraw the map
            RedrawMap = true;

            RunLogicFrame = false;
        }
    }
}
