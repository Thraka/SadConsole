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
        IConsoleViewPort _viewPortConsole;
        Rectangle _cachedViewPort;

        protected internal GameObjectManager(IConsoleViewPort viewPortConsole)
        {
            this._viewPortConsole = viewPortConsole;
            this._cachedViewPort = viewPortConsole.ViewPort;
            this.ItemAdded += GameObjectManager_ItemAdded;
            this.ItemRemoved += GameObjectManager_ItemRemoved;
            this.ItemMoved += GameObjectManager_ItemMoved;
        }

        private void GameObjectManager_ItemMoved(object sender, ItemMovedEventArgs<GameObjects.GameObjectBase> e)
        {
            e.Item.Position = e.NewPosition.ToPoint();
            e.Item.IsVisible = _viewPortConsole.ViewPort.Contains(e.Item.Position);
        }

        private void GameObjectManager_ItemRemoved(object sender, ItemEventArgs<GameObjects.GameObjectBase> e)
        {
        }

        private void GameObjectManager_ItemAdded(object sender, ItemEventArgs<GameObjects.GameObjectBase> e)
        {
            e.Item.Position = e.Position.ToPoint();
            e.Item.IsVisible = _viewPortConsole.ViewPort.Contains(e.Item.Position);
        }

        public void SyncView()
        {
            if (_cachedViewPort == _viewPortConsole.ViewPort) return;

            _cachedViewPort = _viewPortConsole.ViewPort;

            foreach (var item in Items)
            {
                item.PositionOffset = new Point(-_viewPortConsole.ViewPort.Location.X, -_viewPortConsole.ViewPort.Location.Y);
                item.IsVisible = _viewPortConsole.ViewPort.Contains(item.Position);
            }
        }
    }
}
