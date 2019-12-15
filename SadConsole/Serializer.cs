using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SadConsole
{
    /// <summary>
    /// Common serialization tasks for SadConsole.
    /// </summary>
    public static class Serializer
    {
        private static JsonSerializerSettings _settings;


        static Serializer()
        {
            _settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            _settings.ContractResolver = new Contracts();
        }


        private class Contracts: DefaultContractResolver
        {
            protected override JsonContract CreateContract(Type objectType)
            {
                JsonContract contract = base.CreateContract(objectType);

                if (objectType == typeof(SadRogue.Primitives.Rectangle))
                    contract.Converter = new SadConsole.SerializedTypes.RectangleJsonConverter();

                else if (objectType == typeof(SadRogue.Primitives.BoundedRectangle))
                    contract.Converter = new SadConsole.SerializedTypes.BoundedRectangleJsonConverter();

                else if (objectType == typeof(SadRogue.Primitives.Color))
                    contract.Converter = new SadConsole.SerializedTypes.ColorJsonConverter();

                else if (objectType == typeof(Font))
                    contract.Converter = new SerializedTypes.FontJsonConverter();

                return contract;
            }
        }


        /// <summary>
        /// The settings to use during <see cref="Save{T}(T, string, bool)"/> and <see cref="Load{T}(string, bool)"/>.
        /// </summary>
        public static JsonSerializerSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
            }
        }

        /// <summary>
        /// Serializes the <paramref name="instance"/> instance to the specified file.
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="instance">The object to serialize.</param>
        /// <param name="file">The file to save the object to.</param>
        /// <param name="compress">When true, uses GZIP compression on the json string saved to the <paramref name="file"/></param>
        public static void Save<T>(T instance, string file, bool compress)
        {
            if (GameHost.Instance.FileExists(file))
                GameHost.Instance.FileDelete(file);

            using (System.IO.Stream stream = GameHost.Instance.OpenStream(file, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
            {
                if (compress)
                {
                    using (var sw = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress))
                    {
                        //var bytes = Encoding.UTF32.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(instance, Formatting.None, new JsonSerializerSettings() { TraceWriter = LogWriter, TypeNameHandling = TypeNameHandling.All }));
                        byte[] bytes = Encoding.UTF32.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(instance, Formatting.None, _settings));
                        sw.Write(bytes, 0, bytes.Length);
                    }
                }
                else
                {
                    using (var sw = new System.IO.StreamWriter(stream))
                    {
                        //sw.Write(JsonConvert.SerializeObject(instance, Formatting.Indented, new JsonSerializerSettings() { TraceWriter = LogWriter, TypeNameHandling = TypeNameHandling.All }));
                        sw.Write(JsonConvert.SerializeObject(instance, Formatting.Indented, _settings));
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes a new instance of <typeparamref name="T"/> from the specified file.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="file">The file to load from.</param>
        /// <param name="isCompressed">When true, indicates that the json <paramref name="file"/> should be decompressed with GZIP compression.</param>
        /// <returns>A new object instance.</returns>
        public static T Load<T>(string file, bool isCompressed)
        {
            SadConsole.GameHost.SerializerPathHint = System.IO.Path.GetDirectoryName(file);

            using (System.IO.Stream fileObject = GameHost.Instance.OpenStream(file))
            {
                if (isCompressed)
                {
                    using (var sw = new System.IO.Compression.GZipStream(fileObject, System.IO.Compression.CompressionMode.Decompress))
                    {
                        using (var sr = new System.IO.StreamReader(sw, Encoding.UTF32))
                        {
                            string value = sr.ReadToEnd();
                            //return (T)JsonConvert.DeserializeObject(value, typeof(T), new JsonSerializerSettings() { TraceWriter = LogWriter, TypeNameHandling = TypeNameHandling.All });
                            return (T)JsonConvert.DeserializeObject(value, typeof(T), _settings);
                        }
                    }
                }
                else
                {
                    using (var sr = new System.IO.StreamReader(fileObject))
                    {
                        //return (T)JsonConvert.DeserializeObject(sr.ReadToEnd(), typeof(T), new JsonSerializerSettings() { TraceWriter = LogWriter, TypeNameHandling = TypeNameHandling.All });
                        return (T)JsonConvert.DeserializeObject(sr.ReadToEnd(), typeof(T), _settings);
                    }
                }
            }
        }

        public static ITraceWriter LogWriter = new MemoryTraceWriter();

        internal class LogTraceWriter : ITraceWriter
        {
            internal static readonly StringBuilder Log = new StringBuilder();

            private TraceLevel _levelFilter;

            public void Trace(TraceLevel level, string message, Exception ex)
            {
#if DEBUG
                _levelFilter = level;
                LogTraceWriter.Log.AppendLine($"{Enum.GetName(typeof(TraceLevel), level)} :: {message}");
#endif
            }

            public TraceLevel LevelFilter => _levelFilter;
        }
    }
}
