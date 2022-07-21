using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Myra.Assets;

namespace SadConsole.MyraUI
{
    internal class ChainedAssetResolver : IAssetResolver
    {
        private IAssetResolver[] _resolvers;

        public ChainedAssetResolver(params IAssetResolver[] resolvers)
        {
            if (resolvers == null)
                throw new ArgumentNullException(nameof(resolvers));
            if (resolvers.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(resolvers));

            _resolvers = resolvers;
        }
        public Stream Open(string assetName)
        {
            foreach (var item in _resolvers)
            {
                try
                {
                    return item.Open(assetName);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            throw new Exception("No resolvers could find the asset");
        }
    }
}
