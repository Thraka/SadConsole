using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SadRogue.Primitives;

namespace SadConsole.Readers;

/// <summary>
/// Represents a TheDraw ascii font. http://www.roysac.com/thedrawfonts-tdf.html
/// </summary>
public class TheDrawFont
{
    /// <summary>
    /// The type of font.
    /// </summary>
    public enum FontType
    {
        /// <summary>
        /// An outline font.
        /// </summary>
        Outline = 0,

        /// <summary>
        /// A block-based font.
        /// </summary>
        Block = 1,

        /// <summary>
        /// A font supporting color.
        /// </summary>
        Color = 2
    }

    /// <summary>
    /// The title of the font.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The type of font.
    /// </summary>
    public FontType Type { get; set; }

    /// <summary>
    /// The empty characters between letters when drawing.
    /// </summary>
    public int LetterSpacing { get; set; }

    /// <summary>
    /// An array indexed by character code, indicating if a glyph character is supported by the font. Characters 33 to 126 are supported, starting at index 0.
    /// </summary>
    public bool[] CharactersSupported { get; set; } = new bool[94];

    /// <summary>
    /// A dictionary keyed by character code representing each character in the font. Characters 33 to 126 are supported but indexed starting at 0.
    /// </summary>
    Dictionary<int, Character> Characters { get; set; } = new Dictionary<int, Character>();

    /// <summary>
    /// Returns <see langword="true"/> when the specified character glyph is supported by this font; otherwise <see langword="false"/>.
    /// </summary>
    /// <param name="glyph"></param>
    /// <returns>A boolean value indicating whether or not the specified glyph is supported.</returns>
    public bool IsCharacterSupported(int glyph)
    {
        int newGlyph = glyph - 33;
        if (newGlyph >= 0 && newGlyph < 94)
        {
            if (CharactersSupported[newGlyph])
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets a character from this font by character code.
    /// </summary>
    /// <param name="glyph">The character to get.</param>
    /// <returns>The specified character.</returns>
    /// <exception cref="InvalidOperationException">The character glyph index is valid but isn't included in this font.</exception>
    /// <exception cref="IndexOutOfRangeException">The character glyph index isn't in range. It must be between 33 and 126.</exception>
    public Character GetCharacter(int glyph)
    {
        int newGlyph = glyph - 33;
        if (newGlyph >= 0 && newGlyph < 94)
        {
            if (CharactersSupported[newGlyph])
            {
                return Characters[glyph];
            }

            throw new InvalidOperationException("Character not supported.");
        }

        throw new IndexOutOfRangeException("Invalid glyph index. Must be 33 through 126.");
    }

    /// <summary>
    /// Generates a surface from the specified glyph using a white foreground and black background for the individual glyphs of the character.
    /// </summary>
    /// <param name="glyph">The glyph index.</param>
    /// <returns>A surface of just the glyph. Width and height of the surface is based on the TheDraw's font.</returns>
    public CellSurface? GetSurface(int glyph)
    {
        return GetSurface(glyph, Color.White, Color.Black);
    }

    /// <summary>
    /// Generates a surface from the specified glyph using the specified foreground and background for the individual glyphs of the character.
    /// </summary>
    /// <param name="glyph">The glyph index.</param>
    /// <param name="alternateForeground">Foreground color used to draw the glyph.</param>
    /// <param name="alternateBackground">Background color used to draw the glyph.</param>
    /// <returns>A surface of just the glyph. Width and height of the surface is based on the TheDraw's font.</returns>
    public CellSurface? GetSurface(int glyph, Color alternateForeground, Color alternateBackground)
    {
        int newGlyph = glyph - 33;
        if (newGlyph >= 0 && newGlyph < 94)
        {
            if (CharactersSupported[newGlyph])
            {
                Character character = Characters[glyph];

                var surface = new CellSurface(character.Width, character.Rows.Length);

                surface.DefaultBackground = Color.Transparent;
                surface.DefaultForeground = Color.White;

                surface.Clear();

                int y = 0;

                for (int i = 0; i < character.Rows.Length; i++)
                {
                    CharacterRow row = character.Rows[i];
                    var x = 0;

                    for (int cIndex = 0; cIndex < row.Characters.Length; cIndex++)
                    {
                        CharacterSpot c = row.Characters[cIndex];
                        surface[x, y].Glyph = c.Character;

                        if (Type == FontType.Color)
                        {
                            surface[x, y].Foreground = c.Foreground;
                            surface[x, y].Background = c.Background;
                        }
                        else
                        {
                            surface[x, y].Foreground = alternateForeground;
                            surface[x, y].Background = alternateBackground;
                        }
                        x++;
                    }

                    y++;
                }

                return surface;
            }
        }

        return null;
    }

    public static IEnumerable<TheDrawFont> ReadFonts(string file)
    {
        List<TheDrawFont> fonts = new List<TheDrawFont>();

        byte[] all = System.IO.File.ReadAllBytes(file);

        using (var inputStream = System.IO.File.OpenRead(file))
        {
            using (var reader = new System.IO.BinaryReader(inputStream))
            {
                reader.ReadByte(); // Char 19
                string test = System.Text.UTF8Encoding.ASCII.GetString(reader.ReadBytes(18)); // TheDraw FONTS file
                reader.ReadByte(); // Char 26


                // Read fonts
                while (inputStream.Position != inputStream.Length)
                {
                    // Font header
                    byte[] start = reader.ReadBytes(4);
                    byte titleLength = reader.ReadByte();
                    string title = UTF8Encoding.ASCII.GetString(reader.ReadBytes(12).Take(titleLength).ToArray());
                    byte[] empty = reader.ReadBytes(4);
                    int fontType = reader.ReadByte();
                    int letterSpacing = reader.ReadByte();
                    int length = BitConverter.ToInt16(reader.ReadBytes(2), 0);


                    // Character glyph index
                    int[] characterOffsets = new int[94];
                    for (int i = 0; i < 94; i++)
                        characterOffsets[i] = BitConverter.ToInt16(reader.ReadBytes(2), 0);

                    byte[] characterData = reader.ReadBytes(length);

                    // Parse character data
                    List<Character> characters = new List<Character>();

                    for (int i = 0; i < characterData.Length; i++)
                    {
                        List<CharacterRow> rows = new List<CharacterRow>();

                        int characterOffset = i;
                        int charWidth = characterData[i];
                        int charHeight = characterData[++i];
                        byte lastByte = 0;

                        while (i != characterData.Length)
                        {
                            List<CharacterSpot> row = new List<CharacterSpot>();

                            while (true)
                            {
                                lastByte = characterData[++i];

                                // End of row but dont count height?
                                if (lastByte == 38)
                                {
                                    continue;
                                }

                                // Carriage return -- EOL
                                else if (lastByte == 13 || lastByte == 0)
                                {
                                    break;
                                }

                                var spot = new CharacterSpot();

                                // Is the font type an outline font? If so, we need to translate
                                if (fontType == 0)
                                {
                                    //65 - 79
                                    switch (lastByte)
                                    {
                                        case 65:    // A
                                            spot.Character = 205;
                                            break;
                                        case 66:    // B
                                            spot.Character = 196;
                                            break;
                                        case 67:    // C
                                            spot.Character = 179;
                                            break;
                                        case 68:    // D
                                            spot.Character = 186;
                                            break;
                                        case 69:    // E
                                            spot.Character = 213;
                                            break;
                                        case 70:    // F
                                            spot.Character = 187;
                                            break;
                                        case 71:    // G
                                            spot.Character = 214;
                                            break;
                                        case 72:    // H
                                            spot.Character = 191;
                                            break;
                                        case 73:    // I
                                            spot.Character = 200;
                                            break;
                                        case 74:    // J
                                            spot.Character = 190;
                                            break;
                                        case 75:    // K
                                            spot.Character = 192;
                                            break;
                                        case 76:    // L
                                            spot.Character = 189;
                                            break;
                                        case 77:    // M
                                            spot.Character = 181;
                                            break;
                                        case 78:    // N
                                            spot.Character = 199;
                                            break;
                                        case 79:    // O
                                            spot.Character = 247;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                    spot.Character = lastByte;

                                if (fontType == (int)FontType.Color)
                                {
                                    i++;
                                    byte fore = (byte)(characterData[i] & 0x0F);
                                    byte back = (byte)((characterData[i] & 0xF0) >> 4);
                                    spot.Foreground = GetColor(fore, true);
                                    spot.Background = GetColor(back, false);
                                }

                                row.Add(spot);
                            }

                            rows.Add(new CharacterRow() { Characters = row.ToArray() });

                            if (lastByte == 0)
                                break;
                        }

                        int indexOf = Array.IndexOf(characterOffsets, characterOffset);
                        while (indexOf != -1)
                        {
                            characters.Add(new Character() { GlyphIndex = indexOf + 33, Width = charWidth, Rows = rows.ToArray() });
                            indexOf = Array.IndexOf(characterOffsets, characterOffset, indexOf + 1);
                        }
                    }

                    fonts.Add(new TheDrawFont()
                    {
                        Title = title,
                        LetterSpacing = letterSpacing,
                        Type = (FontType)fontType,
                        CharactersSupported = characterOffsets.Select(c => c != -1).ToArray(),
                        Characters = characters.ToDictionary(c => c.GlyphIndex)
                    }
                    );
                }

            }
        }

        return fonts;
    }

    private static Color GetColor(byte color, bool isForeground)
    {
        switch (color)
        {
            case 0:
                return Color.AnsiBlack;
            case 1:
                return Color.AnsiBlue;
            case 2:
                return Color.AnsiGreen;
            case 3:
                return Color.AnsiCyan;
            case 4:
                return Color.AnsiRed;
            case 5:
                return Color.AnsiMagenta;
            case 6:
                return Color.AnsiYellow;
            case 7:
                return Color.AnsiWhite;
            case 8:
                return Color.AnsiBlackBright;
            case 9:
                return Color.AnsiBlueBright;
            case 10:
                return Color.AnsiGreenBright;
            case 11:
                return Color.AnsiCyanBright;
            case 12:
                return Color.AnsiRedBright;
            case 13:
                return Color.AnsiMagentaBright;
            case 14:
                return Color.AnsiYellowBright;
            case 15:
                return Color.AnsiWhiteBright;
            default:
                return Color.Transparent;
        }
    }

    public struct CharacterSpot
    {
        public int Character;
        public Color Foreground;
        public Color Background;
    }

    public struct CharacterRow
    {
        public CharacterSpot[] Characters;
    }

    public struct Character
    {
        public int GlyphIndex;
        public int Width;
        public CharacterRow[] Rows;
    }
}
