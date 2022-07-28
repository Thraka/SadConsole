using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// A 3D theme of the button control using thin lines. Supports the SadConsole extended character set.
/// </summary>
[DataContract]
public class ButtonLinesTheme : ButtonTheme
{
    /// <summary>
    /// When <see langword="true"/>, indicates that the lines of the theme should use the extended SadConsole font characters if available.
    /// </summary>
    [DataMember]
    public bool UseExtended { get; set; }

    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
    public ButtonLinesTheme() =>
        UseExtended = true;

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (!(control is Button button)) return;
        if (!button.IsDirty) return;

        RefreshTheme(control.FindThemeColors(), control);
        ColoredGlyph appearance;
        bool mouseDown = false;
        bool focused = false;

        appearance = ControlThemeState.GetStateAppearance(control.State);


        if (Helpers.HasFlag((int)button.State, (int)ControlStates.MouseLeftButtonDown) ||
            Helpers.HasFlag((int)button.State, (int)ControlStates.MouseRightButtonDown))
            mouseDown = true;

        // Middle part of the button for text.
        int middle = button.Surface.Height != 1 ? button.Surface.Height / 2 : 0;
        Color topleftcolor = !mouseDown ? _colorsLastUsed.Lines.ComputedColor.GetBright() : _colorsLastUsed.Lines.ComputedColor.GetDark();
        Color bottomrightcolor = !mouseDown ? _colorsLastUsed.Lines.ComputedColor.GetDark() : _colorsLastUsed.Lines.ComputedColor.GetBright();

        if (button.Surface.Height > 1 && button.Surface.Height % 2 == 0)
            middle -= 1;

        // Extended font draw
        if (button.Parent.Host.ParentConsole.Font.IsSadExtended && UseExtended)
        {
            // Redraw the control
            button.Surface.Fill(appearance.Foreground, appearance.Background,
                                appearance.Glyph, Mirror.None);

            button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width), appearance);

            if (button.Height == 1)
            {
                button.Surface.SetDecorator(0, button.Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor),
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));
                button.Surface.AddDecorator(0, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                button.Surface.AddDecorator(button.Surface.Width - 1, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
            }
            else if (button.Height == 2)
            {
                button.Surface.SetDecorator(0, button.Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor));




                button.Surface.SetDecorator(Point.ToIndex(0, button.Surface.Height - 1, button.Surface.Width), button.Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));

                button.Surface.AddDecorator(0, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                button.Surface.AddDecorator(Point.ToIndex(0, 1, button.Surface.Width), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                button.Surface.AddDecorator(button.Surface.Width - 1, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                button.Surface.AddDecorator(Point.ToIndex(button.Surface.Width - 1, 1, button.Surface.Width), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
            }
            else
            {
                button.Surface.SetDecorator(0, button.Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor));

                button.Surface.SetDecorator(Point.ToIndex(0, button.Surface.Height - 1, button.Surface.Width), button.Surface.Width,
                                                    new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));

                button.Surface.AddDecorator(0, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                button.Surface.AddDecorator(Point.ToIndex(0, button.Surface.Height - 1, button.Surface.Width), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                button.Surface.AddDecorator(button.Surface.Width - 1, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                button.Surface.AddDecorator(Point.ToIndex(button.Surface.Width - 1, button.Surface.Height - 1, button.Surface.Width), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));

                for (int y = 0; y < button.Surface.Height - 2; y++)
                {
                    button.Surface.AddDecorator(Point.ToIndex(0, y + 1, button.Surface.Width), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    button.Surface.AddDecorator(Point.ToIndex(button.Surface.Width - 1, y + 1, button.Surface.Width), 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                }
            }
        }
        else // Non extended normal draw
        {
            button.Surface.Fill(appearance.Foreground, appearance.Background,
                appearance.Glyph, Mirror.None);

            button.Surface.Print(1, middle, button.Text.Align(button.TextAlignment, button.Width - 2), appearance);

            button.Surface.DrawBox(new Rectangle(0, 0, button.Width, button.Surface.Height),
                                   new ColoredGlyph(topleftcolor, appearance.Background, 0),
                                   null,
                                   connectedLineStyle: focused ? ICellSurface.ConnectedLineThick : ICellSurface.ConnectedLineThin);

            //SadConsole.Algorithms.Line(0, 0, button.Width - 1, 0, (x, y) => { return true; });

            button.Surface.DrawLine(new Point(0, 0), new Point(button.Width - 1, 0), null, topleftcolor, appearance.Background);
            button.Surface.DrawLine(new Point(0, 0), new Point(0, button.Surface.Height - 1), null, topleftcolor, appearance.Background);
            button.Surface.DrawLine(new Point(button.Width - 1, 0), new Point(button.Width - 1, button.Surface.Height - 1), null, bottomrightcolor, appearance.Background);
            button.Surface.DrawLine(new Point(1, button.Surface.Height - 1), new Point(button.Width - 1, button.Surface.Height - 1), null, bottomrightcolor, appearance.Background);
        }

        button.IsDirty = false;
    }

    /// <inheritdoc />
    public override void RefreshTheme(Colors colors, ControlBase control)
    {
        base.RefreshTheme(colors, control);

        var tempBackground = ControlThemeState.Disabled.Background;
        ControlThemeState.SetBackground(ControlThemeState.Normal.Background);
        ControlThemeState.Disabled.Background = tempBackground;
    }

    /// <inheritdoc />
    public override ThemeBase Clone() => new ButtonLinesTheme()
    {
        ControlThemeState = ControlThemeState.Clone(),
        ShowEnds = ShowEnds,
        EndsThemeState = EndsThemeState.Clone(),
        UseExtended = UseExtended
    };
}
