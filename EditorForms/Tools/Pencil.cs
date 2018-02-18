using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.Tools
{
    internal class Pencil : ITool
    {
        public Model.GlyphItem Glyph = new Model.GlyphItem();

        public string Name => "Pencil";

        public System.Windows.Forms.Control GetUI()
        {
            var panel = new System.Windows.Forms.Panel();
            Panels.GlyphEditPanel.SharedInstance.DataObject = Glyph;
            panel.AddArrangeControls(Panels.GlyphEditPanel.SharedInstance, new System.Windows.Forms.Button(), new System.Windows.Forms.Button());
            return panel;
        }

        public void OnUpdate(MouseConsoleState mouse)
        {
            if (Global.MouseState.IsOnScreen && mouse.IsOnConsole)
            {
                if (Global.MouseState.LeftButtonDown)
                {
                    mouse.Cell.Foreground = Glyph.Foreground;
                    mouse.Cell.Background = Glyph.Background;
                    mouse.Cell.Glyph = Glyph.Glyph;
                    mouse.Console.TextSurface.IsDirty = true;
                }
            }
        }
    }
}
