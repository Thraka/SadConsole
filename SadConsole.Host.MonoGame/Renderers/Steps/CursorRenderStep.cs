using System;
using SadRogue.Primitives;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaPoint = Microsoft.Xna.Framework.Point;

namespace SadConsole.Renderers;

/// <summary>
/// Renders a cursor.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Cursor")]
public class CursorRenderStep : IRenderStep
{
    private Components.Cursor _cursor;

    /// <inheritdoc/>
    public string Name => Constants.RenderStepNames.Cursor;

    ///  <inheritdoc/>
    public uint SortOrder { get; set; } = Constants.RenderStepSortValues.Cursor;

    /// <summary>
    /// Sets the <see cref="Components.Cursor"/>.
    /// </summary>
    /// <param name="data">A <see cref="Components.Cursor"/> object.</param>
    public void SetData(object data)
    {
        if (data is Components.Cursor cursor)
            _cursor = cursor;
        else
            throw new Exception($"{nameof(CursorRenderStep)} must have a {nameof(Components.Cursor)} passed to the {nameof(SetData)} method");
    }

    ///  <inheritdoc/>
    public void Reset()
    {
        _cursor = null;
    }

    ///  <inheritdoc/>
    public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced) =>
        false;

    ///  <inheritdoc/>
    public void Composing(IRenderer renderer, IScreenSurface screenObject) { }

    ///  <inheritdoc/>
    public void Render(IRenderer renderer, IScreenSurface screenObject)
    {
        // If the tint isn't covering everything
        if (screenObject.Tint.A != 255)
        {
            // Draw any cursors
            foreach (Components.Cursor cursor in screenObject.GetSadComponents<Components.Cursor>())
            {
                if (cursor.IsVisible && screenObject.Surface.IsValidCell(cursor.Position.X, cursor.Position.Y) && screenObject.Surface.View.Contains(cursor.Position))
                {
                    XnaPoint position = screenObject.Font.GetRenderRect(cursor.Position.X - screenObject.Surface.ViewPosition.X,
                                                                        cursor.Position.Y - screenObject.Surface.ViewPosition.Y,
                                                                        screenObject.FontSize).Translate(screenObject.AbsolutePosition).Position.ToMonoPoint();

                    GameHost.Instance.DrawCalls.Enqueue(
                        new DrawCalls.DrawCallGlyph(cursor.CursorRenderCellActiveState,
                                                    ((Host.GameTexture)screenObject.Font.Image).Texture,
                                                    new XnaRectangle(position.X, position.Y, screenObject.FontSize.X, screenObject.FontSize.Y),
                                                    screenObject.Font.GetGlyphSourceRectangle(cursor.CursorRenderCellActiveState.Glyph).ToMonoRectangle(),
                                                    screenObject.Font.SolidGlyphRectangle.ToMonoRectangle()
                                                    )
                        );
                }
            }
        }
    }


    ///  <inheritdoc/>
    public void Dispose() =>
        Reset();
}
