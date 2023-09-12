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
        _lstDemos = new ListBox(80, 23)
        {
            Position = (0, 0),
            DrawBorder = false
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
        // Forward the event to ours
        SelectedDemoChanged?.Invoke(this, (IDemo)_lstDemos.SelectedItem);
    }
}
