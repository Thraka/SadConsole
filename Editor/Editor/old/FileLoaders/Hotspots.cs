using System;
using SadConsole.Surfaces;
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
            return SadConsole.Serializer.Load<List<SadConsole.GameHelpers.Hotspot>>(file);
        }

        public void Save(object spots, string file)
        {
            SadConsole.Serializer.Save((List<SadConsole.GameHelpers.Hotspot>)spots, file);
        }
    }
}
