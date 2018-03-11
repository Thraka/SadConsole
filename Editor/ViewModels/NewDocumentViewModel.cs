using Microsoft.Xna.Framework;
using MugenMvvmToolkit.ViewModels;
using MugenMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models;

namespace SadConsole.Editor.ViewModels
{
    class NewDocumentViewModel: CloseableViewModel
    {
        private Font documentFont = SadConsole.Global.FontDefault;

        //private EditorTypeEnum

        public System.Windows.Input.ICommand SetFontCommand { get; private set; }

        public EditorTypeEnum EditorType { get; set; }

        public int DocumentWidth { get; set; }

        public int DocumentHeight { get; set; }

        //public Font DocumentFont
        //{
        //    get => documentFont;
        //    set
        //    {
        //        documentFont = value;

        //        txtFont.Text = documentFont.Name;
        //        txtFontSize.Text = FontSizeNames[(int)documentFont.SizeMultiple];
        //    }
        //}
        
        public Color DocumentBackground { get; set; } = Color.Coral;

        public Color DocumentForeground { get; set; } = Color.Purple;

        public string FontName => documentFont.Master.Name;

        public string FontSize => Program.FontSizeNames[(int)documentFont.SizeMultiple];
        

        public NewDocumentViewModel()
        {
            SetFontCommand = MugenMvvmToolkit.Models.RelayCommandBase.FromAsyncHandler(ShowFontChooser);
        }

        protected override void OnInitializing(IDataContext context)
        {
            base.OnInitializing(context);


        }

        private async Task ShowFontChooser()
        {
            
            using (var viewModel = GetViewModel<FontViewModel>())
            {
                viewModel.SetFont(documentFont);
                await viewModel.ShowAsync("ChangeFontView");

                //viewModel.
            }
            //await this.ShowAsync("ChangeFontView", new MugenMvvmToolkit.Models.DataContext());
        }

        
    }
}
