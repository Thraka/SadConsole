﻿#if XNA
using Microsoft.Xna.Framework;
#endif

using System;
using SadConsole.Entities;

namespace SadConsole.Components
{
    /// <summary>
    /// Add to an <see cref="Entity"/> to sync the visibility and position offset with a parent <see cref="Console"/>. 
    /// </summary>
    public class EntityViewSyncComponent : UpdateConsoleComponent
    {
        private Point _oldPosition;
        private Rectangle _oldView;

        /// <inheritdoc />
        public override void Update(Console console, TimeSpan delta)
        {
            if (console.Parent is IConsoleViewPort parent)
            {
                var parentViewPort = parent.ViewPort;

                if (parentViewPort != _oldView || console.Position != _oldPosition)
                {
                    ((Entity)console).PositionOffset = new Point(-parentViewPort.Location.X, -parentViewPort.Location.Y);
                    console.IsVisible = parentViewPort.Contains(console.Position);

                    _oldPosition = console.Position;
                    _oldView = parentViewPort;
                }
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
            if (!(console is Entity ent))
                throw new Exception($"{nameof(EntityViewSyncComponent)} can only be added to an entity.");
        }

        /// <inheritdoc />
        public override void OnRemoved(Console console)
        {
            _oldPosition = Point.Zero;
            _oldView = Rectangle.Empty;

            console.IsVisible = true;
            ((Entity)console).PositionOffset = Point.Zero;
        }
    }
}