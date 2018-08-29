using System;
using Microsoft.Xna.Framework;

using System.Runtime.Serialization;
using SadConsole.Controls;
using SadConsole.Surfaces;

namespace SadConsole.Themes
{
    /// <summary>
    /// The theme for a ListBox control.
    /// </summary>
    [DataContract]
    public class ListBoxTheme : ThemeBase<ListBox>
    {
        /// <summary>
        /// The drawing theme for the boarder when <see cref="DrawBorder"/> is true.
        /// </summary>
        [DataMember]
        public ThemeStates BorderTheme;

        /// <summary>
        /// The line style for the border when <see cref="DrawBorder"/> is true.
        /// </summary>
        [DataMember]
        public int[] BorderLineStyle;

        /// <summary>
        /// If false the border will not be drawn.
        /// </summary>
        [DataMember]
        public bool DrawBorder;

        /// <summary>
        /// The appearance of an item.
        /// </summary>
        [DataMember]
        public ListBoxItemTheme ItemTheme;

        /// <summary>
        /// The appearance of the scrollbar used by the listbox control.
        /// </summary>
        [DataMember]
        public ScrollBarTheme ScrollBarTheme;

        public ListBoxTheme()
        {
            SetForeground(Normal.Foreground);
            SetBackground(Normal.Background);

            DrawBorder = true;
            ScrollBarTheme = (ScrollBarTheme)Library.Default.ScrollBarTheme?.Clone() ?? new ScrollBarTheme();
            ItemTheme = new ListBoxItemTheme();
            BorderTheme = new ThemeStates();
            BorderTheme.SetForeground(Normal.Foreground);
            BorderTheme.SetBackground(Normal.Background);
            BorderLineStyle = (int[])SurfaceBase.ConnectedLineThick.Clone();
        }

        public override void Attached(ListBox control)
        {
            control.Surface = new BasicNoDraw(control.Width, control.Height);
        }

        public override void UpdateAndDraw(ListBox control, TimeSpan time)
        {
            if (!control.IsDirty) return;

            int columnOffset;
            int columnEnd;
            int startingRow;
            int endingRow;


            Cell appearance = GetStateAppearance(control.State);
            Cell scrollBarAppearance = ScrollBarTheme.GetStateAppearance(control.State);
            Cell borderAppearance = BorderTheme.GetStateAppearance(control.State);

            // Redraw the control
            control.Surface.Fill(
                appearance.Foreground,
                appearance.Background,
                appearance.Glyph);

            if (DrawBorder)
            {
                endingRow = control.Height - 2;
                startingRow = 1;
                columnOffset = 1;
                columnEnd = control.Width - 2;
                control.Surface.DrawBox(new Rectangle(0, 0, control.Width, control.Height), new Cell(borderAppearance.Foreground, borderAppearance.Background, 0), null, BorderLineStyle);
            }
            else
            {
                endingRow = control.Height;
                startingRow = 0;
                columnOffset = 0;
                columnEnd = control.Width;
                control.Surface.Fill(borderAppearance.Foreground, borderAppearance.Background, 0, null);
            }

            int offset = control.IsSliderVisible ? control.Slider.Value : 0;
            for (int i = 0; i < endingRow; i++)
            {
                var itemIndexRelative = i + offset;
                if (itemIndexRelative < control.Items.Count)
                {
                    ControlStates state = 0;

                    if (Helpers.HasFlag(control.State, ControlStates.MouseOver) && control.RelativeIndexMouseOver == itemIndexRelative)
                        Helpers.SetFlag(ref state, ControlStates.MouseOver);

                    if (control.State.HasFlag(ControlStates.MouseLeftButtonDown))
                        Helpers.SetFlag(ref state, ControlStates.MouseLeftButtonDown);

                    if (control.State.HasFlag(ControlStates.MouseRightButtonDown))
                        Helpers.SetFlag(ref state, ControlStates.MouseRightButtonDown);

                    if (control.State.HasFlag(ControlStates.Disabled))
                        Helpers.SetFlag(ref state, ControlStates.Disabled);

                    if (itemIndexRelative == control.SelectedIndex)
                        Helpers.SetFlag(ref state, ControlStates.Selected);

                    ItemTheme.Draw(control.Surface, new Rectangle(columnOffset, i + startingRow, columnEnd, 1), control.Items[itemIndexRelative], state);
                }
            }

            if (control.IsSliderVisible)
            {
                control.Slider.IsDirty = true;
                control.Slider.Update(time);
                var y = control.SliderRenderLocation.Y;

                for (var ycell = 0; ycell < control.Slider.Height; ycell++)
                {
                    control.Surface.SetGlyph(control.SliderRenderLocation.X, y, control.Slider.Surface[0, ycell].Glyph);
                    control.Surface.SetCellAppearance(control.SliderRenderLocation.X, y, control.Slider.Surface[0, ycell]);
                    y++;
                }
            }


            control.IsDirty = Helpers.HasFlag(control.State, ControlStates.MouseOver);
        }

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new ListBoxTheme()
            {
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
                BorderTheme = BorderTheme.Clone(),
                BorderLineStyle = (int[])BorderLineStyle.Clone(),
                DrawBorder = DrawBorder,
                ItemTheme = (ListBoxItemTheme)ItemTheme.Clone(),
                ScrollBarTheme = (ScrollBarTheme)ScrollBarTheme.Clone()
            };
        }
    }

    public class ListBoxItemTheme: ThemeStates
    {
        public ListBoxItemTheme()
        {
            SetForeground(Normal.Foreground);
            SetBackground(Normal.Background);

            Selected.Foreground = Library.Default.Appearance_ControlSelected.Foreground;
            MouseOver = Library.Default.Appearance_ControlOver.Clone();
        }

        public virtual void Draw(Surfaces.SurfaceBase surface, Rectangle area, object item, ControlStates itemState)
        {
            string value = item.ToString();
            if (value.Length < area.Width)
                value += new string(' ', area.Width - value.Length);
            else if (value.Length > area.Width)
                value = value.Substring(0, area.Width);
            if (Helpers.HasFlag(itemState, ControlStates.Selected) && !Helpers.HasFlag(itemState, ControlStates.MouseOver))
                surface.Print(area.Left, area.Top, value, Selected);
            else
                surface.Print(area.Left, area.Top, value, GetStateAppearance(itemState));
        }

        public virtual object Clone()
        {
            return new ListBoxItemTheme()
            {
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
            };
        }
    }

    public class ListBoxItemColorTheme : ListBoxItemTheme
    {
        public virtual void Draw(Surfaces.SurfaceBase surface, Rectangle area, object item, ControlStates itemState)
        {
            if (item is Color || item is Tuple<Color, Color, string>)
            {
                string value = new string(' ', area.Width - 2);

                Cell cellLook = GetStateAppearance(itemState).Clone();

                if (item is Color color)
                {
                    cellLook.Background = color;
                    surface.Print(area.Left + 1, area.Top, value, cellLook);
                }
                else
                {
                    cellLook.Foreground = ((Tuple<Color, Color, string>)item).Item2;
                    cellLook.Background = ((Tuple<Color, Color, string>)item).Item1;
                    value = ((Tuple<Color, Color, string>)item).Item3.Align(HorizontalAlignment.Left, area.Width - 2);
                    surface.Print(area.Left + 1, area.Top, value, cellLook);
                }

                surface.Print(area.Left, area.Top, " ", cellLook);
                surface.Print(area.Left + area.Width - 1, area.Top, " ", cellLook);

                if (itemState.HasFlag(ControlStates.Clicked))
                {
                    surface.SetGlyph(area.Left, area.Top, 16);
                    surface.SetGlyph(area.Left + area.Width - 1, area.Top, 17);
                }
            }
            else
                base.Draw(surface, area, item, itemState);
        }

        public override object Clone()
        {
            return new ListBoxItemColorTheme()
            {
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
            };
        }
    }
}
