using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.GameHelpers
{
    public class GameObjectCollection : Dictionary<Point, GameObject>
    {
        [IgnoreDataMember]
        public List<Trigger> Triggers;

        [IgnoreDataMember]
        public List<Processor> Processors;

        [IgnoreDataMember]
        public List<Action> Actions;

        public virtual void ParseAllObjects()
        {
            Triggers = new List<Trigger>();
            Processors = new List<Processor>();
            Actions = new List<Action>();

            List<Tuple<Point, GameObject>> transformedObjects = new List<Tuple<Point, GameObject>>();

            foreach (var item in base.Keys)
            {
                var newObject = GameObjectParser.Parse(this[item]);

                if (newObject != this[item])
                    transformedObjects.Add(new Tuple<Point, GameObject>(item, newObject));
            }

            foreach (var item in transformedObjects)
                this[item.Item1] = item.Item2;
        }

        protected virtual void SortObject(GameObject gameObject)
        {
            if (gameObject is Trigger)
                Triggers.Add((Trigger)gameObject);

            else if (gameObject is Processor)
                Processors.Add((Processor)gameObject);

            else if (gameObject is Action)
                Actions.Add((Action)gameObject);
        }

        public static void Save(GameObjectCollection instance, string file)
        {
            if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);

            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GameObjectCollection));
            using (var stream = System.IO.File.OpenWrite(file))
            {
                serializer.WriteObject(stream, instance);
            }
        }

        public static GameObjectCollection Load(string file)
        {
            if (System.IO.File.Exists(file))
            {
                using (var fileObject = System.IO.File.OpenRead(file))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GameObjectCollection), new Type[] { typeof(GameObject) });

                    var collection = serializer.ReadObject(fileObject) as GameObjectCollection;

                    foreach (var item in collection)
                        item.Value.Parent = new WeakReference<GameObjectCollection>(collection);
                }
            }

            throw new System.IO.FileNotFoundException("File not found.", file);
        }

        public static void SaveCollection(IEnumerable<GameObjectCollection> objects, string file)
        {
            if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);

            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(IEnumerable<GameObjectCollection>));
            using (var stream = System.IO.File.OpenWrite(file))
            {
                serializer.WriteObject(stream, objects);
            }
        }

        public static IEnumerable<GameObjectCollection> LoadCollection(string file)
        {
            if (System.IO.File.Exists(file))
            {
                using (var fileObject = System.IO.File.OpenRead(file))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(IEnumerable<GameObjectCollection>), new Type[] { typeof(GameObjectCollection), typeof(GameHelpers.GameObject) });

                    var collections = serializer.ReadObject(fileObject) as IEnumerable<GameObjectCollection>;

                    foreach (var collection in collections)
                        foreach (var item in collection)
                            item.Value.Parent = new WeakReference<GameObjectCollection>(collection);

                    return collections;
                }
            }

            throw new System.IO.FileNotFoundException("File not found.", file);
        }
    }
}
