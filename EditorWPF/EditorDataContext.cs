using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PointSys = System.Windows.Point;
using WpfWindow = System.Windows.Window;

namespace SadConsole.Editor
{
    public class EditorDataContext : DependencyObject
    {
        public SadConsole.Surfaces.ISurface Surface
        {
            get { return (SadConsole.Surfaces.ISurface)GetValue(SurfaceProperty); }
            set { SetValue(SurfaceProperty, value); }
        }

        public PointSys MousePosition
        {
            get { return (PointSys)GetValue(MousePositionProperty); }
            set { SetValue(MousePositionProperty, value); }
        }

        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        public static readonly DependencyProperty SurfaceProperty =
            DependencyProperty.Register("Surface", typeof(SadConsole.Surfaces.ISurface), typeof(EditorDataContext), new PropertyMetadata(null));

        public static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.Register("MousePosition", typeof(PointSys), typeof(EditorDataContext), new PropertyMetadata(new PointSys()));

        public static readonly DependencyProperty IsEditModeProperty =
           DependencyProperty.Register("IsEditMode", typeof(bool), typeof(EditorDataContext), new PropertyMetadata(false, new PropertyChangedCallback((o, p) => { ((EditorDataContext)o).ToggleEditMode(); })));


        public SadConsole.Surfaces.SurfaceEditor Editor;
        
        internal void ToggleEditMode()
        {
            Settings.DoUpdate = !IsEditMode;
        }
    }
}
