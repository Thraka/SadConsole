using System;
using SadConsole.Consoles;
using System.Linq;
using System.Collections.Generic;

namespace SadConsoleEditor.FileLoaders
{
    class Hotspots : IFileLoader
    {
        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "hotspots" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "List of Hotspots";
            }
        }

        public object Load(string file)
        {
            return SadConsole.Serializer.Load<List<SadConsole.Game.Hotspot>>(file);
        }

        public void Save(object spots, string file)
        {
            SadConsole.Serializer.Save((List<SadConsole.Game.Hotspot>)spots, file);
        }
    }
}
