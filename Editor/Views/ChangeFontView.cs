using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MugenMvvmToolkit.Binding.Builders;
using MugenMvvmToolkit.Binding;
using MugenMvvmToolkit.Attributes;

namespace SadConsole.Editor.Views
{
    [ViewModel(typeof(ViewModels.FontViewModel), "ChangeFontView")]
    public partial class ChangeFontView : Form
    {
        private bool loading;

        public ChangeFontView()
        {
            InitializeComponent();

            using (var set = new BindingSet<ViewModels.FontViewModel>())
            {
                set.Bind(lstFonts, "Items").To(() => (vm, ctx) => vm.Fonts);
                set.Bind(lstFonts, AttachedMemberConstants.SelectedItem).To(() => (vm, ctx) => vm.FontMaster).TwoWay();
            }
        }

        public Font SelectedFont;


        public ChangeFontView(Font font)
        {
            loading = true;
            SelectedFont = font;
            InitializeComponent();
        }

        private void ChangeFont_Load(object sender, EventArgs e)
        {
            //cboMultiplier.SelectedIndex = 0;
            //foreach(var master in Global.Fonts.Values)
            //{
            //    lstFonts.Items.Add(master);
            //}

            //foreach (var item in lstFonts.Items)
            //{
            //    if (((SadConsole.FontMaster)item) == SelectedFont.Master)
            //    {
            //        lstFonts.SelectedItem = item;
            //        break;
            //    }
            //}

            //cboMultiplier.SelectedIndex = (int)SelectedFont.SizeMultiple;
            loading = false;
            //DrawFont();
        }

        public void DrawFont()
        {
            if (loading)
                return;

            var master = lstFonts.SelectedItem as SadConsole.FontMaster;
            if (master != null)
            {
                SelectedFont = master.GetFont((SadConsole.Font.FontSizes)cboMultiplier.SelectedIndex);
            }

            var surface = SelectedFont.GetFontSurface();
            surface.DefaultBackground = Microsoft.Xna.Framework.Color.Black;
            surface.DefaultForeground = Microsoft.Xna.Framework.Color.White;

            for (int i = 0; i < surface.CellCount; i++)
            {
                surface[i].Foreground = surface.DefaultForeground;
                surface[i].Background = surface.DefaultBackground;
            }
            var renderer = new Renderers.SurfaceRenderer();
            renderer.Render(surface, true);

            picPreview.Image = surface.LastRenderResult.ToImage();
        }

        private void cboMultiplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DrawFont();
        }

        private void lstFonts_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DrawFont();
        }
    }
}
