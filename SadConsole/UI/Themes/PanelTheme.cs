using System;
using System.Reflection;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes;

/// <summary>
/// A basic theme for a drawing surface that simply fills the surface based on the state.
/// </summary>
[DataContract]
public class PanelTheme : ThemeBase
{
    /// <summary>
    /// When true, only uses <see cref="ThemeStates.Normal"/> for drawing.
    /// </summary>
    [DataMember]
    public bool UseNormalStateOnly { get; set; } = true;

    /// <summary>
    /// When true, ignores all states and doesn't draw anything.
    /// </summary>
    [DataMember]
    public bool SkipDrawing { get; set; } = false;

    /// <summary>
    /// When <see langword="true"/>, indicates that the theme should draw along the inner bounds of the panel.
    /// </summary>
    [DataMember]
    public bool DrawBorder { get; set; } = false;

    /// <summary>
    /// When <see langword="true"/>, indicates that the lines of the theme should use the extended SadConsole font characters if available.
    /// </summary>
    [DataMember]
    public bool UseExtendedBorderGlyphs { get; set; } = false;

    /// <summary>
    /// When <see langword="true"/>, indicates that the border colors should make it appear like the border is inset.
    /// </summary>
    [DataMember]
    public bool UseInsetBorder{ get; set; } = false;


    /// <summary>
    /// The current Appearance based on the control state.
    /// </summary>
    [DataMember]
    public ColoredGlyph? Appearance { get; protected set; }

    /// <inheritdoc />
    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (SkipDrawing || control is not Panel)
            return;

        RefreshTheme(control.FindThemeColors(), control);

        if (!UseNormalStateOnly)
            Appearance = ControlThemeState.GetStateAppearance(control.State);
        else
            Appearance = ControlThemeState.Normal;

        control.Surface.Fill(Appearance.Foreground, Appearance.Background, Appearance.Glyph);

        if (DrawBorder)
        {
            Color topleftcolor = !UseInsetBorder ? _colorsLastUsed.Lines.ComputedColor.GetBright() : _colorsLastUsed.Lines.ComputedColor.GetDark();
            Color bottomrightcolor = !UseInsetBorder ? _colorsLastUsed.Lines.ComputedColor.GetDark() : _colorsLastUsed.Lines.ComputedColor.GetBright();

            if (control!.Parent!.Host!.ParentConsole!.Font.IsSadExtended && UseExtendedBorderGlyphs)
            {
                if (control.Height == 1)
                {
                    control.Surface.SetDecorator(0, control.Surface.Width,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor),
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));
                    control.Surface.AddDecorator(0, 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    control.Surface.AddDecorator(control.Surface.Width - 1, 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                }
                else if (control.Height == 2)
                {
                    control.Surface.SetDecorator(0, control.Surface.Width,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor));




                    control.Surface.SetDecorator(Point.ToIndex(0, control.Surface.Height - 1, control.Surface.Width), control.Surface.Width,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));

                    control.Surface.AddDecorator(0, 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    control.Surface.AddDecorator(Point.ToIndex(0, 1, control.Surface.Width), 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    control.Surface.AddDecorator(control.Surface.Width - 1, 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                    control.Surface.AddDecorator(Point.ToIndex(control.Surface.Width - 1, 1, control.Surface.Width), 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                }
                else
                {
                    control.Surface.SetDecorator(0, control.Surface.Width,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor));

                    control.Surface.SetDecorator(Point.ToIndex(0, control.Surface.Height - 1, control.Surface.Width), control.Surface.Width,
                                                        new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(bottomrightcolor));

                    control.Surface.AddDecorator(0, 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    control.Surface.AddDecorator(Point.ToIndex(0, control.Surface.Height - 1, control.Surface.Width), 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                    control.Surface.AddDecorator(control.Surface.Width - 1, 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                    control.Surface.AddDecorator(Point.ToIndex(control.Surface.Width - 1, control.Surface.Height - 1, control.Surface.Width), 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));

                    for (int y = 0; y < control.Surface.Height - 2; y++)
                    {
                        control.Surface.AddDecorator(Point.ToIndex(0, y + 1, control.Surface.Width), 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
                        control.Surface.AddDecorator(Point.ToIndex(control.Surface.Width - 1, y + 1, control.Surface.Width), 1, control.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));
                    }
                }
            }
            else // Non extended normal draw
            {
                control.Surface.DrawBox(new Rectangle(0, 0, control.Width, control.Surface.Height),
                                       ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
                                                                       new ColoredGlyph(topleftcolor, Appearance.Background, 0)));

                //SadConsole.Algorithms.Line(0, 0, control.Width - 1, 0, (x, y) => { return true; });

                control.Surface.DrawLine(Point.Zero, new Point(control.Width - 1, 0), null, topleftcolor, Appearance.Background);
                control.Surface.DrawLine(Point.Zero, new Point(0, control.Surface.Height - 1), null, topleftcolor, Appearance.Background);
                control.Surface.DrawLine(new Point(control.Width - 1, 0), new Point(control.Width - 1, control.Surface.Height - 1), null, bottomrightcolor, Appearance.Background);
                control.Surface.DrawLine(new Point(1, control.Surface.Height - 1), new Point(control.Width - 1, control.Surface.Height - 1), null, bottomrightcolor, Appearance.Background);
            }
        }

        control.IsDirty = false;
    }

    /// <inheritdoc />
    public override ThemeBase Clone() => new PanelTheme()
    {
        ControlThemeState = ControlThemeState.Clone(),
        UseNormalStateOnly = UseNormalStateOnly,
        SkipDrawing = SkipDrawing
    };
}
