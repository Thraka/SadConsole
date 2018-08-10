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

        /// <summary>
        /// Returns a clone of this object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ScrollBarTheme()
            {
                StartButtonVerticalGlyph =  StartButtonVerticalGlyph,
                EndButtonVerticalGlyph = EndButtonVerticalGlyph,
                StartButtonHorizontalGlyph = StartButtonHorizontalGlyph,
                EndButtonHorizontalGlyph = EndButtonHorizontalGlyph,
                BarGlyph = BarGlyph,
                SliderGlyph = SliderGlyph
            };
        }

        public override void Draw(ScrollBar control, SurfaceBase hostSurface)
        {
            if (control.IsDirty)
            {
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

                if (control.Orientation == Orientation.Horizontal)
                {
                    hostSurface.SetCellAppearance(control.Bounds.Left, control.Bounds.Top, appearance);
                    hostSurface.SetGlyph(control.Bounds.Left, control.Bounds.Top, StartButtonVerticalGlyph);

                    hostSurface.SetCellAppearance(control.Bounds.Right - 1, 0, appearance);
                    hostSurface.SetGlyph(control.Bounds.Right - 1, 0, EndButtonVerticalGlyph);

                    for (int i = 1; i <= control.SliderBarSize; i++)
                    {
                        hostSurface.SetCellAppearance(control.Bounds.Left + i, control.Bounds.Top, appearance);
                        hostSurface.SetGlyph(control.Bounds.Left + i, control.Bounds.Top, SliderGlyph);
                    }

                    if (control.Value >= control.Minimum && control.Value <= control.Maximum && control.Minimum != control.Maximum)
                    {
                        if (control.IsEnabled)
                        {
                            hostSurface.SetCellAppearance(control.Bounds.Left + 1 + control.CurrentSliderPosition, control.Bounds.Top, appearance);
                            hostSurface.SetGlyph(control.Bounds.Left + 1 + control.CurrentSliderPosition, control.Bounds.Top, SliderGlyph);
                        }
                    }
                }
                else
                {
                    hostSurface.SetCellAppearance(control.Bounds.Left, control.Bounds.Top, appearance);
                    hostSurface.SetGlyph(control.Bounds.Left, control.Bounds.Top, StartButtonVerticalGlyph);

                    hostSurface.SetCellAppearance(control.Bounds.Left, control.Bounds.Bottom - 1, appearance);
                    hostSurface.SetGlyph(control.Bounds.Left, control.Bounds.Bottom - 1, EndButtonVerticalGlyph);

                    for (int i = 0; i < control.SliderBarSize; i++)
                    {
                        hostSurface.SetCellAppearance(control.Bounds.Left, control.Bounds.Top + i + 1, appearance);
                        hostSurface.SetGlyph(control.Bounds.Left, control.Bounds.Top + i + 1, SliderGlyph);
                    }

                    if (control.Value >= control.Minimum && control.Value <= control.Maximum && control.Minimum != control.Maximum)
                    {
                        if (control.IsEnabled)
                        {
                            hostSurface.SetCellAppearance(control.Bounds.Left, control.Bounds.Top + 1 + control.CurrentSliderPosition, appearance);
                            hostSurface.SetGlyph(control.Bounds.Left, control.Bounds.Top + 1 + control.CurrentSliderPosition, SliderGlyph);
                        }
                    }
                    
                }

                control.IsDirty = false;
            }
        }
    }
}
