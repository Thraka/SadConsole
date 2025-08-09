using SadConsole.UI.Controls;

namespace SadConsole.Examples;

class ListDemosScreen : SadConsole.UI.ControlsConsole
{
    public event EventHandler<IDemo?> SelectedDemoChanged;

    private ListBox _lstDemos;

    public IDemo? CurrentDemo
    {
        get => _lstDemos.SelectedItem as IDemo;
        set => _lstDemos.SelectedItem = value;
    }

    public ListDemosScreen(int width, int height) : base(width, height)
    {
        FocusOnMouseClick = false;

        _lstDemos = new ListBox(width, height)
        {
            Position = (0, 0),
            DrawBorder = false,
            ItemTheme = new DemoItemTheme()
        };

        _lstDemos.SelectedItemChanged += Demos_SelectedItemChanged;
        Controls.Add(_lstDemos);

        // Add demos
        foreach (var item in GameSettings.Demos)
            _lstDemos.Items.Add(item);

        // Select first item
        _lstDemos.SelectedIndex = 0;
    }

    private void Demos_SelectedItemChanged(object? sender, ListBox.SelectedItemEventArgs e)
    {
        // Forward the event to ours if it's a demo
        if (_lstDemos.SelectedItem is IDemo demo)
            SelectedDemoChanged?.Invoke(this, demo);

        // Select the next item instead of the header
        else
            _lstDemos.SelectedIndex += 1;
    }

    public class DemoItemTheme : ListBoxItemTheme
    {
        public override void Draw(ControlBase control, Rectangle area, object item, ControlStates itemState)
        {
            // Pad the item by two spaces for nice indentation
            if (item is IDemo)
                base.Draw(control, area, "  " + item.ToString(), itemState);

            // Item is a header item, just display it with a color
            else
                base.Draw(control, area, item, itemState);
        }
    }
}
