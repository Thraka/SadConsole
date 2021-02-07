using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Settings = SadConsoleEditor.Settings;

namespace SadConsole.Game
{
    public class GameObjectMeta
    {
        public GameObject BackingObject { get; private set; }
        public bool SystemLoaded;

        public GameObjectMeta(GameObject backingObject, bool systemLoaded)
        {
            BackingObject = backingObject;
            SystemLoaded = systemLoaded;
        }

        public override string ToString()
        {
            return BackingObject.Name;
        }

        public static implicit operator GameObject(GameObjectMeta m)
        {
            return m.BackingObject;
        }
    }

    public class GameObjectListBoxItem: SadConsole.Controls.ListBoxItem
    {
        private CellAppearance _systemObjectNormal = new CellAppearance(Settings.Blue, Settings.Color_ControlBack);
        private CellAppearance _mapObjectNormal = new CellAppearance(Settings.Color_Text, Settings.Color_ControlBack);

        protected override void DetermineAppearance()
        {
            var oldAppearance = base._currentAppearance;

            if (base.Item is GameObjectMeta)
            {
                if (_isMouseOver && _isSelected)
                    base._currentAppearance = Settings.Appearance_ControlOver;
                else if (_isMouseOver)
                    base._currentAppearance = Settings.Appearance_ControlOver;
                else if (_isSelected)
                    base._currentAppearance = Settings.Appearance_ListBoxItem_SelectedItem;
                else
                {
                    if (((GameObjectMeta)Item).SystemLoaded)
                        base._currentAppearance = _systemObjectNormal;
                    else
                        base._currentAppearance = _mapObjectNormal;
                }
            }

            if (oldAppearance != base._currentAppearance)
                IsDirty = true;
        }
    }
}
