using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole.UI.Controls;

public partial class Panel
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
    public bool UseInsetBorder { get; set; } = false;

    /// <summary>
    /// The current Appearance based on the control state.
    /// </summary>
    [DataMember]
    public ColoredGlyphBase? Appearance { get; protected set; }

    /// <inheritdoc/>
    public override void UpdateAndRedraw(TimeSpan time)
    {
        // If we're allowed to draw the panel itself
        if (IsDirty && !SkipDrawing)
        {
            Colors currentColors = FindThemeColors();

            RefreshThemeStateColors(currentColors);

            if (!UseNormalStateOnly)
                Appearance = ThemeState.GetStateAppearance(State);
            else
                Appearance = ThemeState.Normal;

            Surface.Fill(Appearance.Foreground, Appearance.Background, Appearance.Glyph);

            if (DrawBorder)
            {
                Color topleftcolor = !UseInsetBorder ? currentColors.Lines : currentColors.Lines.ComputedColor.GetDarker();
                Color bottomrightcolor = !UseInsetBorder ? currentColors.Lines.ComputedColor.GetDarker() : currentColors.Lines;

                if (Parent!.Host!.ParentConsole!.Font.IsSadExtended && UseExtendedBorderGlyphs)
                {
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
                    Surface.DrawBox(new Rectangle(0, 0, Width, Surface.Height),
                                           ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
                                                                           new ColoredGlyph(topleftcolor, Appearance.Background, 0)));

                    //SadConsole.Algorithms.Line(0, 0, Width - 1, 0, (x, y) => { return true; });

                    Surface.DrawLine(Point.Zero, new Point(Width - 1, 0), null, topleftcolor, Appearance.Background);
                    Surface.DrawLine(Point.Zero, new Point(0, Surface.Height - 1), null, topleftcolor, Appearance.Background);
                    Surface.DrawLine(new Point(Width - 1, 0), new Point(Width - 1, Surface.Height - 1), null, bottomrightcolor, Appearance.Background);
                    Surface.DrawLine(new Point(1, Surface.Height - 1), new Point(Width - 1, Surface.Height - 1), null, bottomrightcolor, Appearance.Background);
                }
            }

            IsDirty = false;
        }

        // Draw the children controls
        base.UpdateAndRedraw(time);
    }
}
