namespace SadConsole.SerializedTypes
{
    using System.Runtime.Serialization;

    [DataContract]
    public class FontSerialized
    {
        [DataMember]
        public string Name;

        [DataMember]
        public Font.FontSizes Size;

        public static implicit operator FontSerialized(Font font) => new FontSerialized() { Name = font.Name, Size = font.SizeMultiple };

        public static implicit operator Font(FontSerialized font) => Global.Fonts.ContainsKey(font.Name) ? Global.Fonts[font.Name].GetFont(font.Size)
                                                       : Global.FontDefault;
    }
}
