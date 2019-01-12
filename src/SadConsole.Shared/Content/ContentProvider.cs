namespace SadConsole.Content
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;

    public interface IContentProvider
    {
        /// <summary>
        /// Opens a stream for the given content name.
        /// </summary>
        Task<Stream> Open(string name);
    }

    /// <summary>
    /// ContentProvider supports reading files, http files and embedded resources.
    /// </summary>
    /// <example>
    /// await contentProvider.Open("https://example.com/logo.png");
    /// await contentProvider.Open("embedded:SadConsole.Resources.IBM.font");
    /// await contentProvider.Open("C:\example.png");
    /// </example>
    public class ContentProvider : IContentProvider
    {
        private readonly Lazy<HttpClient> http;

        /// <summary>
        /// Initialize a new <see cref="ContentProvider"/>.
        /// </summary>
        /// <param name="messageHandler"></param>
        public ContentProvider(HttpMessageHandler messageHandler = null)
        {
            this.http = new Lazy<HttpClient>(() => new HttpClient(messageHandler ?? new HttpClientHandler()));
        }

        /// <summary>
        /// Opens a stream for the given content name.
        /// </summary>
        public async Task<Stream> Open(string name)
        {
            if (name.StartsWith("embedded:", StringComparison.OrdinalIgnoreCase))
            {
                var embeddedName = name.Substring(9);
                return GetAssembly().GetManifestResourceStream(embeddedName);
            }

            if (name.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                name.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                var response = await http.Value.GetAsync(name).ConfigureAwait(false);
                var content = response.EnsureSuccessStatusCode().Content;
                return await content.ReadAsStreamAsync().ConfigureAwait(false);
            }

#if XNA
            return Microsoft.Xna.Framework.TitleContainer.OpenStream(name);
#else
            if (!File.Exists(name))
                return null;

            return File.OpenRead(name);
#endif
        }

        private static Assembly GetAssembly()
        {
#if WINDOWS_UWP || WINDOWS_UAP
            return typeof(ContentProvider).GetTypeInfo().Assembly;
#else
            return Assembly.GetExecutingAssembly();
#endif
        }
    }
}
