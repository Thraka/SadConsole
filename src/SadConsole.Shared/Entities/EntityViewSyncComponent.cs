using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SadConsole.Entities
{
    /// <summary>
    /// Add to an <see cref="Entity"/> to sync the visibility and position offset with a parent <see cref="Console"/>. 
    /// </summary>
    public class EntityViewSyncComponent : UpdateConsoleComponent
    {
        /// <inheritdoc />
        public override void Update(Console console, TimeSpan delta)
        {
            if (console.Parent is IConsoleViewPort parent)
            {
                var parentViewPort = parent.ViewPort;
                ((Entity)console).PositionOffset = new Point(-parentViewPort.Location.X, -parentViewPort.Location.Y);
                console.IsVisible = parentViewPort.Contains(console.Position);
            }
            else
            {
                console.IsVisible = console.Position.X >= 0 && console.Position.Y >= 0 &&
                                    console.Position.X < console.Parent.Width && console.Position.Y < console.Parent.Height;
            }
        }

        /// <inheritdoc />
        public override void OnAdded(Console console)
        {
            if (!(console is Entity))
                throw new Exception($"{nameof(EntityViewSyncComponent)} can only be added to an entity.");
        }
    }
}
