using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Consoles;
using SadConsole.Controls;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsoleEditor.Controls
{
    public class ListBoxItemFrame : ListBoxItem
    {
        public override void Draw(ITextSurface surface, Rectangle area)
        {
            string value = ((Editors.EntityEditor.FrameWrapper)Item).CurrentIndex.ToString();
            if (value.Length < area.Width)
                value += new string(' ', area.Width - value.Length);
            else if (value.Length > area.Width)
                value = value.Substring(0, area.Width);

            var editor = new SurfaceEditor(surface);
            editor.Print(area.X, area.Y, value, _currentAppearance);
            _isDirty = false;
        }
    }
}
