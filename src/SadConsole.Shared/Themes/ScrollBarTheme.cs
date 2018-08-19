using System;
using SadConsole.Controls;
using SadConsole.Surfaces;

namespace SadConsole.Themes
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The theme of the slider control.
    /// </summary>
    [DataContract]
    public class ScrollBarTheme: ThemeBase<ScrollBar>
    {
        /// <summary>
        /// The theme part fot the start button.
        /// </summary>
        [DataMember]
        public int StartButtonVerticalGlyph;

        /// <summary>
        /// The theme part fot the start button.
        /// </summary>
        [DataMember]
        public int EndButtonVerticalGlyph;

        /// <summary>
        /// The theme part fot the start button.
        /// </summary>
        [DataMember]
        public int StartButtonHorizontalGlyph;

        /// <summary>
        /// The theme part fot the start button.
        /// </summary>
        [DataMember]
        public int EndButtonHorizontalGlyph;

        /// <summary>
        /// The theme part for the scroll bar bar where the slider is not located.
        /// </summary>
        [DataMember]
        public int BarGlyph;

        /// <summary>
        /// The theme part for the scroll bar icon.
        /// </summary>
        [DataMember]
        public int SliderGlyph;

        public ScrollBarTheme()
        {
            //TODO add states for ends. Bar should use base state.
            StartButtonVerticalGlyph = 30;
            EndButtonVerticalGlyph = 31;
            StartButtonHorizontalGlyph = 17;
            EndButtonHorizontalGlyph = 16;
            SliderGlyph = 219;
        }

        public override void Attached(ScrollBar control)
        {
            control.Surface = new BasicNoDraw(control.Width, control.Height);
        }

        public override void UpdateAndDraw(ScrollBar control, TimeSpan time)
        {
            if (!control.IsDirty) return;

            Cell appearance;

            if (Helpers.HasFlag(control.State, ControlStates.Disabled))
                appearance = Disabled;

            else if (Helpers.HasFlag(control.State, ControlStates.MouseLeftButtonDown) || Helpers.HasFlag(control.State, ControlStates.MouseRightButtonDown))
                appearance = MouseDown;

            else if (Helpers.HasFlag(control.State, ControlStates.MouseOver))
                appearance = MouseOver;

            else if (Helpers.HasFlag(control.State, ControlStates.Focused))
                appearance = Focused;

            else
                appearance = Normal;

            control.Surface.Clear();

            if (control.Orientation == Orientation.Horizontal)
            {
                control.Surface.SetCellAppearance(0, 0, appearance);
                control.Surface.SetGlyph(0, 0, StartButtonVerticalGlyph);

                control.Surface.SetCellAppearance(control.Width - 1, 0, appearance);
                control.Surface.SetGlyph(control.Width - 1, 0, EndButtonVerticalGlyph);

                for (int i = 1; i <= control.SliderBarSize; i++)
                {
                    control.Surface.SetCellAppearance(i, 0, appearance);
                    control.Surface.SetGlyph(i, 0, SliderGlyph);
                }

                if (control.Value >= control.Minimum && control.Value <= control.Maximum && control.Minimum != control.Maximum)
                {
                    if (control.IsEnabled)
                    {
                        control.Surface.SetCellAppearance(1 + control.CurrentSliderPosition, 0, appearance);
                        control.Surface.SetGlyph(1 + control.CurrentSliderPosition, 0, SliderGlyph);
                    }
                }
            }
            else
            {
                control.Surface.SetCellAppearance(0, 0, appearance);
                control.Surface.SetGlyph(0, 0, StartButtonVerticalGlyph);

                control.Surface.SetCellAppearance(0, control.Height - 1, appearance);
                control.Surface.SetGlyph(0, control.Height - 1, EndButtonVerticalGlyph);

                for (int i = 0; i < control.SliderBarSize; i++)
                {
                    control.Surface.SetCellAppearance(0, i + 1, appearance);
                    control.Surface.SetGlyph(0, i + 1, SliderGlyph);
                }

                if (control.Value >= control.Minimum && control.Value <= control.Maximum && control.Minimum != control.Maximum)
                {
                    if (control.IsEnabled)
                    {
                        control.Surface.SetCellAppearance(0, 1 + control.CurrentSliderPosition, appearance);
                        control.Surface.SetGlyph(0, 1 + control.CurrentSliderPosition, SliderGlyph);
                    }
                }
                    
            }

            control.IsDirty = false;
        }

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new ScrollBarTheme()
            {
                Normal = Normal.Clone(),
                Disabled = Disabled.Clone(),
                MouseOver = MouseOver.Clone(),
                MouseDown = MouseDown.Clone(),
                Selected = Selected.Clone(),
                Focused = Focused.Clone(),
                StartButtonVerticalGlyph = StartButtonVerticalGlyph,
                EndButtonVerticalGlyph = EndButtonVerticalGlyph,
                StartButtonHorizontalGlyph = StartButtonHorizontalGlyph,
                EndButtonHorizontalGlyph = EndButtonHorizontalGlyph,
                BarGlyph = BarGlyph,
                SliderGlyph = SliderGlyph
            };
        }
    }
}
