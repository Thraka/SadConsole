using System;
using SadConsole;
using SadConsoleEditor.FileLoaders;

namespace EntityPlugin.FileLoaders
{
    class Entity : IFileLoader
    {
        public bool SupportsLoad { get { return true; } }

        public bool SupportsSave { get { return true; } }

        public string[] Extensions
        {
            get
            {
                return new string[] { "entity", "entityz" };
            }
        }

        public string FileTypeName
        {
            get
            {
                return "Entity";
            }
        }

        public string Id => "ENTITY";

        public object Load(string file)
        {
            var entity = SadConsole.Serializer.Load<SadConsole.Entities.Entity>(file, file.EndsWith('z'));

            // Convert animations

            throw new Exception();

            return entity;
        }

        public bool Save(object surface, string file)
        {
            try
            {
                var originalEntity = (SadConsole.Entities.Entity)surface;
                var entity = new SadConsole.Entities.Entity(originalEntity.Width, originalEntity.Height);

                // Convert animations

                throw new Exception();

                SadConsole.Serializer.Save(entity, file, file.EndsWith('z'));

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
