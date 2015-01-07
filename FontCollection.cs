using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Linq;

namespace SadConsole
{
    [DataContract]
    public class FontCollection
    {
        private Dictionary<string, FontBase> _defaultFonts = new Dictionary<string, FontBase>();

        [DataMember(Name = "Families")]
        private Dictionary<string, List<FontBase>> _fontFamilies = new Dictionary<string, List<FontBase>>();

        /// <summary>
        /// Adds the console font to a new list of console fonts with the specified family name. If the list already exists, the font will be added to it.
        /// </summary>
        /// <param name="familyName">The family name for the font.</param>
        /// <param name="font">The console font.</param>
        /// <param name="defaultFont">Mark this font as default for the family.</param>
        public void Add(string familyName, FontBase font, bool defaultFont)
        {
            if (_fontFamilies.ContainsKey(familyName))
            {
                var fonts = _fontFamilies[familyName];
                if (!fonts.Contains(font))
                    fonts.Add(font);
            }
            else
            {
                var fonts = new List<FontBase>();
                fonts.Add(font);
                _fontFamilies.Add(familyName, fonts);
            }

            if (defaultFont)
            {
                if (_defaultFonts.ContainsKey(familyName))
                {
                    _defaultFonts[familyName].IsDefault = false;
                    _defaultFonts.Remove(familyName);
                }

                _defaultFonts.Add(familyName, font);
            }

        }

        public void Remove(string familyName, int cellWidth, int cellHeight)
        {
            if (_fontFamilies.ContainsKey(familyName))
            {
                for (int i = 0; i < _fontFamilies[familyName].Count; i++)
                {
                    var item = _fontFamilies[familyName][i];
                    if (item.CellHeight == cellHeight && item.CellWidth == cellWidth)
                    {
                        _fontFamilies[familyName].Remove(item);
                        break;
                    }
                }
            }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<FontBase> this[string familyName]
        {
            get
            {
                if (_fontFamilies.ContainsKey(familyName))
                    return _fontFamilies[familyName].AsReadOnly();
                else
                    return new List<FontBase>().AsReadOnly();
            }
        }

        public FontBase this[string familyName, int cellWidth, int cellHeight]
        {
            get
            {
                if (_fontFamilies.ContainsKey(familyName))
                {
                    foreach (var item in _fontFamilies[familyName])
                    {
                        if (item.CellHeight == cellHeight && item.CellWidth == cellWidth)
                            return item;
                    }
                }

                return null;
            }
        }

        public FontBase GetDefaultFont(string familyName)
        {
            if (_defaultFonts.ContainsKey(familyName))
                return _defaultFonts[familyName];
            else
                return null;
        }

        [OnDeserialized]
        private void AfterDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            foreach (var family in _fontFamilies.Keys)
            {
                var fonts = _fontFamilies[family];

                foreach (var font in fonts)
                {
                    if (font.IsDefault)
                    {
                        _defaultFonts.Add(family, font);
                        break;
                    }
                }
            }
        }

        public void Save(string fontCollectionFile)
        {
            System.Runtime.Serialization.DataContractSerializer serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(FontCollection), new Type[] { typeof(FontBase), typeof(Font) });
            var stream = System.IO.File.OpenWrite(fontCollectionFile);

            serializer.WriteObject(stream, this);
            stream.Dispose();
        }

        public static FontCollection Load(string fontCollectionFile)
        {
            var file = System.IO.File.OpenRead(fontCollectionFile);
            var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(FontCollection), new Type[] { typeof(FontBase), typeof(Font) });

            return serializer.ReadObject(file) as FontCollection;
        }

        public void LoadFamily(string fontCollectionFile)
        {
            var collection = FontCollection.Load(fontCollectionFile);

            if (collection != null)
            {
                var families = collection._fontFamilies.Keys.ToList();

                foreach (var family in families)
                {
                    foreach (var font in collection[family])
                    {
                        Add(family, font, font.IsDefault);
                    }
                }
            }
        }

    }
}
