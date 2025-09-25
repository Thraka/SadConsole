using Gum.Wireframe;
using SadConsole.Configuration;
using SadConsole.Editor;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

Settings.WindowTitle = "SadEditor v0.2";

Builder config =
    new Builder()
        .SetWindowSizeInCells(130, 50)
        .OnStart(StartHandler);

        //.UseDefaultConsole()
        //.IsStartingScreenFocused(true)
        //.SetStartingScreen<ExampleConsole>();

        //.SetStartingScreen<KeyboardScreen>()
        //.IsStartingScreenFocused(true);

Game.Create(config);
Game.Instance.Run();
Game.Instance.Dispose();

static void StartHandler(object? sender, GameHost host)
{
    Core.Start();
}

class ExampleConsole : ControlsConsole
{
    public ExampleConsole() : base(130, 50)
    {
        GraphicalUiElement.CanvasWidth = Width;
        GraphicalUiElement.CanvasHeight = Height;

        //SadConsoleGumHost parentContainer = new(this)
        //{
        //    HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
        //    ChildrenLayout = Gum.Managers.ChildrenLayout.AutoGridVertical,
        //    WrapsChildren = true
        //};

        //for (int i = 0; i < 10; i++)
        //{
        //    SadConsoleGumControl buttonWrapper = new((renderObject) => new ButtonBox((int)renderObject.Width, (int)renderObject.Height) { Text = "Button " + i.ToString() })
        //    {
        //        XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Left,
        //        XUnits = Gum.Converters.GeneralUnitType.PixelsFromSmall,
        //    };

        //    parentContainer.Children.Add(buttonWrapper);
        //}

        //parentContainer.Generate();

        ///////////////////////

        Panel panel = new(Width, Height)
        {
        };

        GraphicalUiElement parentContainer = new(new ControlBaseGumWrapper(panel), null!)
        {
            Width = Width,
            Height = Height,
            ChildrenLayout = Gum.Managers.ChildrenLayout.TopToBottomStack
        };



        SelectionButton? previousButton = null;

        //for (int i = 0; i < 10; i++)
        //{

        SelectionButton button1 = new(1, 1);


        GraphicalUiElement buttonWrapper = new(new ControlBaseGumWrapper(button1), null!)
        {
            XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Left,
            XUnits = Gum.Converters.GeneralUnitType.PixelsFromSmall,
            Width = 10,
            Height = 1
        };

        parentContainer.Children.Add(buttonWrapper);


        buttonWrapper.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToContainer;
        buttonWrapper.Width = 0;

        //}

        //foreach (IRenderableIpso child in parentContainer.Children)
        //{
        //    SelectionButton button1 = new((int)child.Width, (int)child.Height)
        //    {
        //        Text = "Test",
        //        Position = ((int)child.X, (int)child.Y)
        //    };

        //    if (previousButton != null)
        //    {
        //        button1.PreviousSelection = previousButton;
        //        previousButton.NextSelection = button1;
        //    }

        //    previousButton = button1;

        //    panel.Add(button1);
        //}

        //((SelectionButton)panel[^1]).NextSelection = (SelectionButton)panel[0];
        //((SelectionButton)panel[0]).PreviousSelection = (SelectionButton)panel[^1];

        Controls.Add(panel);

    }

}
