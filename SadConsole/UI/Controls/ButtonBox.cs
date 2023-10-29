using System;
using System.Runtime.Serialization;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

/// <summary>
/// A button control that is drawn using lines around it.
/// </summary>
[DataContract]
public class ButtonBox : ButtonBase
{
    /// <summary>
    /// When <see langword="true"/>, indicates that the lines of the theme should use the extended SadConsole font characters if available.
    /// </summary>
    [DataMember]
    public bool UseExtended { get; set; }

    /// <summary>
    /// Creates an instance of the button control with the specified width and height.
    /// </summary>
    /// <param name="width">Width of the control.</param>
    /// <param name="height">Height of the control (default is 1).</param>
    public ButtonBox(int width, int height)
        : base(width, height)
    {
        
    }

    ///<inheritdoc/>
    protected override void RefreshThemeStateColors(Colors colors)
    {
        base.RefreshThemeStateColors(colors);

        Color tempColor = ThemeState.Disabled.Background;
        ThemeState.SetBackground(ThemeState.Normal.Background);
        ThemeState.Disabled.Background = tempColor;

        tempColor = ThemeState.Focused.Foreground;
        ThemeState.SetForeground(ThemeState.Normal.Foreground);
        ThemeState.Focused.Foreground = tempColor;
    }

    ///<inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        if (!IsDirty) return;

        ColoredGlyphBase appearance;
        bool mouseDown = false;
        bool focused = false;

        Colors colors = FindThemeColors();

        RefreshThemeStateColors(colors);

        appearance = ThemeState.GetStateAppearance(State);

        if (Helpers.HasFlag((int)State, (int)ControlStates.MouseLeftButtonDown) ||
            Helpers.HasFlag((int)State, (int)ControlStates.MouseRightButtonDown))
            mouseDown = true;

        // Middle part of the button for text.
        int middle = Surface.Height != 1 ? Surface.Height / 2 : 0;
        Color topleftcolor = !mouseDown ? colors.Lines.ComputedColor.GetBright() : colors.Lines.ComputedColor.GetDark();
        Color bottomrightcolor = !mouseDown ? colors.Lines.ComputedColor.GetDark() : colors.Lines.ComputedColor.GetBright();

        if (Surface.Height > 1 && Surface.Height % 2 == 0)
            middle -= 1;

        // Extended font draw
        if (Parent!.Host!.ParentConsole!.Font.IsSadExtended && UseExtended)
        {
            // Redraw the control
            Surface.Fill(appearance.Foreground, appearance.Background,
                                appearance.Glyph, Mirror.None);

            Surface.Print(0, middle, Text.Align(TextAlignment, Width), appearance);

            if (Height == 1)
            {
                Surface.SetDecorator(0, Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor),
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));
                Surface.AddDecorator(0, 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                Surface.AddDecorator(Surface.Width - 1, 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
            }
            else if (Height == 2)
            {
                Surface.SetDecorator(0, Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor));




                Surface.SetDecorator(Point.ToIndex(0, Surface.Height - 1, Surface.Width), Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));

                Surface.AddDecorator(0, 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                Surface.AddDecorator(Point.ToIndex(0, 1, Surface.Width), 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                Surface.AddDecorator(Surface.Width - 1, 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                Surface.AddDecorator(Point.ToIndex(Surface.Width - 1, 1, Surface.Width), 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
            }
            else
            {
                Surface.SetDecorator(0, Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor));

                Surface.SetDecorator(Point.ToIndex(0, Surface.Height - 1, Surface.Width), Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));

                Surface.AddDecorator(0, 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                Surface.AddDecorator(Point.ToIndex(0, Surface.Height - 1, Surface.Width), 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                Surface.AddDecorator(Surface.Width - 1, 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                Surface.AddDecorator(Point.ToIndex(Surface.Width - 1, Surface.Height - 1, Surface.Width), 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));

                for (int y = 0; y < Surface.Height - 2; y++)
                {
                    Surface.AddDecorator(Point.ToIndex(0, y + 1, Surface.Width), 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    Surface.AddDecorator(Point.ToIndex(Surface.Width - 1, y + 1, Surface.Width), 1, Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                }
            }
        }
        else // Non extended normal draw
        {
            Surface.Fill(appearance.Foreground, appearance.Background,
                appearance.Glyph, Mirror.None);

            Surface.Print(1, middle, Text.Align(TextAlignment, Width - 2), appearance);

            Surface.DrawBox(new Rectangle(0, 0, Width, Surface.Height),
                                   ShapeParameters.CreateStyledBox(focused ? ICellSurface.ConnectedLineThick : ICellSurface.ConnectedLineThin,
                                                                   new ColoredGlyph(topleftcolor, appearance.Background, 0)));

            //SadConsole.Algorithms.Line(0, 0, Width - 1, 0, (x, y) => { return true; });

            Surface.DrawLine(Point.Zero, new Point(Width - 1, 0), null, topleftcolor, appearance.Background);
            Surface.DrawLine(Point.Zero, new Point(0, Surface.Height - 1), null, topleftcolor, appearance.Background);
            Surface.DrawLine(new Point(Width - 1, 0), new Point(Width - 1, Surface.Height - 1), null, bottomrightcolor, appearance.Background);
            Surface.DrawLine(new Point(1, Surface.Height - 1), new Point(Width - 1, Surface.Height - 1), null, bottomrightcolor, appearance.Background);
        }

        IsDirty = false;
    }

    [OnDeserialized]
    private void AfterDeserialized(StreamingContext context)
    {
        DetermineState();
        IsDirty = true;
    }
}
