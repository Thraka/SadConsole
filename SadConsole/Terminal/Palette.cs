using System;
using SadRogue.Primitives;

namespace SadConsole.Terminal;

/// <summary>
/// 256-color terminal palette with truecolor support.
/// </summary>
public class Palette
{
    private static readonly int[] s_cubeSteps = { 0, 95, 135, 175, 215, 255 };

    private readonly Color[] _colors = new Color[256];
    private readonly Color[] _defaults = new Color[256];

    /// <summary>
    /// Creates a new palette initialized with standard ANSI defaults (CTerm spec).
    /// </summary>
    public Palette()
    {
        BuildDefaults(_colors);
        Array.Copy(_colors, _defaults, 256);
    }

    /// <summary>
    /// Gets or sets the color at the specified index in the collection.
    /// </summary>
    /// <remarks>Accessing an index that is out of range will throw an IndexOutOfRangeException.</remarks>
    /// <param name="index">The zero-based index of the color to get or set. Must be within the bounds of the collection.</param>
    /// <returns>The color associated with the specified index.</returns>
    public Color this[int index]
    {
        get => GetColor(index);
        set => SetColor(index, value);
    }

    /// <summary>
    /// Gets the number of colors contained in the palette.
    /// </summary>
    public int Length => _colors.Length;

    /// <summary>
    /// Gets the color at the specified palette index (0–255).
    /// </summary>
    public Color GetColor(int index) =>
        (uint)index < 256 ? _colors[index] : _colors[0];

    /// <summary>
    /// Redefines the color at the specified palette index (for OSC 4).
    /// </summary>
    public void SetColor(int index, Color color)
    {
        if ((uint)index < 256)
            _colors[index] = color;
    }

    /// <summary>
    /// Resets a single palette entry to its default (for OSC 104).
    /// </summary>
    public void ResetColor(int index)
    {
        if ((uint)index < 256)
            _colors[index] = _defaults[index];
    }

    /// <summary>
    /// Resets all 256 palette entries to their defaults.
    /// </summary>
    public void ResetAll() =>
        Array.Copy(_defaults, _colors, 256);

    /// <summary>
    /// Creates a <see cref="Color"/> directly from RGB components.
    /// </summary>
    public static Color FromTrueColor(int r, int g, int b) =>
        new Color(
            Math.Clamp(r, 0, 255),
            Math.Clamp(g, 0, 255),
            Math.Clamp(b, 0, 255));

    private static void BuildDefaults(Color[] colors)
    {
        // Standard 16 ANSI colors (CTerm / CGA defaults)
        colors[0]  = new Color(0, 0, 0);         // Black
        colors[1]  = new Color(170, 0, 0);       // Red
        colors[2]  = new Color(0, 170, 0);       // Green
        colors[3]  = new Color(170, 85, 0);      // Brown
        colors[4]  = new Color(0, 0, 170);       // Blue
        colors[5]  = new Color(170, 0, 170);     // Magenta
        colors[6]  = new Color(0, 170, 170);     // Cyan
        colors[7]  = new Color(170, 170, 170);   // White (light gray)
        colors[8]  = new Color(85, 85, 85);      // Bright Black (dark gray)
        colors[9]  = new Color(255, 85, 85);     // Bright Red
        colors[10] = new Color(85, 255, 85);     // Bright Green
        colors[11] = new Color(255, 255, 85);    // Bright Yellow
        colors[12] = new Color(85, 85, 255);     // Bright Blue
        colors[13] = new Color(255, 85, 255);    // Bright Magenta
        colors[14] = new Color(85, 255, 255);    // Bright Cyan
        colors[15] = new Color(255, 255, 255);   // Bright White

        // 216-color cube (indices 16–231): r*36 + g*6 + b + 16
        for (int r = 0; r < 6; r++)
            for (int g = 0; g < 6; g++)
                for (int b = 0; b < 6; b++)
                    colors[16 + r * 36 + g * 6 + b] =
                        new Color(s_cubeSteps[r], s_cubeSteps[g], s_cubeSteps[b]);

        // 24 grayscale ramp (indices 232–255): 8 + 10*n
        for (int n = 0; n < 24; n++)
        {
            int v = 8 + 10 * n;
            colors[232 + n] = new Color(v, v, v);
        }
    }
}
