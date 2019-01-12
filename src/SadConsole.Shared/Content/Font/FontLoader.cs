namespace SadConsole.Content
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class FontLoader
    {
        private readonly IContentProvider contentProvider;
        private readonly TextureLoader textureLoader;

        public FontLoader(IContentProvider contentProvider, TextureLoader textureLoader)
        {
            this.contentProvider = contentProvider ?? throw new ArgumentNullException(nameof(contentProvider));
            this.textureLoader = textureLoader ?? throw new ArgumentNullException(nameof(textureLoader));
        }

        public async Task<FontMaster> Load(string name)
        {
            using (var stream = await contentProvider.Open(name).ConfigureAwait(false))
            using (var reader = new StreamReader(stream))
            {
                var font = (FontMaster)JsonConvert.DeserializeObject(
                    reader.ReadToEnd(),
                    typeof(FontMaster),
                    new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });

                font.Image = await textureLoader.Load(font.FilePath);
                font.ConfigureRects();

                return font;
            }
        }
    }
}
