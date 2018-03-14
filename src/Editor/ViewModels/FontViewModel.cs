using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.ViewModels
{
    class FontViewModel
    {
        private SadConsole.Font _font;
        private SadConsole.FontMaster _fontMaster;
        private SadConsole.Font.FontSizes _fontSize;

        public SadConsole.FontMaster[] Fonts => SadConsole.Global.Fonts.Values.ToArray();

        public SadConsole.Font Font { get => _font; set => _font = value; }

        public SadConsole.FontMaster FontMaster { get => _fontMaster; set { _fontMaster = value; Font = value.GetFont(_fontSize); } }

        public SadConsole.Font.FontSizes FontSize { get => _fontSize; set { _fontSize = value; Font = FontMaster.GetFont(value); } }


    }
}
