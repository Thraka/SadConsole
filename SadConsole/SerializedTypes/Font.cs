using System.Runtime.Serialization;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class FontSerialized
    {
        [DataMember]
        public string Name;

        [DataMember]
        public Font.FontSizes Size;

        public static implicit operator FontSerialized(Font font) => new FontSerialized() { Name = font.Name, Size = font.SizeMultiple };

        public static implicit operator Font(FontSerialized font) => GameHost.Instance.Fonts.ContainsKey(font.Name) ? GameHost.Instance.Fonts[font.Name].GetFont(font.Size)
                                                       : GameHost.Instance.DefaultFont;
    }
}
