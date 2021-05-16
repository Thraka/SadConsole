using System;
using System.Collections.Generic;
using System.Text;
using Game.Messages;
using Game.Tiles;
using GoRogue.Messaging;

namespace Game.ObjectComponents
{
    class DestroyOnPlayerTouch : IGameObjectComponent, ITileComponent, ISubscriber<Messages.Touched>
    {
        public static DestroyOnPlayerTouch Singleton { get; } = new DestroyOnPlayerTouch();

        public void Handle(Touched message)
        {
            if (message.SourceObject.HasComponent<PlayerControlled>())
            {
                if (message.TargetTile != null)
                {
                    message.Board.SetTerrain(message.TargetTile.Position, "empty", Factories.TileBlueprint.Config.Empty);
                }
                else
                {
                    message.Board.DestroyGameObject(message.TargetObject);
                }
            }
        }

        public void Added(BasicTile obj) =>
            obj.RegisterSubscriber(this);

        public void Added(GameObject obj) =>
            obj.RegisterSubscriber(this);

        public void Removed(BasicTile obj) =>
            obj.UnregisterSubscriber(this);

        public void Removed(GameObject obj) =>
            obj.UnregisterSubscriber(this);
    }
}
