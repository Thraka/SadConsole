using SadRogue.Primitives;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaPoint = Microsoft.Xna.Framework.Point;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Renderers;

/// <summary>
/// Renders a terminal cursor.
/// </summary>
[System.Diagnostics.DebuggerDisplay("TerminalCursor")]
public class TerminalCursorRenderStep : IRenderStep
{
    private Terminal.TerminalCursor? _cursor;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.TerminalCursor;

    /// <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.TerminalCursor;

    /// <summary>
    /// Sets the <see cref="Terminal.TerminalCursor"/> to render.
    /// </summary>
    /// <param name="data">A <see cref="Terminal.TerminalCursor"/> instance.</param>
    public void SetData(object data)
    {
        if (data is Terminal.TerminalCursor cursor)
            _cursor = cursor;
        else
            _cursor = null;
    }

    ///  <inheritdoc/>
    public void Reset()
    {
    }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
    {
        if (_cursor != null && _cursor.RenderCellActiveState.IsDirty)
            return true;

        return false;
    }

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject)
    {
        if (_cursor == null || !_cursor.IsVisible || !_cursor.IsBlinkVisible)
            return;

        if (!screenObject.Surface.IsValidCell(_cursor.Position.X, _cursor.Position.Y) || !screenObject.Surface.View.Contains(_cursor.Position))
            return;

        XnaRectangle rect = screenObject.Font.GetRenderRect(_cursor.Position.X - screenObject.Surface.ViewPosition.X,
                                                            _cursor.Position.Y - screenObject.Surface.ViewPosition.Y,
                                                            screenObject.FontSize).ToMonoRectangle();

        Host.Global.SharedSpriteBatch.Draw(((Host.GameTexture)screenObject.Font.Image).Texture, rect,
                                           screenObject.Font.GetGlyphSourceRectangle(_cursor.RenderCellActiveState.Glyph).ToMonoRectangle(),
                                           _cursor.RenderCellActiveState.Foreground.ToMonoColor(),
                                           0f, Microsoft.Xna.Framework.Vector2.Zero, SpriteEffects.None, 0.2f);
    }

    ///  <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject) { }

    ///  <inheritdoc/>
    public void Dispose() =>
        Reset();
}
