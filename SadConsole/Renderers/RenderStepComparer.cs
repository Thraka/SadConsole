using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Renderers
{
    /// <summary>
    /// Compares <see cref="IRenderStep"/> with the <see cref="IRenderStep.SortOrder"/> property.
    /// </summary>
    public class RenderStepComparer : IComparer<Renderers.IRenderStep>
    {
        /// <inheritdoc/>
        public int Compare(IRenderStep x, IRenderStep y)
        {
            if (x.SortOrder < y.SortOrder)
                return -1;

            // default for if (x.SortOrder > y.SortOrder)
            return 1;
        }
    }
}
