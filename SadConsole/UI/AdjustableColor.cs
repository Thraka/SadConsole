using SadRogue.Primitives;
using static SadConsole.UI.Colors;

namespace SadConsole.UI
{
    /// <summary>
    /// A color that can be adjusted by brightness and mapped to a <see cref="Colors"/> color.
    /// </summary>
    public class AdjustableColor
    {
        /// <summary>
        /// The name of the color.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The color without brightness.
        /// </summary>
        public Color BaseColor { get; private set; }

        /// <summary>
        /// The color with brightness applied.
        /// </summary>
        public Color ComputedColor
        {
            get
            {
                return Brightness switch
                {
                    Brightness.Brightest => BaseColor.GetBrightest(),
                    Brightness.Bright => BaseColor.GetBright(),
                    Brightness.Dark => BaseColor.GetDark(),
                    Brightness.Darkest => BaseColor.GetDarkest(),
                    _ => BaseColor,
                };
            }
        }

        /// <summary>
        /// <see langword="true"/> when this color is not defined by a <see cref="Colors"/> object; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsCustomColor { get; set; }

        /// <summary>
        /// The <see cref="Colors"/> mapped color when <see cref="IsCustomColor"/> is <see langword="false"/>.
        /// </summary>
        public ColorNames UIColor { get; private set; }

        /// <summary>
        /// A brightness to apply to the color.
        /// </summary>
        public Brightness Brightness { get; set; }

        /// <summary>
        /// Creates a color that isn't mapped to a <see cref="Colors"/> color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="name">A friendly name of the color.</param>
        public AdjustableColor(Color color, string name)
        {
            Name = name;
            SetColor(color);
        }

        /// <summary>
        /// Creates a color and tries to map it to a <see cref="Colors"/> color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="name">A friendly name of the color.</param>
        /// <param name="colors">The colors to try and map to.</param>
        public AdjustableColor(Color color, string name, Colors colors)
        {
            Name = name;
            SetColor(color, colors, Brightness.Normal);
        }

        /// <summary>
        /// Creates a color that maps to a <see cref="Colors"/> color.
        /// </summary>
        /// <param name="color">The predefined color.</param>
        /// <param name="name">A friendly name of the color.</param>
        /// <param name="colors">The colors used setting the <see cref="BaseColor"/> value.</param>
        public AdjustableColor(ColorNames color, string name, Colors colors)
        {
            UIColor = color;
            SetUIColor(color, colors, Brightness.Normal);
            Name = name;
        }

        private AdjustableColor() { }

        /// <summary>
        /// Maps this adjustable color to a <see cref="Colors"/> color.
        /// </summary>
        /// <param name="color">The predefined color.</param>
        /// <param name="colors">The colors used setting the <see cref="BaseColor"/> value.</param>
        /// <param name="brightness">The brightness to apply to the color.</param>
        public void SetUIColor(ColorNames color, Colors colors, Brightness brightness)
        {
            UIColor = color;
            IsCustomColor = false;
            BaseColor = colors.FromColorName(color);
            Brightness = brightness;
        }

        /// <summary>
        /// Tries to map this adjustable color to a <see cref="Colors"/> color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="colors">The colors to try and map to.</param>
        /// <param name="brightness">The brightness to apply to the color.</param>
        public void SetColor(Color color, Colors colors, Brightness brightness)
        {
            BaseColor = color;
            IsCustomColor = true;
            Brightness = brightness;

            if (colors.TryToColorName(color, out ColorNames colorName))
            {
                UIColor = colorName;
                IsCustomColor = false;
            }
        }

        /// <summary>
        /// Configures this adjustable color to the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetColor(Color color)
        {
            BaseColor = color;
            IsCustomColor = true;
            Brightness = Brightness.Normal;
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>A new adjustable color object.</returns>
        public AdjustableColor Clone()
        {
            return new AdjustableColor()
            {
                Name = Name,
                BaseColor = BaseColor,
                IsCustomColor = IsCustomColor,
                UIColor = UIColor,
                Brightness = Brightness
            };
        }

        /// <summary>
        /// Casts this object to a <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The adjustable color.</param>
        public static implicit operator Color(AdjustableColor color) =>
            color.ComputedColor;
    }
}
