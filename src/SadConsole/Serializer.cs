namespace SadConsole
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Common serialization tasks for SadConsole.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Serializes the <paramref name="instance"/> instance to the specified file.
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="instance">The object to serialize.</param>
        /// <param name="file">The file to save the object to.</param>
        /// <param name="compress">When true, uses GZIP compression on the json string saved to the <paramref name="file"/></param>
        public static void Save<T>(T instance, string file, bool compress)
        {
            if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);

            using (var stream = System.IO.File.OpenWrite(file))
            {
                if (compress)
                {
                    using (var sw = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress))
                    {
                        //var bytes = Encoding.UTF32.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(instance, Formatting.None, new JsonSerializerSettings() { TraceWriter = LogWriter, TypeNameHandling = TypeNameHandling.All }));
                        var bytes = Encoding.UTF32.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(instance, Formatting.None, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));
                        sw.Write(bytes, 0, bytes.Length);
                    }
                }
                else
                {
                    using (var sw = new System.IO.StreamWriter(stream))
                    {
                        //sw.Write(JsonConvert.SerializeObject(instance, Formatting.Indented, new JsonSerializerSettings() { TraceWriter = LogWriter, TypeNameHandling = TypeNameHandling.All }));
                        sw.Write(JsonConvert.SerializeObject(instance, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));
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
            Global.SerializerPathHint = System.IO.Path.GetDirectoryName(file);

            using (var fileObject = Microsoft.Xna.Framework.TitleContainer.OpenStream(file))
            {
                if (isCompressed)
                {
                    using (var sw = new System.IO.Compression.GZipStream(fileObject, System.IO.Compression.CompressionMode.Decompress))
                    {
                        using (var sr = new System.IO.StreamReader(sw, Encoding.UTF32))
                        {
                            string value = sr.ReadToEnd();
                            //return (T)JsonConvert.DeserializeObject(value, typeof(T), new JsonSerializerSettings() { TraceWriter = LogWriter, TypeNameHandling = TypeNameHandling.All });
                            return (T)JsonConvert.DeserializeObject(value, typeof(T), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
                        }
                    }
                }
                else
                    using (var sr = new System.IO.StreamReader(fileObject))
                        //return (T)JsonConvert.DeserializeObject(sr.ReadToEnd(), typeof(T), new JsonSerializerSettings() { TraceWriter = LogWriter, TypeNameHandling = TypeNameHandling.All });
                        return (T)JsonConvert.DeserializeObject(sr.ReadToEnd(), typeof(T), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
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
