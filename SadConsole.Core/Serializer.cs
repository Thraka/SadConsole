namespace SadConsole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        public static IEnumerable<Type> ConsoleTypes { get; set; }

        static Serializer()
        {
            ConsoleTypes = new Type[] { typeof(Consoles.TextSurface), typeof(Consoles.TextSurfaceView), typeof(Consoles.ConsoleList), typeof(Consoles.TextSurfaceRenderer), typeof(Cell), typeof(CellAppearance), typeof(Consoles.Cursor), typeof(Consoles.CachedTextSurfaceRenderer), typeof(Consoles.LayeredTextRenderer), typeof(Consoles.LayeredTextSurface), typeof(Consoles.AnimatedTextSurface) };
        }

        /// <summary>
        /// Serializes the <paramref name="inputObject"/> instance using the specified <paramref name="output"/> stream.
        /// </summary>
        /// <typeparam name="T">The types of object to serialize</typeparam>
        /// <param name="inputObject">The object to serialize</param>
        /// <param name="output">The stream to write the serialization to.</param>
        /// <param name="knownTypes">Known types used during serialization.</param>
        public static void Serialize<T>(T inputObject, System.IO.Stream output, IEnumerable<Type> knownTypes = null)
            where T : class
        {
            List<Type> allTypes = new List<Type>();

            if (knownTypes != null)
                allTypes.AddRange(knownTypes);

            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), allTypes);
            serializer.WriteObject(output, inputObject);
        }

        /// <summary>
        /// Deserializes a new instance of <typeparamref name="T"/> using the specified <paramref name="input"/> stream.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="input">The input stream to read.</param>
        /// <param name="knownTypes">Known types used during deserialization.</param>
        /// <returns>A new object instance.</returns>
        public static T Deserialize<T>(System.IO.Stream input, IEnumerable<Type> knownTypes = null)
            where T : class
        {
            List<Type> allTypes = new List<Type>();

            if (knownTypes != null)
                allTypes.AddRange(knownTypes);

            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), allTypes);
            return serializer.ReadObject(input) as T;
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

            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer;

            if (knownTypes != null)
                serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), knownTypes);
            else
                serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));


            using (var stream = System.IO.File.OpenWrite(file))
                serializer.WriteObject(stream, instance);
        }

        /// <summary>
        /// Deserializes a new instance of <typeparamref name="T"/> from the specified file.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="file">The file to load from.</param>
        /// <param name="knownTypes">Known types used during deserialization.</param>
        /// <returns>A new object instance.</returns>
        public static T Load<T>(string file, IEnumerable<Type> knownTypes = null) where T : class
        {
            if (System.IO.File.Exists(file))
            {
                using (var fileObject = System.IO.File.OpenRead(file))
                {
                    System.Runtime.Serialization.Json.DataContractJsonSerializer serializer;

                    if (knownTypes != null)
                        serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), knownTypes);
                    else
                        serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

                    return serializer.ReadObject(fileObject) as T;
                }
            }

            throw new System.IO.FileNotFoundException("File not found.", file);
        }
    }
}


namespace SadConsole.Instructions
{
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Common serialization tasks for SadConsole.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// The types commonly used when sesrializing an <see cref="Entity"/>.
        /// </summary>
        public static IEnumerable<System.Type> InstructionTypes
        {
            get
            {
                return (new System.Type[] { typeof(InstructionBase), typeof(InstructionSet), typeof(Wait), typeof(DrawString) }).Union(SadConsole.Serializer.ConsoleTypes);
            }
        }
    }
}
