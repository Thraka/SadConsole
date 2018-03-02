using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor
{
    class SurfaceEditor: IEditor
    {
        protected SadConsole.Surfaces.LayeredSurface surface;
        protected Renderers.LayeredSurfaceRenderer renderer;


        public string File { get; set; } = null;
        
        public Surfaces.BasicSurface Surface => surface;

        public void Update()
        {

        }

        public void Draw(SpriteBatch batch)
        {

        }

        public void Save()
        {

        }

        protected SurfaceEditor()
        {
            renderer = new Renderers.LayeredSurfaceRenderer();
        }

        public SurfaceEditor(int width, int height)
        {
            surface = new Surfaces.LayeredSurface(width, height, 1);
            surface.GetLayer(0).Metadata = new Surfaces.LayerMetadata() { Name = "Root" };

            renderer = new Renderers.LayeredSurfaceRenderer();
        }

        public static SurfaceEditor Load()
        {
            SurfaceEditor editor = new SurfaceEditor();
            
            

            return editor;
        }
    }
}
