using System;
using System.Runtime.Serialization;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace SadConsole.UI.Themes
{
    /// <summary>
    /// The theme for a ListBox control.
    /// </summary>
    [DataContract]
    public class ListBoxTheme : ThemeBase
    {
        protected bool _reconfigureSrollBar;
        private bool _drawBorder;

        /// <summary>
        /// The drawing theme for the boarder when <see cref="DrawBorder"/> is true.
        /// </summary>
        [DataMember]
        public ThemeStates BorderTheme;

        /// <summary>
        /// The line style for the border when <see cref="DrawBorder"/> is true.
        /// </summary>
        [DataMember]
        public int[] BorderLineStyle = (int[])ICellSurface.ConnectedLineThin.Clone();

        /// <summary>
        /// If false the border will not be drawn.
        /// </summary>
        [DataMember]
        public bool DrawBorder
        {
            get => _drawBorder;
            set { _drawBorder = value; _reconfigureSrollBar = true; }
        }

        /// <summary>
        /// The appearance of the scrollbar used by the listbox control.
        /// </summary>
        [DataMember]
        public ScrollBarTheme ScrollBarTheme;

        /// <summary>
        /// Creates a new theme used by the <see cref="ListBox"/>.
        /// </summary>
        /// <param name="scrollBarTheme">The theme to use to draw the scroll bar.</param>
        public ListBoxTheme(ScrollBarTheme scrollBarTheme)
        {
            ScrollBarTheme = scrollBarTheme;
            BorderTheme = new ThemeStates();
        }

        protected void SetupScrollBar(ListBox listbox)
        {
            if (DrawBorder)
                listbox.SetupScrollBar(Orientation.Vertical, listbox.Height - 2, new Point(listbox.Width - 1, 1));
            else
                listbox.SetupScrollBar(Orientation.Vertical, listbox.Height, new Point(listbox.Width - 1, 0));
        }

        /// <inheritdoc />
        public override void Attached(ControlBase control)
        {
            if (!(control is ListBox listbox)) throw new Exception("Added ListBoxTheme to a control that isn't a ListBox.");

            control.Surface = new CellSurface(control.Width, control.Height)
            {
                DefaultBackground = Color.Transparent
            };
            control.Surface.Clear();

            SetupScrollBar(listbox);
        }

        /// <inheritdoc />
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is ListBox listbox))
            {
                return;
            }

            if (!listbox.IsDirty)
            {
                return;
            }

            if (_reconfigureSrollBar)
            {
                SetupScrollBar(listbox);
                _reconfigureSrollBar = false;
            }

            RefreshTheme(control.FindThemeColors(), control);
            
            int columnOffset;
            int columnEnd;
            int startingRow;
            int endingRow;

            ColoredGlyph appearance = GetStateAppearance(listbox.State);
            ColoredGlyph scrollBarAppearance = ScrollBarTheme.GetStateAppearance(listbox.State);
            ColoredGlyph borderAppearance = BorderTheme.GetStateAppearance(listbox.State);

            // Redraw the control
            listbox.Surface.Fill(
                appearance.Foreground,
                appearance.Background,
                appearance.Glyph);

            ShowHideScrollBar(listbox);

            if (DrawBorder)
            {
                endingRow = listbox.Height - 2;
                startingRow = 1;
                columnOffset = 1;
                columnEnd = listbox.Width - 2;
                listbox.Surface.DrawBox(new Rectangle(0, 0, listbox.Width, listbox.Height), new ColoredGlyph(borderAppearance.Foreground, borderAppearance.Background, 0), null, BorderLineStyle);
            }
            else
            {
                endingRow = listbox.Height;
                startingRow = 0;
                columnOffset = 0;
                columnEnd = listbox.Width - (listbox.IsScrollBarVisible ? 1 : 0);
                listbox.Surface.Fill(borderAppearance.Foreground, borderAppearance.Background, 0, null);
            }

            listbox.MouseArea = new Rectangle(columnOffset, startingRow, columnEnd, endingRow);

            listbox.VisibleItemsTotal = listbox.Items.Count >= endingRow ? endingRow : listbox.Items.Count;
            listbox.VisibleItemsMax = listbox.MouseArea.Height;

            int offset = listbox.IsScrollBarVisible ? listbox.ScrollBar.Value : 0;
            for (int i = 0; i < endingRow; i++)
            {
                int itemIndexRelative = i + offset;
                if (itemIndexRelative < listbox.Items.Count)
                {
                    ControlStates state = 0;

                    if (listbox.State.HasFlag(ControlStates.MouseOver) && listbox.RelativeIndexMouseOver == itemIndexRelative)
                        state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.MouseOver);

                    if (listbox.State.HasFlag(ControlStates.MouseLeftButtonDown))
                        state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.MouseLeftButtonDown);

                    if (listbox.State.HasFlag(ControlStates.MouseRightButtonDown))
                        state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.MouseRightButtonDown);

                    if (listbox.State.HasFlag(ControlStates.Disabled))
                        state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.Disabled);

                    if (itemIndexRelative == listbox.SelectedIndex)
                        state = (ControlStates)Helpers.SetFlag((int)state, (int)ControlStates.Selected);

                    listbox.ItemTheme.Draw(listbox, new Rectangle(columnOffset, i + startingRow, columnEnd, 1), listbox.Items[itemIndexRelative], state);
                }
            }

            //if (listbox.IsScrollBarVisible)
            //{
            //    listbox.ScrollBar.IsDirty = true;
            //    listbox.ScrollBar.Update(time);
            //    int y = 0;

            //    for (int ycell = 0; ycell < listbox.ScrollBar.Height; ycell++)
            //    {
            //        listbox.Surface.SetGlyph(listbox.ScrollBarRenderLocation.X, listbox.ScrollBarRenderLocation.Y + y, listbox.ScrollBar.Surface[0, ycell].Glyph);
            //        listbox.Surface.SetCellAppearance(listbox.ScrollBarRenderLocation.X, listbox.ScrollBarRenderLocation.Y + y, listbox.ScrollBar.Surface[0, ycell]);
            //        y++;
            //    }
            //}

            listbox.IsDirty = Helpers.HasFlag((int)listbox.State, (int)ControlStates.MouseOver);
        }

        /// <inheritdoc />
        public override void RefreshTheme(Colors colors, ControlBase control)
        {
            if (colors == null) colors = Library.Default.Colors;

            var listbox = (ListBox)control;

            base.RefreshTheme(colors, control);

            SetForeground(Normal.Foreground);
            SetBackground(Normal.Background);
            listbox.ItemTheme.RefreshTheme(colors, control);

            listbox.ScrollBar.Theme = ScrollBarTheme;

            ScrollBarTheme?.RefreshTheme(colors, listbox.ScrollBar);

            BorderTheme.RefreshTheme(colors, control);
            BorderTheme.SetForeground(colors.Lines);
            BorderTheme.SetBackground(Normal.Background);
        }

        /// <inheritdoc />
        public override ThemeBase Clone() => new ListBoxTheme((ScrollBarTheme)ScrollBarTheme.Clone())
        {
            Normal = Normal.Clone(),
            Disabled = Disabled.Clone(),
            MouseOver = MouseOver.Clone(),
            MouseDown = MouseDown.Clone(),
            Selected = Selected.Clone(),
            Focused = Focused.Clone(),
            BorderTheme = BorderTheme?.Clone(),
            BorderLineStyle = (int[])BorderLineStyle?.Clone(),
            DrawBorder = DrawBorder,
        };

        /// <summary>
        /// Shows the scroll bar when there are too many items to display; otherwise, hides it.
        /// </summary>
        /// <param name="control">Reference to the listbox being processed.</param>
        public void ShowHideScrollBar(ListBox control)
        {
            int heightOffset = DrawBorder ? 2 : 0;

            // process the scroll bar
            int scrollbarItems = control.Items.Count - (control.Height - heightOffset);

            if (scrollbarItems > 0)
            {
                control.ScrollBar.Maximum = scrollbarItems;
                control.IsScrollBarVisible = true;
            }
            else
            {
                control.ScrollBar.Maximum = 0;
                control.IsScrollBarVisible = false;
            }
        }
    }

    public class ListBoxItemTheme : ThemeStates
    {
        public ListBoxItemTheme() { }

        /// <inheritdoc />
        public override void RefreshTheme(Colors themeColors, ControlBase control)
        {
            if (themeColors == null) themeColors = Library.Default.Colors;

            base.RefreshTheme(themeColors, control);

            SetForeground(Normal.Foreground);
            SetBackground(Normal.Background);

            Selected.Foreground = themeColors.Appearance_ControlSelected.Foreground;
            MouseOver = themeColors.Appearance_ControlOver.Clone();
        }

        public virtual void Draw(ListBox control, Rectangle area, object item, ControlStates itemState)
        {
            string value = item.ToString();
            if (value.Length < area.Width)
            {
                value += new string(' ', area.Width - value.Length);
            }
            else if (value.Length > area.Width)
            {
                value = value.Substring(0, area.Width);
            }

            if (Helpers.HasFlag((int)itemState, (int)ControlStates.Selected) && !Helpers.HasFlag((int)itemState, (int)ControlStates.MouseOver))
            {
                control.Surface.Print(area.X, area.Y, value, Selected);
            }
            else
            {
                control.Surface.Print(area.X, area.Y, value, GetStateAppearance(itemState));
            }
        }

        public virtual object Clone() => new ListBoxItemTheme()
        {
            Normal = Normal.Clone(),
            Disabled = Disabled.Clone(),
            MouseOver = MouseOver.Clone(),
            MouseDown = MouseDown.Clone(),
            Selected = Selected.Clone(),
            Focused = Focused.Clone(),
        };
    }

    public class ListBoxItemColorTheme : ListBoxItemTheme
    {
        // TODO: Change ValueTyple to specific types

        /// <summary>
        /// When <see langword="false"/>, colored boxes used when drawing the color for (Color, string) tuple will use two characters; otherwise <see langword="true"/> and only one character is used.
        /// </summary>
        public bool UseSingleCharacterForBox { get; set; } = false;

        public ListBoxItemColorTheme() { }

        public override void Draw(ListBox control, Rectangle area, object item, ControlStates itemState)
        {
            if (item is Color || item is ValueTuple<Color, string> || item is ValueTuple<Color, Color, string>)
            {
                string value = new string(' ', area.Width - 2);

                ColoredGlyph cellLook = GetStateAppearance(itemState).Clone();

                control.Surface.Print(area.X + 1, area.Y, value, cellLook);

                control.Surface.Print(area.X, area.Y, " ", cellLook);
                control.Surface.Print(area.X + area.Width - 1, area.Y, " ", cellLook);
                

                if (item is Color color)
                {
                    cellLook.Background = color;
                    control.Surface.Print(area.X + 1, area.Y, value, cellLook);

                    if (itemState.HasFlag(ControlStates.Selected))
                    {
                        control.Surface.SetGlyph(area.X, area.Y, 16);
                        control.Surface.SetGlyph(area.X + area.Width - 1, area.Y, 17);
                    }
                }
                else if (item is ValueTuple<Color, string> color2)
                {
                    bool useExtended = false;

                    if (control.AlternateFont != null)
                        useExtended = control.AlternateFont.IsSadExtended;
                    else if (control.Parent?.Host?.ParentConsole?.Font != null)
                        useExtended = control.Parent.Host.ParentConsole.Font.IsSadExtended;

                    string colorBoxesCommands = UseSingleCharacterForBox ? $"[c:r f:{color2.Item1.ToParser()}:2][c:sg 219]m" : $"[c:r f:{color2.Item1.ToParser()}:2][c:sg 219:2]mm";

                    if (useExtended)
                        colorBoxesCommands = UseSingleCharacterForBox ? $"[c:r f:{color2.Item1.ToParser()}:2][c:sg 254]m" : $"[c:r f:{color2.Item1.ToParser()}:2][c:sg 301]m[c:sg 302]m";

                    colorBoxesCommands = $"[c:r b:{cellLook.Background.ToParser()}]" + colorBoxesCommands;

                    control.Surface.Print(area.X, area.Y, ColoredString.Parse(colorBoxesCommands));
                    control.Surface.Print(area.X + 3, area.Y, color2.Item2.Align(HorizontalAlignment.Left, area.Width - 3), cellLook);
                }
                else
                {
                    cellLook.Foreground = ((ValueTuple<Color, Color, string>)item).Item2;
                    cellLook.Background = ((ValueTuple<Color, Color, string>)item).Item1;
                    value = ((ValueTuple<Color, Color, string>)item).Item3.Align(HorizontalAlignment.Left, area.Width - 2);
                    control.Surface.Print(area.X + 1, area.Y, value, cellLook);

                    if (itemState.HasFlag(ControlStates.Selected))
                    {
                        control.Surface.SetGlyph(area.X, area.Y, 16);
                        control.Surface.SetGlyph(area.X + area.Width - 1, area.Y, 17);
                    }
                }
            }
            else
            {
                base.Draw(control, area, item, itemState);
            }
        }

        public override object Clone() => new ListBoxItemColorTheme()
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
