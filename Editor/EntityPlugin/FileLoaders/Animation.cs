using System;
using SadConsole;
using SadConsoleEditor.FileLoaders;

namespace EntityPlugin.FileLoaders
{
    class Animation : IFileLoader
    {
        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "anim", "animation" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "Animation";
            }
        }

        public string Id => "ANIMATION";

        public object Load(string file)
        {
            return SadConsole.Serializer.Load<AnimatedScreenSurface>(file, file.EndsWith('z'));
        }

        public bool Save(object surface, string file)
        {
            try
            {
                SadConsole.Serializer.Save((AnimatedScreenSurface)surface, file, file.EndsWith('z'));

                return true;
            }
            catch (Exception e)
            {
                SadConsoleEditor.MainConsole.Instance.ShowSaveErrorPopup();
                return false;
            }
        }
    }
}
