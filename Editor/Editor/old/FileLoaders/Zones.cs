using System;
using SadConsole.Surfaces;
using System.Linq;
using System.Collections.Generic;

namespace SadConsoleEditor.FileLoaders
{
    class Zones : IFileLoader
    {
        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "zones" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "List of Zones";
            }
        }

        public object Load(string file)
        {
            return SadConsole.Serializer.Load<List<SadConsole.GameHelpers.Zone>>(file);
        }

        public void Save(object spots, string file)
        {
            SadConsole.Serializer.Save((List<SadConsole.GameHelpers.Zone>)spots, file);
        }
    }
}
