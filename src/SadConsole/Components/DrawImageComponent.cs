using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Components
{
    /// <summary>
    /// Draws an image on top of a console.
    /// </summary>
    public class DrawImageComponent : SadConsole.Components.DrawConsoleComponent, IDisposable
    {
        /// <summary>
        /// Sets or gets how the <see cref="PositionOffset"/> property is interpreted.
        /// </summary>
        public PositionModes PositionMode { get; set; } = PositionModes.Cells;

        /// <summary>
        /// The positioning offset.
        /// </summary>
        /// <remarks>
        /// The position the image is displayed is based on the hosting console's position. This property adjusts the position of the image by the specified amount.
        /// </remarks>
        public Point PositionOffset { get; set; } = new Point(0, 0);

        private Texture2D _image;
        private bool _isDisposed;

        /// <summary>
        /// Creates a new component with the specified image.
        /// </summary>
        /// <param name="filePath">Relative path to the image.</param>
        public DrawImageComponent(string filePath)
        {
            using (var stream = Microsoft.Xna.Framework.TitleContainer.OpenStream(filePath))
                _image = Texture2D.FromStream(SadConsole.Global.GraphicsDevice, stream);
        }

        /// <summary>
        /// Calls <see cref="Dispose"/>.
        /// </summary>
        ~DrawImageComponent() =>
            Dispose();

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="console">The host of the component.</param>
        /// <param name="delta">Unused.</param>
        public override void Draw(Console console, TimeSpan delta)
        {
            if (PositionMode == PositionModes.Cells)
                SadConsole.Global.DrawCalls.Add(new SadConsole.DrawCalls.DrawCallTexture(_image, (console.CalculatedPosition + console.Font.GetWorldPosition(PositionOffset)).ToVector2()));
            else
                SadConsole.Global.DrawCalls.Add(new SadConsole.DrawCalls.DrawCallTexture(_image, (console.CalculatedPosition + PositionOffset).ToVector2()));
        }

        /// <summary>
        /// Disposes the image.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
                _image.Dispose();

            _isDisposed = true;
        }

        /// <summary>
        /// Indicates how positioning is intepreted.
        /// </summary>
        public enum PositionModes
        {
            /// <summary>
            /// Positions in pixels.
            /// </summary>
            Pixels,

            /// <summary>
            /// Positions by the font of the host in cells.
            /// </summary>
            Cells
        }
    }
}
