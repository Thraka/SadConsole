using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsoleEditor.FileLoaders
{
    class Text: IFileLoader
    {
        public bool SupportsLoad => true;

        public bool SupportsSave => false;

        public string[] Extensions
        {
            get
            {
                return new string[] { "txt" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "Text";
            }
        }

        public string Id => "TEXT";

        public object Load(string file)
        {
            throw new NotImplementedException();
        }

        public bool Save(object surface, string file)
        {
            throw new NotSupportedException();
        }
    }
}
