using Microsoft.Xna.Framework;
using SadRogue.Primitives;

namespace SadConsole.Host;

/// <summary>
/// A MonoGame component that clears the screen with the <see cref="SadConsole.Settings.ClearColor"/> color.
/// </summary>
public class ClearScreenGameComponent : DrawableGameComponent
{
    /// <inheritdoc/>
    public ClearScreenGameComponent(Microsoft.Xna.Framework.Game game) : base(game) => DrawOrder = 0;

    /// <inheritdoc/>
    public override void Draw(GameTime gameTime)
    {
        Game.GraphicsDevice.SetRenderTarget(null);
        Game.GraphicsDevice.Clear(SadConsole.Settings.ClearColor.ToMonoColor());
    }
}
