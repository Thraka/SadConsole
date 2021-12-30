using System;
using System.Collections.Generic;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Components
{
    /// <summary>
    /// Draws an image on top of a console.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Image")]
    public class DrawImage : RenderComponent, IDisposable
    {
        private bool _hasFont;
        private PositionModes _positionMode = PositionModes.Pixels;

        /// <summary>
        /// Sets or gets how the <see cref="PositionOffset"/> property is interpreted.
        /// </summary>
        public PositionModes PositionMode
        {
            get => _positionMode;
            set
            {
                _positionMode = value;

                if (!_hasFont && value == PositionModes.Cells)
                    throw new Exception("Cannot set mode to Cells, component host doesn't have a font. (Host isn't an IScreenSurface)");
            }
        }

        /// <summary>
        /// The positioning offset.
        /// </summary>
        /// <remarks>
        /// The position the image is displayed is based on the hosting console's position. This property adjusts the position of the image by the specified amount.
        /// </remarks>
        public Point PositionOffset { get; set; } = new Point(0, 0);

        private readonly Host.GameTexture _image;
        private bool _isDisposed;

        /// <summary>
        /// Creates a new component with the specified image.
        /// </summary>
        /// <param name="filePath">Relative path to the image.</param>
        public DrawImage(string filePath) =>
            _image = (Host.GameTexture)SadConsole.GameHost.Instance.GetTexture(filePath);

        /// <summary>
        /// Calls <see cref="Dispose"/>.
        /// </summary>
        ~DrawImage() =>
            Dispose();

        /// <inheritdoc/>
        public override void OnAdded(IScreenObject host)
        {
            _hasFont = host is IScreenSurface;

            if (!_hasFont && PositionMode == PositionModes.Cells)
                throw new Exception("Position mode on component is Cells, but component host doesn't have a font. (Host isn't an IScreenSurface)");
        }

        /// <inheritdoc/>
        public override void OnRemoved(IScreenObject host)
        {
            // Always default to true so that the position mode can be set to anything.
            _hasFont = true;
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="host">The host of the component.</param>
        /// <param name="delta">Unused.</param>
        public override void Render(IScreenObject host, TimeSpan delta)
        {
            if (PositionMode == PositionModes.Cells)
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(_image.Texture, (host.AbsolutePosition + ((IScreenSurface)host).Font.GetWorldPosition(PositionOffset, ((IScreenSurface)host).FontSize)).ToMonoPoint().ToVector2()));
            else
                GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallTexture(_image.Texture, (host.AbsolutePosition + PositionOffset).ToMonoPoint().ToVector2()));
        }

        /// <summary>
        /// Disposes the image.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _image.Dispose();
                GC.SuppressFinalize(this);
            }

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
