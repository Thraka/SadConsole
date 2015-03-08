using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    public class GameObject
    {
        [IgnoreDataMember]
        private string name;

        public string Name { get { return name; } set { name = value != null ? value.ToLower() : null; } }
        public CellAppearance Character { get; set; }
        public List<Setting> Settings { get; set; }
        public Point Position { get; set; }

        [IgnoreDataMember]
        public WeakReference<GameObjectCollection> Parent;

        [IgnoreDataMember]
        public int Layer;

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

        public bool HasSetting(string name)
        {
            return Settings.Where(s => s.Name == name).FirstOrDefault() != null;
        }

        public string GetSetting(string name)
        {
            return Settings.Where(s => s.Name == name).Select(s => s.Value).FirstOrDefault();
        }

        public IEnumerable<string> GetSettings(string name)
        {
            return Settings.Where(s => s.Name == name).Select(s => s.Value);
        }

        public virtual void Loaded(GameConsole console)
        {

        }

        public virtual void Process(GameConsole console)
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
