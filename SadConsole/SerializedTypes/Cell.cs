using System.Linq;
using System.Runtime.Serialization;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class CellSerialized
    {
        [DataMember]
        public CellDecorator[] Decorators;

        [DataMember]
        public ColorSerialized Foreground;

        [DataMember]
        public ColorSerialized Background;

        [DataMember]
        public int Glyph;

        [DataMember]
        public Mirror Mirror;

        [DataMember]
        public bool IsVisible;

        public static implicit operator CellSerialized(ColoredGlyph cell) => new CellSerialized()
        {
            Foreground = cell.Foreground,
            Background = cell.Background,
            Glyph = cell.Glyph,
            IsVisible = cell.IsVisible,
            Mirror = cell.Mirror,
            Decorators = cell.Decorators
        };

        public static implicit operator ColoredGlyph(CellSerialized cell)
        {
            var newCell = new ColoredGlyph(cell.Foreground, cell.Background, cell.Glyph, cell.Mirror)
            {
                IsVisible = cell.IsVisible,
                Decorators = cell.Decorators.Length != 0 ? cell.Decorators.ToArray() : System.Array.Empty<CellDecorator>()
            };

            return newCell;
        }
    }
}
