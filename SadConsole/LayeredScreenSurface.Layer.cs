using SadRogue.Primitives;

namespace SadConsole
{
    public partial class LayeredScreenSurface
    {
        /// <summary>
        /// Represents a <see cref="ICellSurface"/> with position, visibility, and a friendly-name.
        /// </summary>
        public class Layer: ScreenSurface
        {
            /// <summary>
            /// A friendly-name for the layer.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Internal use only. A flag used by renderers.
            /// </summary>
            public bool HasPositionChanged { get; set; }

            public Layer(string name, int width, int height) : base(width, height) =>
                Name = name;

            public Layer(string name, int width, int height, ColoredGlyph[] initialCells) : base(width, height, initialCells) =>
                Name = name;

            public Layer(string name, ICellSurface surface, Font font = null, Point? fontSize = null) : base(surface, font, fontSize) =>
                Name = name;

            public Layer(string name, int width, int height, int bufferWidth, int bufferHeight) : base(width, height, bufferWidth, bufferHeight) =>
                Name = name;

            public Layer(string name, int width, int height, int bufferWidth, int bufferHeight, ColoredGlyph[] initialCells) : base(width, height, bufferWidth, bufferHeight, initialCells) =>
                Name = name;

            /// <inheritdoc/>
            protected override void OnPositionChanged(Point oldPosition, Point newPosition)
            {
                base.OnPositionChanged(oldPosition, newPosition);
                HasPositionChanged = true;
            }
        }
    }
}
