namespace SadConsole.Content
{
#if XNA
    using Microsoft.Xna.Framework.Graphics;
#endif

    using System;
    using System.Threading.Tasks;

    public class TextureLoader
    {
        private readonly IContentProvider contentProvider;

        public TextureLoader(IContentProvider contentProvider)
        {
            this.contentProvider = contentProvider ?? throw new ArgumentNullException(nameof(contentProvider));
        }

        public async Task<Texture2D> Load(string name)
        {
            using (var stream = await contentProvider.Open(name).ConfigureAwait(false))
            {
#if XNA
                return Texture2D.FromStream(Global.GraphicsDevice, stream);
#else
                throw new NotImplementedException();
#endif
            }
        }
    }
}
