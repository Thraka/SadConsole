using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    public class GameObject
    {
        public string Name { get; set; }
        public CellAppearance Character { get; set; }
        public List<Setting> Settings { get; set; }
        public Point Position { get; set; }

        public GameObject()
        {
            Settings = new List<Setting>();
            Character = new CellAppearance();
            Character.CharacterIndex = 1;
            Name = "New";
        }
        
        public GameObject Clone()
        {
            var newObject = new GameObject();
            newObject.Name = Name;
            newObject.Character = Character.Clone();
            newObject.Settings = new List<Setting>(Settings.Count);
            newObject.Position = Position;

            foreach (var item in Settings)
            {
                newObject.Settings.Add(new Setting() { Name = item.Name, Value = item.Value });
            }

            return newObject;
        }

        public void CopyTo(GameObject destination)
        {
            destination.Name = Name;
            destination.Character = Character.Clone();
            destination.Settings = new List<Setting>(Settings.Count);
            destination.Position = Position;

            foreach (var item in Settings)
            {
                destination.Settings.Add(new Setting() { Name = item.Name, Value = item.Value });
            }

        }


        public virtual void Loaded(GameObjectCollection parent, Consoles.Console console, IEnumerable<GameObjectCollection> otherCollections = null)
        {

        }

        public virtual void Process(GameObjectCollection parent, Consoles.Console console, IEnumerable<GameObjectCollection> otherCollections = null)
        {

        }
    }
}
