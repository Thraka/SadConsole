using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SadConsole.SerializedTypes;

namespace SadConsole.Surfaces
{
    /// <summary>
    /// Represents mutliple surfaces grouped together and rendered at the same time.
    /// </summary>
    /// <inheritdoc cref="IList"/>
    [JsonConverter(typeof(LayeredJsonConverter))]
    [System.Diagnostics.DebuggerDisplay("Layered Surface")]
    public class Layered : ScreenObject, IList<Basic>
    {
        private readonly IList<Basic> _backingList;
        public Basic this[int index]
        {
            get => _backingList[index];
            set => _backingList[index] = value;
        }

        public Layered()
        {
            _backingList = new List<Basic>();
        }

        public IEnumerator<Basic> GetEnumerator()
        {
            return _backingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            foreach (var surface in _backingList)
            {
                surface.Position = this.CalculatedPosition;
                surface.Draw(timeElapsed);
            }

            base.Draw(timeElapsed);
        }

        public void Add(Basic item)
        {
            _backingList.Add(item);
        }

        public void Clear()
        {
            _backingList.Clear();
        }

        public bool Contains(Basic item)
        {
            return _backingList.Contains(item);
        }

        public void CopyTo(Basic[] array, int arrayIndex)
        {
            _backingList.CopyTo(array, arrayIndex);
        }

        public bool Remove(Basic item)
        {
            return _backingList.Remove(item);
        }

        public int IndexOf(Basic item)
        {
            return _backingList.IndexOf(item);
        }

        public void Insert(int index, Basic item)
        {
            _backingList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _backingList.RemoveAt(index);
        }

        public int Count => _backingList.Count;

        public bool IsReadOnly => _backingList.IsReadOnly;

        /// <summary>
        /// Saves the <see cref="SurfaceBase"/> to a file.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file) => Serializer.Save(this, file, Settings.SerializationIsCompressed);

        /// <summary>
        /// Loads a <see cref="SurfaceBase"/> from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <returns></returns>
        public static Layered Load(string file) => Serializer.Load<Layered>(file, Settings.SerializationIsCompressed);
    }
}
