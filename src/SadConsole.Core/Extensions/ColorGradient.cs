using ColorHelper = Microsoft.Xna.Framework.Color;
using MyMathHelper = Microsoft.Xna.Framework.MathHelper;

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections;


namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Represents a gradient with multiple color stops.
    /// </summary>
    [DataContract]
    public class ColorGradient: IEnumerable<ColorGradientStop>
    {
        /// <summary>
        /// The color stops that define the gradient.
        /// </summary>
        [DataMember]
        public ColorGradientStop[] Stops { get; set; }
        
        /// <summary>
        /// Creates a new color gradient with the defined colors and stops.
        /// </summary>
        /// <param name="colors">The colors with the gradient.</param>
        /// <param name="stops">The gradient stops where the colors are used.</param>
        public ColorGradient(IEnumerable<Color> colors, IEnumerable<float> stops)
        {
            Color[] colorList = (Color[])colors;
            float[] stopList = (float[])stops;


            if (colorList.Length != stopList.Length)
                throw new global::System.Exception("Both colors and stops much match in array length.");

            Stops = new ColorGradientStop[colorList.Length];

            for (int i = 0; i < colorList.Length; i++)
            {
                Stops[i] = new ColorGradientStop();
                Stops[i].Color = colorList[i];
                Stops[i].Stop = stopList[i];
            }
        }

        /// <summary>
        /// Creates a new color gradient with only two colors, the first at the start, and the second at the end.
        /// </summary>
        /// <param name="startingColor">The starting color of the gradient.</param>
        /// <param name="endingColor">The ending color of the gradient.</param>
        public ColorGradient(Color startingColor, Color endingColor) : this(new Color[] { startingColor, endingColor }, new float[] { 0f, 1f})
        {

        }

        /// <summary>
        /// Creates a new color gradient, evenly spacing them out. At least one color must be provided.
        /// </summary>
        /// <param name="colors">The colors to create a gradient from.</param>
        public ColorGradient(params Color[] colors)
        {
            if (colors.Length == 0)
                throw new global::System.ArgumentException("At least one color must be provided on this constructor.");

            if (colors.Length == 1)
            {
                Color color = colors[0];
                colors = new Color[2];
                colors[0] = color;
                colors[1] = color;
            }
            
            if (colors.Length >= 2)
            {
                Stops = new ColorGradientStop[colors.Length];
                float stopStrength = 1f / (colors.Length - 1);

                for (int i = 0; i < colors.Length; i++)
                {
                    Stops[i] = new ColorGradientStop {Color = colors[i], Stop = i*stopStrength};
                }
            }

        }

        /// <summary>
        /// Gets an enumerator with all of the gradient stops.
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<ColorGradientStop> GetEnumerator()
        {
            return ((IEnumerable<ColorGradientStop>)Stops).GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator with all of the gradient stops.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Stops.GetEnumerator();
        }

        /// <summary>
        /// Creates a <see cref="SadConsole.ColoredString"/> object using the current gradient.
        /// </summary>
        /// <param name="text">The text to use for the colored string.</param>
        /// <returns>A new colored string object.</returns>
        public SadConsole.ColoredString ToColoredString(string text)
        {
            SadConsole.ColoredString stringObject = new SadConsole.ColoredString(text);

            if (Stops.Length == 0)
                throw new global::System.IndexOutOfRangeException("The ColorGradient object does not have any gradient stops defined.");

            else if (Stops.Length == 1)
            {
                stringObject.SetForeground(Stops[0].Color);
                return stringObject;
            }

            float lerp = 1f / (text.Length - 1);
            float lerpTotal = 0f;

            stringObject[0].Foreground = Stops[0].Color;
            stringObject[text.Length - 1].Foreground = Stops[Stops.Length - 1].Color;

            for (int i = 1; i < text.Length - 1; i++)
            {
                lerpTotal += lerp;
                int counter;
                for (counter = 0; counter < Stops.Length && Stops[counter].Stop < lerpTotal; counter++) ;

                counter--;
                counter = (int)MyMathHelper.Clamp(counter, 0, Stops.Length - 2);

                float newLerp = (Stops[counter].Stop - (float)lerpTotal) / (Stops[counter].Stop - Stops[counter + 1].Stop);

                stringObject[i].Foreground = ColorHelper.Lerp(Stops[counter].Color, Stops[counter + 1].Color, newLerp);
            }

            return stringObject;
        }

        /// <summary>
        /// Returns a color from this gradient at the specified lerp value.
        /// </summary>
        /// <param name="amount">The lerp amount.</param>
        /// <returns>A color.</returns>
        public Color Lerp(float amount)
        {
            if (Stops.Length == 0)
                throw new global::System.IndexOutOfRangeException("The ColorGradient object does not have any gradient stops defined.");

            else if (Stops.Length == 1)
            {
                return Stops[0].Color;
            }

            int counter;
            for (counter = 0; counter < Stops.Length && Stops[counter].Stop < amount; counter++) ;

            counter--;
            counter = (int)MyMathHelper.Clamp(counter, 0, Stops.Length - 2);

            float newLerp = (Stops[counter].Stop - (float)amount) / (Stops[counter].Stop - Stops[counter + 1].Stop);

            return ColorHelper.Lerp(Stops[counter].Color, Stops[counter + 1].Color, newLerp);
        }

        public static implicit operator ColorGradient(Color color)
        {
            return new ColorGradient(color, color);
        }

        public static implicit operator Color(ColorGradient gradient)
        {
            return gradient.Stops[0].Color;
        }
    }

    /// <summary>
    /// A gradient stop. Defines a color and where it is located within the gradient.
    /// </summary>
    [DataContract]
    public struct ColorGradientStop
    {
        /// <summary>
        /// The color.
        /// </summary>
        [DataMember]
        public Color Color;

        /// <summary>
        /// The color stop in the gradiant this applies to.
        /// </summary>
        [DataMember]
        public float Stop;
    }
}
