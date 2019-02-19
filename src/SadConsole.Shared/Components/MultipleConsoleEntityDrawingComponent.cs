#if XNA
using Microsoft.Xna.Framework;
#endif

using System;
using SadConsole.Entities;
using System.Collections.Generic;
using Console = SadConsole.Console;

namespace SadConsole.Components
{
    /// <summary>
    /// Add to a <see cref="Console"/> to draw the <see cref="Entities"/> on it. May be used with multiple consoles.
    /// </summary>
    public class MultipleConsoleEntityDrawingComponent : LogicConsoleComponent
    {
        /// <summary>
        /// Entities to draw on each console this component is added to.
        /// </summary>
        public List<Entity> Entities { get; } = new List<Entity>();

        /// <inheritdoc />
        public override void Update(Console console, TimeSpan delta)
        {
            foreach (var entity in Entities)
                entity.Update(delta);
        }

        /// <inheritdoc />
        public override void Draw(Console console, TimeSpan delta)
        {
            foreach (var entity in Entities)
            {
                if (console is IConsoleViewPort scroller)
                {
                    var parentViewPort = scroller.ViewPort;

                    entity.PositionOffset = new Point(-parentViewPort.Location.X, -parentViewPort.Location.Y) + console.CalculatedPosition;
                    entity.IsVisible = parentViewPort.Contains(entity.Position);
                }
                else
                {
                    entity.PositionOffset = console.CalculatedPosition;
                    entity.IsVisible = entity.Position.X >= 0 && entity.Position.Y >= 0 &&
                                       entity.Position.X < console.Width && entity.Position.Y < console.Height;
                }

                if (entity.IsVisible)
                    entity.Draw(delta);
            }
        }
    }
}
