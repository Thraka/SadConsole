using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace SadConsoleEditor.Controls
{
    class EditorsListBoxItem : ListBoxItemTheme
    {
        public override void Draw(ListBox control, Rectangle area, object item, ControlStates itemState)
        {
            var look = GetStateAppearance(itemState);
            string value = ((Editors.IEditorMetadata)item).Title;
            if (value.Length < area.Width)
                value += new string(' ', area.Width - value.Length);
            else if (value.Length > area.Width)
                value = value.Substring(0, area.Width);
            control.Surface.Print(area.X, area.Y, value, look);
        }
    }

    class FileLoaderListBoxItem : ListBoxItemTheme
    {
        public override void Draw(ListBox control, Rectangle area, object item, ControlStates itemState)
        {
            var look = GetStateAppearance(itemState);
            string value = ((FileLoaders.IFileLoader)item).FileTypeName;
            if (value.Length < area.Width)
                value += new string(' ', area.Width - value.Length);
            else if (value.Length > area.Width)
                value = value.Substring(0, area.Width);
            control.Surface.Print(area.X, area.Y, value, look);
        }
    }
}
