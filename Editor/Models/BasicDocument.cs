using Microsoft.Xna.Framework;
using MugenMvvmToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Models
{
    class BasicDocument: DataContext
    {
        public BasicDocument(Color background)
        {
            Background = background;
        }

        public Color Background = Color.Transparent;
    }
}
