using System;
using SadConsole.Entities;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace SadConsole.Components
{
    /// <summary>
    /// Add to a <see cref="Console"/> to draw the <see cref="Entities"/> on it. May be used with multiple consoles.
    /// </summary>
    public class MultipleHostEntityDrawing : LogicComponent
    {
        /// <summary>
        /// Entities to draw on each console this component is added to.
        /// </summary>
        public List<Entity> Entities { get; } = new List<Entity>();

        /// <summary>
        /// If set to true, controls the <see cref="ScreenObject.IsVisible"/> property of the attached object.
        /// </summary>
        public bool HandleIsVisible { get; set; } = true;

        /// <inheritdoc />
        public override void Update(ScreenObject host)
        {
            foreach (Entity entity in Entities)
            {
                entity.Update();
            }
        }

        /// <inheritdoc />
        public override void Draw(ScreenObject host)
        {
            foreach (Entity entity in Entities)
            {
                if (host is ScreenObjectSurface parent)
                {
                    Rectangle parentViewPort = parent.Surface.GetViewRectangle();

                    entity.PositionOffset = new Point(-parentViewPort.Position.X, -parentViewPort.Position.Y).TranslateFont(parent.FontSize, entity.FontSize);

                    if (HandleIsVisible)
                        entity.IsVisible = parent.AbsoluteArea.Contains(entity.AbsolutePosition);
                }

                if (entity.IsVisible)
                    entity.Draw();
            }
        }
    }
}
