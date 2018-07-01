using FrameworkColor = Microsoft.Xna.Framework.Color;
using FrameworkPoint = Microsoft.Xna.Framework.Point;
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
using FrameworkSpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects;


namespace SadConsole
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Common serialization tasks for SadConsole.
    /// </summary>
    public static partial class Serializer
    {
        /// <summary>
        /// The types commonly used when sesrializing a basic console.
        /// </summary>
        public static IEnumerable<Type> KnownTypes { get; set; }

        static Serializer()
        {
        }

        /// <summary>
        /// Serializes the <paramref name="instance"/> instance to the specified file.
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="instance">The object to serialize.</param>
        /// <param name="file">The file to save the object to.</param>
        /// <param name="knownTypes">Optional list of known types for serialization.</param>
        public static void Save<T>(T instance, string file, IEnumerable<Type> knownTypes = null)
        {
            if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);

            //var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), knownTypes, int.MaxValue, false, new SerializerSurrogate(), false);
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), knownTypes);

            using (var stream = System.IO.File.OpenWrite(file))
            //using (var sw = new System.IO.StreamWriter(stream))
            using (var sw = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress))
            {
                var bytes = Encoding.UTF32.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.None));

                sw.Write(bytes, 0, bytes.Length);
            }

            //serializer.WriteObject(stream, instance);

        }

        /// <summary>
        /// Deserializes a new instance of <typeparamref name="T"/> from the specified file.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="file">The file to load from.</param>
        /// <param name="knownTypes">Known types used during deserialization.</param>
        /// <returns>A new object instance.</returns>
        public static T Load<T>(string file, IEnumerable<Type> knownTypes = null)
        {
            bool isCompressed = false;

            //if (System.IO.File.Exists(file))
            //{
            Global.SerializerPathHint = System.IO.Path.GetDirectoryName(file);
            using (var fileObject = Microsoft.Xna.Framework.TitleContainer.OpenStream(file))
            {
                if (isCompressed)
                {
                    using (var sw = new System.IO.Compression.GZipStream(fileObject, System.IO.Compression.CompressionMode.Decompress))
                    {

                        using (var stringStream = new System.IO.MemoryStream())
                        {
                            sw.CopyTo(stringStream);

                            using (var sr = new System.IO.StreamReader(stringStream))
                            {
                                string content = sr.ReadToEnd();

                                //using (var tr = new System.IO.StringReader(content))
                                {
                                    return (T)Newtonsoft.Json.JsonConvert.DeserializeObject(content, typeof(T));
                                    //return (T)Newtonsoft.Json.JsonSerializer.Create().Deserialize(tr, typeof(T));

                                }
                            }
                        }


                    }
                }
                else
                    using (var sr = new System.IO.StreamReader(fileObject))
                    {
                        string content = sr.ReadToEnd();

                        //using (var tr = new System.IO.StringReader(content))
                        {
                            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject(content, typeof(T));
                            //return (T)Newtonsoft.Json.JsonSerializer.Create().Deserialize(tr, typeof(T));

                        }
                    }



                //var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), knownTypes, int.MaxValue, false, new SerializerSurrogate(), false);
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), knownTypes);
                return (T)serializer.ReadObject(fileObject);
            }
            //}

            //throw new System.IO.FileNotFoundException("File not found.", file);
        }
    }
}