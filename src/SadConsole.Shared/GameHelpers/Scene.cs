using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Surfaces;
using Console = SadConsole.Console;
using System.Runtime.Serialization;
using SadConsole.Input;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// Groups a <see cref="ISurface"/> and a list of <see cref="GameObject"/> types together.
    /// </summary>
    public class Scene: Screen
    {
        /// <summary>
        /// The objects for the scene.
        /// </summary>
        public List<GameObject> Objects;

        /// <summary>
        /// Regions defined for the scene.
        /// </summary>
        public List<Zone> Zones;

        /// <summary>
        /// Hotspots defined for the scene.
        /// </summary>
        public List<Hotspot> Hotspots;

        /// <summary>
        /// Width of the backing <see cref="Surface"/>.
        /// </summary>
        public int Width { get { return Surface.Width; } }

        /// <summary>
        /// Height of the backing <see cref="Surface"/>.
        /// </summary>
        public int Height { get { return Surface.Height; } }

        /// <summary>
        /// Access to the backing console that is internally wrapped in this scene.
        /// </summary>
        public SurfaceEditor Surface { get; }

        /// <summary>
        /// The renderer used in drawing the <see cref="Surface"/>.
        /// </summary>
        public Renderers.ISurfaceRenderer SurfaceRenderer { get; set; }

        /// <summary>
        /// Treats the <see cref="Position"/> of the console as if it is pixels and not cells.
        /// </summary>
        [DataMember]
        public bool UsePixelPositioning { get; set; } = false;

        /// <summary>
        /// Creates a new Scene from an existing <see cref="LayeredSurface"/>.
        /// </summary>
        /// <param name="surface">The surface for the scene.</param>
        /// <param name="renderer">The renderer for the surface.</param>
        public Scene(ISurface surface, Renderers.ISurfaceRenderer renderer)
        {
            Surface = new SurfaceEditor(surface);
            SurfaceRenderer = renderer;
            Objects = new List<GameObject>();
            Zones = new List<Zone>();
            Hotspots = new List<Hotspot>();
        }

        /// <summary>
        /// Creates a new surface using a <see cref="BasicSurface"/> at the specified width and height.
        /// </summary>
        /// <param name="width">The width of the surface.</param>
        /// <param name="height">The height of the surface.</param>
        public Scene(int width, int height): this(new BasicSurface(width, height), new Renderers.SurfaceRenderer())
        {

        }

        public override void Draw(TimeSpan timeElapsed)
        {
            if (IsVisible)
            {
                // Draw the scene.
                SurfaceRenderer.Render(Surface.TextSurface);
                Global.DrawCalls.Add(new DrawCallSurface(Surface.TextSurface, calculatedPosition, UsePixelPositioning));

                // Draw objects
                foreach (var item in Objects)
                    item.Draw(timeElapsed);

                // Draw the children.
                if (Children.Count != 0)
                {
                    var copyList = new List<IScreen>(Children);

                    foreach (var child in copyList)
                        child.Draw(timeElapsed);
                }
            }
        }

        Point cachedOffset;

        public override void Update(TimeSpan timeElapsed)
        {
            base.Update(timeElapsed);
            
            Point offset = position + Surface.TextSurface.RenderArea.Location;

            if (offset != cachedOffset)
            {
                cachedOffset = offset;

                foreach (var gameObject in Objects)
                    gameObject.PositionOffset = cachedOffset;
            }
        }

        public override void OnCalculateRenderPosition()
        {
            base.OnCalculateRenderPosition();

            Point offset = position + Surface.TextSurface.RenderArea.Location;

            if (offset != cachedOffset)
            {
                cachedOffset = offset;

                foreach (var gameObject in Objects)
                    gameObject.PositionOffset = cachedOffset;
            }
        }

        /// <summary>
        /// Saves the scene to a file. You must serialize the <see cref="Surface"/> separately.
        /// </summary>
        /// <param name="file">The destination file.</param>
        public void Save(string file)
        {
            Serializer.Save((SerializedTypes.SceneSerialized)this, file);
        }

        /// <summary>
        /// Loads scene from a file.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="surface">The surface for the scene.</param>
        /// <param name="renderer">The renderer used to draw the surface.</param>
        /// <returns></returns>
        public static Scene Load(string file, ISurface surface, Renderers.ISurfaceRenderer renderer)
        {
            var scene = (Scene)Serializer.Load<SerializedTypes.SceneSerialized>(file);

            scene.Surface.TextSurface = surface;
            scene.SurfaceRenderer = renderer;

            scene.OnCalculateRenderPosition();

            return scene;
        }


    }
}
