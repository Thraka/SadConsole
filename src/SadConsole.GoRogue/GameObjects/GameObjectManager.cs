using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue;
using SadConsole.Maps;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Point = Microsoft.Xna.Framework.Point;

namespace SadConsole.GameObjects
{
    public class GameObjectManager : SpatialMap<GameObjects.GameObjectBase>
    {
        SimpleMap map;
        Rectangle cachedViewPort;

        protected internal GameObjectManager(SimpleMap map)
        {
            this.map = map;
            this.cachedViewPort = map.Surface.ViewPort;
            this.ItemAdded += GameObjectManager_ItemAdded;
            this.ItemRemoved += GameObjectManager_ItemRemoved;
            this.ItemMoved += GameObjectManager_ItemMoved;
        }

        private void GameObjectManager_ItemMoved(object sender, ItemMovedEventArgs<GameObjects.GameObjectBase> e)
        {
            e.Item.Position = e.NewPosition.ToPoint();
            e.Item.MoveTo(e.NewPosition.ToPoint());
            e.Item.IsVisible = map.Surface.ViewPort.Contains(e.Item.Position);
        }

        private void GameObjectManager_ItemRemoved(object sender, ItemEventArgs<GameObjects.GameObjectBase> e)
        {
        }

        private void GameObjectManager_ItemAdded(object sender, ItemEventArgs<GameObjects.GameObjectBase> e)
        {
            e.Item.Position = e.Position.ToPoint();
            e.Item.IsVisible = map.Surface.ViewPort.Contains(e.Item.Position);
        }

        public void SyncView()
        {
            if (cachedViewPort == map.Surface.ViewPort) return;

            cachedViewPort = map.Surface.ViewPort;

            foreach (var item in Items)
            {
                item.PositionOffset = new Point(-map.Surface.ViewPort.Location.X, -map.Surface.ViewPort.Location.Y);
                item.IsVisible = map.Surface.ViewPort.Contains(item.Position);
            }
        }
    }
}
