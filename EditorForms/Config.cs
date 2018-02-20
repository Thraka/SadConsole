using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor
{
    public class Config
    {
        public static Config Instance = new Config();

        /// <summary>
        /// The clear color outside of SadConsole when in edit mode.
        /// </summary>
        public Color EditClearColorOuter = Color.LightSlateGray;

        /// <summary>
        /// The clear color inside of SadConsole when in edit mode.
        /// </summary>
        public Color EditClearColorInner = Color.DarkSlateGray;

        /// <summary>
        /// Draws a border around the <see cref="Global.RenderOutput"/> object during drawing.
        /// </summary>
        public bool DrawOuterBorder = true;

        /// <summary>
        /// Uses the <see cref="EditClearColorOuter"/> color when in edit mode.
        /// </summary>
        public bool DrawEditClearColorOuter = true;

        /// <summary>
        /// Uses the <see cref="EditClearColorInner"/> color when in edit mode.
        /// </summary>
        public bool DrawEditClearColorInner = true;

        public static void Load()
        {
            if (System.IO.File.Exists("editor.json"))
                Instance = SadConsole.Serializer.Load<Config>("editor.json");
        }

        public static void Save()
        {
            SadConsole.Serializer.Save(Instance, "editor.json");
        }

        public static void Create()
        {
            if (!System.IO.File.Exists("editor.json"))
                Save();
        }
    }
}
