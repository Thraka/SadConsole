#if XNA
using Microsoft.Xna.Framework;
#endif

using System;
using SadConsole.Entities;
using System.Collections.Generic;

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

        /// <summary>
        /// If set to true, controls the <see cref="Console.IsVisible"/> property of the attached object.
        /// </summary>
        public bool HandleIsVisible { get; set; } = true;

        /// <inheritdoc />
        public override void Update(Console console, TimeSpan delta)
        {
            foreach (Entity entity in Entities)
            {
                entity.Update(delta);
            }
        }

        /// <inheritdoc />
        public override void Draw(Console console, TimeSpan delta)
        {
            foreach (Entity entity in Entities)
            {
                if (console is IConsoleViewPort scroller)
                {
                    Rectangle parentViewPort = scroller.ViewPort;

                    entity.PositionOffset = new Point(-parentViewPort.Location.X, -parentViewPort.Location.Y).TranslateFont(console.Font, entity.Font) + console.CalculatedPosition.PixelLocationToConsole(entity.Font);

                    if (HandleIsVisible)
                    {
                        entity.IsVisible = console.AbsoluteArea.Contains(entity.CalculatedPosition);
                    }
                }
                else
                {
                    entity.PositionOffset = console.CalculatedPosition.PixelLocationToConsole(entity.Font);

                    if (HandleIsVisible)
                    {
                        entity.IsVisible = console.AbsoluteArea.Contains(entity.CalculatedPosition);
                    }
                }


                if (entity.IsVisible)
                {
                    entity.Draw(delta);
                }
            }
        }
    }
}
