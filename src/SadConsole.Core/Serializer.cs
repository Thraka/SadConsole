#if SFML
using FrameworkColor = SFML.Graphics.Color;
using FrameworkPoint = SFML.System.Vector2i;
using FrameworkRect = SFML.Graphics.IntRect;
#elif MONOGAME
using FrameworkColor = Microsoft.Xna.Framework.Color;
using FrameworkPoint = Microsoft.Xna.Framework.Point;
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
using FrameworkSpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects;
#endif


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
        public static IEnumerable<Type> ConsoleTypes { get; set; }

        static Serializer()
        {
            ConsoleTypes = new Type[] { typeof(Consoles.TextSurface), typeof(Consoles.TextSurfaceView), typeof(Consoles.ConsoleList), typeof(Consoles.TextSurfaceRenderer), typeof(Cell), typeof(CellAppearance), typeof(Consoles.Cursor), typeof(Consoles.LayeredTextSurface) };//typeof(Consoles.CachedTextSurfaceRenderer), typeof(Consoles.LayeredTextRenderer) };
        }

        ///// <summary>
        ///// Serializes the <paramref name="inputObject"/> instance using the specified <paramref name="output"/> stream.
        ///// </summary>
        ///// <typeparam name="T">The types of object to serialize</typeparam>
        ///// <param name="inputObject">The object to serialize</param>
        ///// <param name="output">The stream to write the serialization to.</param>
        ///// <param name="knownTypes">Known types used during serialization.</param>
        //public static void Serialize<T>(T inputObject, System.IO.Stream output, IEnumerable<Type> knownTypes = null)
        //    where T : class
        //{
        //    List<Type> allTypes = new List<Type>();

        //    if (knownTypes != null)
        //        allTypes.AddRange(knownTypes);

        //    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), allTypes);
        //    serializer.WriteObject(output, inputObject);
        //}

        ///// <summary>
        ///// Deserializes a new instance of <typeparamref name="T"/> using the specified <paramref name="input"/> stream.
        ///// </summary>
        ///// <typeparam name="T">The type of object to deserialize.</typeparam>
        ///// <param name="input">The input stream to read.</param>
        ///// <param name="knownTypes">Known types used during deserialization.</param>
        ///// <returns>A new object instance.</returns>
        //public static T Deserialize<T>(System.IO.Stream input, IEnumerable<Type> knownTypes = null)
        //    where T : class
        //{
        //    List<Type> allTypes = new List<Type>();

        //    if (knownTypes != null)
        //        allTypes.AddRange(knownTypes);

        //    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), allTypes);
        //    return serializer.ReadObject(input) as T;
        //}

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

            System.Runtime.Serialization.Json.DataContractJsonSerializerSettings settings = new System.Runtime.Serialization.Json.DataContractJsonSerializerSettings();
            settings.DataContractSurrogate = new SerializerSurrogate();

            if (knownTypes != null)
                settings.KnownTypes = knownTypes;

            //if (knownTypes != null)
            //    serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), knownTypes,);
            //else
            //    serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

            //serializer.DataContractSurrogate = new SerializerSurrogate();

            serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), settings);

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
        public static T Load<T>(string file, IEnumerable<Type> knownTypes = null)
        {
            if (System.IO.File.Exists(file))
            {
                using (var fileObject = System.IO.File.OpenRead(file))
                {
                    System.Runtime.Serialization.Json.DataContractJsonSerializer serializer;

                    System.Runtime.Serialization.Json.DataContractJsonSerializerSettings settings = new System.Runtime.Serialization.Json.DataContractJsonSerializerSettings();
                    settings.DataContractSurrogate = new SerializerSurrogate();

                    if (knownTypes != null)
                        settings.KnownTypes = knownTypes;

                    //if (knownTypes != null)
                    //    serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), knownTypes,);
                    //else
                    //    serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

                    //serializer.DataContractSurrogate = new SerializerSurrogate();

                    serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), settings);

                    //if (knownTypes != null)
                    //    serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T), knownTypes);
                    //else
                    //    serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

                    return (T)serializer.ReadObject(fileObject);
                }
            }

            throw new System.IO.FileNotFoundException("File not found.", file);
        }
    }

    

    internal class SerializerSurrogate : IDataContractSurrogate
    {
        public Type GetDataContractType(Type type)
        {
            if (type == typeof(FrameworkColor))
                return typeof(SerializedTypes.Color);

            else if (type == typeof(Font))
                return typeof(SerializedTypes.Font);

            else if (type == typeof(FrameworkPoint))
                return typeof(SerializedTypes.Point);

            else if (type == typeof(FrameworkRect))
                return typeof(SerializedTypes.Rectangle);

#if MONOGAME
            else if (type == typeof(FrameworkSpriteEffect))
                return typeof(SerializedTypes.SpriteEffects);
#endif

            return type;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (obj is FrameworkColor)
                return SerializedTypes.Color.FromFramework((FrameworkColor)obj);

            else if (obj is Font)
                return SerializedTypes.Font.FromFramework((Font)obj);

            else if (obj is FrameworkPoint)
                return SerializedTypes.Point.FromFramework((FrameworkPoint)obj);

            else if (obj is FrameworkRect)
                return SerializedTypes.Rectangle.FromFramework((FrameworkRect)obj);

#if MONOGAME
            else if (obj is FrameworkSpriteEffect)
                return (SerializedTypes.SpriteEffects)obj;
#endif

            return obj;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (obj is SerializedTypes.Color)
                return SerializedTypes.Color.ToFramework((SerializedTypes.Color)obj);

            else if (obj is SerializedTypes.Font)
                return SerializedTypes.Font.ToFramework((SerializedTypes.Font)obj);

            else if (obj is SerializedTypes.Point)
                return SerializedTypes.Point.ToFramework((SerializedTypes.Point)obj);

            else if (obj is SerializedTypes.Rectangle)
                return SerializedTypes.Rectangle.ToFramework((SerializedTypes.Rectangle)obj);

#if MONOGAME
            else if (obj is SerializedTypes.SpriteEffects)
                return (FrameworkSpriteEffect)obj;
#endif

            return obj;
        }


        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
            
        }
        
        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            return null;
        }

        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            return null;
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
