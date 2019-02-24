using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Entities;

namespace SadConsole.Actions
{
    class Move : ActionBase
    {
        public static Move MoveBy(GameObjects.GameObjectBase source, Point change, Maps.MapConsole map)
        {
            return new Move() { Source = source, PositionChange = change, Map = map };
        }

        public Maps.MapConsole Map;
        public GameObjects.GameObjectBase Source;
        public Point PositionChange;
        public Point TargetPosition;

        public override void Run(TimeSpan timeElapsed)
        {
            TargetPosition = Source.Position + PositionChange;

            if (TargetPosition == Source.Position)
                return;

            if (Map.IsTileWalkable(TargetPosition.X, TargetPosition.Y))
            {
                var ents = Map.GameObjects.GetEntities(TargetPosition).ToArray();

                if (ents.Length == 0)
                {
                    Source.MoveBy(PositionChange);

                    if (Source == Map.ControlledGameObject)
                    {
                        if (PositionChange == Directions.West)
                            BasicTutorial.GameState.Dungeon.Messages.Print("You move west.", BasicTutorial.MessageConsole.MessageTypes.Status);
                        else if (PositionChange == Directions.East)
                            BasicTutorial.GameState.Dungeon.Messages.Print("You move east.", BasicTutorial.MessageConsole.MessageTypes.Status);
                        else if (PositionChange == Directions.North)
                            BasicTutorial.GameState.Dungeon.Messages.Print("You move north.", BasicTutorial.MessageConsole.MessageTypes.Status);
                        else if (PositionChange == Directions.South)
                            BasicTutorial.GameState.Dungeon.Messages.Print("You move south.", BasicTutorial.MessageConsole.MessageTypes.Status);
                    }
                }
                else
                {
                    foreach (var item in ents)
                    {
                        BumpGameObject bump = new BumpGameObject(Source, (GameObjects.GameObjectBase)item);
                        BasicTutorial.GameState.Dungeon.ActionProcessor.PushAndRun(bump);
                    }
                }
            }
            else
            {
                BumpTile bump = new BumpTile(Source, Map[TargetPosition]);
                BasicTutorial.GameState.Dungeon.ActionProcessor.PushAndRun(bump);
            }

            Finish(ActionResult.Success);
        }
    }

}
