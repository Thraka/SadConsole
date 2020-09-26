using SadRogue.Primitives;
using static SadConsole.UI.Colors;

namespace SadConsole.UI
{
    //TODO: Document
    public class AdjustableColor
    {
        /// <summary>
        /// The name of the theme color.
        /// </summary>
        public string Title { get; set; }
        public Color BaseColor { get; set; }
        public Color ComputedColor
        {
            get
            {
                if (IsCustomColor)
                    return BaseColor;

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
        public bool IsCustomColor { get; set; }
        public ColorNames UIColor { get; private set; }

        public Brightness Brightness { get; set; } = Brightness.Normal;

        public AdjustableColor(Color color, string title, Colors colors)
        {
            BaseColor = color;
            Title = title;
            IsCustomColor = true;

            if (colors.TryToColorName(color, out ColorNames colorName))
            {
                UIColor = colorName;
                IsCustomColor = false;
            }
        }

        public AdjustableColor(ColorNames color, string title, Colors colors)
        {
            UIColor = color;
            SetUIColor(color, colors);
            Title = title;
        }

        public void SetUIColor(ColorNames color, Colors colors)
        {
            UIColor = color;
            IsCustomColor = false;
            BaseColor = colors.FromColorName(color);
        }
    }
}
