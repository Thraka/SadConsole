using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Entities
{
    public class EntityLiteManager : Components.UpdateComponent
    {
        /// <summary>
        /// The entities associated with this manager.
        /// </summary>
        public List<EntityLite> Entities { get; } = new List<EntityLite>();

        /// <summary>
        /// Indicates that this object needs to be redrawn.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Internal use only
        /// </summary>
        public Renderers.IRenderStep RenderStep;

        public override void OnAdded(IScreenObject host)
        {
            if (!(host is IScreenSurface surface)) throw new ArgumentException($"Must add this component to a type that implements {nameof(IScreenSurface)}");

            RenderStep?.Dispose();
            RenderStep = GameHost.Instance.GetRendererStep("entitylite");
            
            surface.Renderer.AddRenderStep(RenderStep);
        }

        public override void OnRemoved(IScreenObject host)
        {
            ((IScreenSurface)host).Renderer.RemoveRenderStep(RenderStep);
            RenderStep?.Dispose();
            RenderStep = null;
        }

        public override void Update(IScreenObject host, TimeSpan delta)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Update(delta);
                if (Entities[i].Appearance.IsDirty)
                    IsDirty = true;
            }
        }
    }
}
