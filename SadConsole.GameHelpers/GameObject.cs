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

        /// <summary>
        /// The name of the game object.
        /// </summary>
        public string Name { get { return name; } set { name = value != null ? value.ToLower() : null; } }

        /// <summary>
        /// How the game object looks in debug mode.
        /// </summary>
        public CellAppearance Appearance { get; set; }

        /// <summary>
        /// Key-value pairs.
        /// </summary>
        public List<Setting> Settings { get; set; }

        /// <summary>
        /// The area of the game object. Can be a single point with a width and height of 1.
        /// </summary>
        public Rectangle Area { get; set; }

        /// <summary>
        /// When true, it means the <see cref="Area"/> has a width and height of 1 and the game object can be treated as a single positioned item.
        /// </summary>
        public bool IsPoint { get { return Area.Width == 1 && Area.Height == 1; } }

        /// <summary>
        /// Creates a new game object.
        /// </summary>
        public GameObject()
        {
            Settings = new List<Setting>();
            Appearance = new CellAppearance();
            Appearance.GlyphIndex = 1;
            Name = "New";
        }

        /// <summary>
        /// Returns a brand new copy of this object.
        /// </summary>
        /// <returns>A new game object.</returns>
        public GameObject Clone()
        {
            var newObject = new GameObject();
            CopyTo(newObject);
            return newObject;
        }

        /// <summary>
        /// Copies the values of this game object into the <paramref name="destination"/>.
        /// </summary>
        /// <param name="destination">The target game object to copy this one in to.</param>
        public void CopyTo(GameObject destination)
        {
            destination.Name = Name;
            destination.Appearance = Appearance.Clone();
            destination.Settings = new List<Setting>(Settings.Count);
            destination.Area = Area;

            foreach (var item in Settings)
            {
                destination.Settings.Add(new Setting() { Name = item.Name, Value = item.Value });
            }
        }

        /// <summary>
        /// Returns true when this object has a setting with the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <returns>True when the setting exists; otherwise false.</returns>
        public bool HasSetting(string name)
        {
            return Settings.Where(s => s.Name == name).FirstOrDefault() != null;
        }

        /// <summary>
        /// Gets a setting value by name.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <returns>The value of the setting.</returns>
        public string GetSetting(string name)
        {
            return Settings.Where(s => s.Name == name).Select(s => s.Value).FirstOrDefault();
        }

        /// <summary>
        /// Returns all setting values.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <returns>The setting values.</returns>
        public IEnumerable<string> GetSettings(string name)
        {
            return Settings.Where(s => s.Name == name).Select(s => s.Value);
        }

        /// <summary>
        /// Called when game object is loaded.
        /// </summary>
        /// <param name="surface">The surface that owns this object.</param>
        public virtual void Loaded(GameObjectTextSurface surface)
        {

        }

        /// <summary>
        /// Called when game object is .
        /// </summary>
        /// <param name="surface">The surface that owns this object.</param>
        public virtual void Process(GameObjectTextSurface surface)
        {

        }

        /// <summary>
        /// The name of the game object.
        /// </summary>
        /// <returns>The name.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
