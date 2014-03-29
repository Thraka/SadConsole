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
        public static IEnumerable<Type> ConsoleTypes
        {
            get
            {
                return (new Type[] { typeof(Consoles.Console), typeof(Consoles.CellsRenderer), typeof(Consoles.ConsoleList), typeof(CellSurface), typeof(Cell), typeof(Font) }).Union(SadConsole.Engine.RegisteredEffects);
            }
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
        /// <returns></returns>
        public static T Deserialize<T>(System.IO.Stream input, IEnumerable<Type> knownTypes = null)
            where T : class
        {
            List<Type> allTypes = new List<Type>();

            if (knownTypes != null)
                allTypes.AddRange(knownTypes);

            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), allTypes);
            return serializer.ReadObject(input) as T;
        }
    }
}

namespace SadConsole.Entities
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
        public static IEnumerable<System.Type> AnimationTypes
        {
            get
            {
                return (new System.Type[] { typeof(Animation), typeof(Entity), typeof(Frame) }).Union(SadConsole.Serializer.ConsoleTypes);
            }
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
