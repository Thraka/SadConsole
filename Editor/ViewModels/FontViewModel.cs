using MugenMvvmToolkit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.ViewModels
{
    class FontViewModel: CloseableViewModel
    {
        private Font font;
        private Font.FontSizes fontSize;

        public Font Font => font;

        public IList<SadConsole.FontMaster> Fonts => SadConsole.Global.Fonts.Values.ToArray();

        public FontMaster FontMaster
        {
            get => font.Master;
            set { font = value.GetFont(fontSize); OnPropertyChanged("Font"); }
        }

        public Font.FontSizes FontSize
        {
            get => fontSize;
            set { fontSize = value; font = FontMaster.GetFont(value); OnPropertyChanged("Font"); }
        }

        //public string FontName => font.Master.Name;

        //public string FontSizeName => Program.FontSizeNames[(int)font.SizeMultiple];

        protected override void OnInitialized()
        {
            base.OnInitialized();
            font = SadConsole.Global.FontDefault;
        }

        public void SetFont(Font font)
        {
            this.font = font;
            OnPropertyChanged("Font");
        }
    }
}
