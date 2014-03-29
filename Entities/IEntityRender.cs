namespace SadConsole.Entities
{
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Renders and updates an entity.
    /// </summary>
    public interface IEntityComponent
    {
        /// <summary>
        /// Renders the entity with its own <see cref="SpriteBatch"/>, calling Begin and End.
        /// </summary>
        void Render();

        /// <summary>
        /// Renders the entity with the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        void Render(SpriteBatch spriteBatch);

        /// <summary>
        /// Runs update logic on the entity;
        /// </summary>
        void Update();
    }
}
