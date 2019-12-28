using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SadRogue.Primitives;
using SadConsole;

namespace SadConsoleEditor.TheDraw
{
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

    public class Font
    {
        public enum FontType
        {
            Outline = 0,
            Block = 1,
            Color = 2
        }

        public string Title;
        public FontType Type;
        public int LetterSpacing;
        public bool[] CharactersSupported = new bool[94];
        Dictionary<int, Character> Characters = new Dictionary<int, Character>();
        
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

        public Character GetCharacter(int glyph)
        {
            int newGlyph = glyph - 33;
            if (newGlyph >= 0 && newGlyph < 94)
            {
                if (CharactersSupported[newGlyph])
                {
                    return Characters[glyph];
                }

                throw new Exception("Character not supported.");
            }

            throw new Exception("Invalid glyph index. Must be 33 through 93.");
        }

        public CellSurface GetSurface(int glyph)
        {
            return GetSurface(glyph, Color.White, Color.Black);
        }

        public CellSurface GetSurface(int glyph, Color alternateForeground, Color alternateBackground)
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

                    foreach (var row in character.Rows)
                    {
                        var x = 0;

                        foreach (var c in row.Characters)
                        {
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

        public static IEnumerable<Font> ReadFonts(string file)
        {
            List<Font> fonts = new List<Font>();

            byte[] all = System.IO.File.ReadAllBytes(file);

            using (var inputStream = Game.Instance.OpenStream(file))
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

                                while(true)
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

                        fonts.Add(new Font()
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
                    return ColorAnsi.Black;
                case 1:
                    return ColorAnsi.Blue;
                case 2:
                    return ColorAnsi.Green;
                case 3:
                    return ColorAnsi.Cyan;
                case 4:
                    return ColorAnsi.Red;
                case 5:
                    return ColorAnsi.Magenta;
                case 6:
                    return ColorAnsi.Yellow;
                case 7:
                    return ColorAnsi.White;
                case 8:
                    return ColorAnsi.BlackBright;
                case 9:
                    return ColorAnsi.BlueBright;
                case 10:
                    return ColorAnsi.GreenBright;
                case 11:
                    return ColorAnsi.CyanBright;
                case 12:
                    return ColorAnsi.RedBright;
                case 13:
                    return ColorAnsi.MagentaBright;
                case 14:
                    return ColorAnsi.YellowBright;
                case 15:
                    return ColorAnsi.WhiteBright;
                default:
                    return Color.Transparent;
            }
        }
    }
}
