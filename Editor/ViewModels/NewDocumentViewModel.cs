using Microsoft.Xna.Framework;
using MugenMvvmToolkit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadConsole.Editor.ViewModels
{
    class NewDocumentViewModel: CloseableViewModel
    {
        public string[] FontSizeNames = new[] { "Quarter", "Half", "1x", "2x", "3x", "4x" };
        private Font documentFont;
        //private EditorTypeEnum


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

        public NewDocumentViewModel()
        {
            
        }
    }
}
