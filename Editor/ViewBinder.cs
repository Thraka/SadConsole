using System.ComponentModel;
using MugenMvvmToolkit.WinForms.Binding.UiDesigner;

namespace SadConsole.Editor
{
    /// <summary>
    /// This class allows you to use the bindings from the WinForms designer. 
    /// Drag the class from the Toolbox panel on your form.
    /// </summary>
    public partial class ViewBinder : Binder
    {
        public ViewBinder()
        {
        }

        public ViewBinder(IContainer container)
            : base(container)
        {
        }
    }
}