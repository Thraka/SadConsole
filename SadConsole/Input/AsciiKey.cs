using System.Collections.Generic;

namespace SadConsole.Input;

/// <summary>
/// Represents the state of a single key.
/// </summary>
public struct AsciiKey
{
    /// <summary>
    /// A link between two characters, one unshifted and the other shifted.
    /// </summary>
    /// <param name="Unshifted">The character when unshifted.</param>
    /// <param name="Shifted">The character when shifted.</param>
    public readonly record struct ShiftedCharacterMapping(char Unshifted, char Shifted);

    /// <summary>
    /// Associates a character glyph and a <see cref="Keys"/> value.
    /// </summary>
    /// <param name="CharacterGlyph">The number pad character.</param>
    /// <param name="Key">The key the character maps to.</param>
    /// <remarks>Used when the <see cref="Keys.NumLock"/> is active.</remarks>
    public readonly record struct CharacterKeyMapping(char CharacterGlyph, Keys Key);

    /// <summary>
    /// List of <see cref="Keys"/> to consider as shifted when capslock is on.
    /// </summary>
    public static readonly List<Keys> CapsLockedKeys = new()
    {
        Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
        Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
    };

    /// <summary>
    /// A dictionary that is keyed off of a <see cref="Keys"/> and associates that key with a character in a shifted and unshifted state.
    /// </summary>
    public static readonly Dictionary<Keys, ShiftedCharacterMapping> ShiftKeyMappings = new Dictionary<Keys, ShiftedCharacterMapping>
    {
        {Keys.OemComma, new ShiftedCharacterMapping(',', '<')},
        {Keys.OemMinus, new ShiftedCharacterMapping('-', '_')},
        {Keys.OemOpenBrackets, new ShiftedCharacterMapping('[', '{')},
        {Keys.OemCloseBrackets, new ShiftedCharacterMapping(']', '}')},
        {Keys.OemPeriod, new ShiftedCharacterMapping('.', '>')},
        {Keys.OemBackslash, new ShiftedCharacterMapping('\\', '|')},
        {Keys.OemPipe, new ShiftedCharacterMapping('\\', '|')},
        {Keys.OemPlus, new ShiftedCharacterMapping('=', '+')},
        {Keys.OemQuestion, new ShiftedCharacterMapping('/', '?')},
        {Keys.OemQuotes, new ShiftedCharacterMapping('\'', '"')},
        {Keys.OemSemicolon, new ShiftedCharacterMapping(';', ':')},
        {Keys.OemTilde, new ShiftedCharacterMapping('`', '~')},
        {Keys.Space, new ShiftedCharacterMapping(' ', ' ')},
        {Keys.Divide, new ShiftedCharacterMapping('/', '/')},
        {Keys.Multiply, new ShiftedCharacterMapping('*', '*')},
        {Keys.Subtract, new ShiftedCharacterMapping('-', '-')},
        {Keys.Add, new ShiftedCharacterMapping('+', '+')},

        {Keys.D0, new ShiftedCharacterMapping('0', ')')},
        {Keys.D1, new ShiftedCharacterMapping('1', '!')},
        {Keys.D2, new ShiftedCharacterMapping('2', '@')},
        {Keys.D3, new ShiftedCharacterMapping('3', '#')},
        {Keys.D4, new ShiftedCharacterMapping('4', '$')},
        {Keys.D5, new ShiftedCharacterMapping('5', '%')},
        {Keys.D6, new ShiftedCharacterMapping('6', '^')},
        {Keys.D7, new ShiftedCharacterMapping('7', '&')},
        {Keys.D8, new ShiftedCharacterMapping('8', '*')},
        {Keys.D9, new ShiftedCharacterMapping('9', '(')},

        {Keys.A, new ShiftedCharacterMapping('a', 'A')},
        {Keys.B, new ShiftedCharacterMapping('b', 'B')},
        {Keys.C, new ShiftedCharacterMapping('c', 'C')},
        {Keys.D, new ShiftedCharacterMapping('d', 'D')},
        {Keys.E, new ShiftedCharacterMapping('e', 'E')},
        {Keys.F, new ShiftedCharacterMapping('f', 'F')},
        {Keys.G, new ShiftedCharacterMapping('g', 'G')},
        {Keys.H, new ShiftedCharacterMapping('h', 'H')},
        {Keys.I, new ShiftedCharacterMapping('i', 'I')},
        {Keys.J, new ShiftedCharacterMapping('j', 'J')},
        {Keys.K, new ShiftedCharacterMapping('k', 'K')},
        {Keys.L, new ShiftedCharacterMapping('l', 'L')},
        {Keys.M, new ShiftedCharacterMapping('m', 'M')},
        {Keys.N, new ShiftedCharacterMapping('n', 'N')},
        {Keys.O, new ShiftedCharacterMapping('o', 'O')},
        {Keys.P, new ShiftedCharacterMapping('p', 'P')},
        {Keys.Q, new ShiftedCharacterMapping('q', 'Q')},
        {Keys.R, new ShiftedCharacterMapping('r', 'R')},
        {Keys.S, new ShiftedCharacterMapping('s', 'S')},
        {Keys.T, new ShiftedCharacterMapping('t', 'T')},
        {Keys.U, new ShiftedCharacterMapping('u', 'U')},
        {Keys.V, new ShiftedCharacterMapping('v', 'V')},
        {Keys.W, new ShiftedCharacterMapping('w', 'W')},
        {Keys.X, new ShiftedCharacterMapping('x', 'X')},
        {Keys.Y, new ShiftedCharacterMapping('y', 'Y')},
        {Keys.Z, new ShiftedCharacterMapping('z', 'Z')},
    };

    /// <summary>
    /// Dictionary that maps <see cref="Keys"/> usually triggered by the numberpad with a character and non-numpad key.
    /// </summary>
    public static readonly Dictionary<Keys, CharacterKeyMapping> NumberKeyMappings = new()
    {
        {Keys.Decimal, new CharacterKeyMapping('.', Keys.Delete)},
        {Keys.NumPad0, new CharacterKeyMapping('0', Keys.Insert)},
        {Keys.NumPad1, new CharacterKeyMapping('1', Keys.End)},
        {Keys.NumPad2, new CharacterKeyMapping('2', Keys.Down)},
        {Keys.NumPad3, new CharacterKeyMapping('3', Keys.PageDown)},
        {Keys.NumPad4, new CharacterKeyMapping('4', Keys.Left)},
        {Keys.NumPad5, new CharacterKeyMapping('5', Keys.D5)},
        {Keys.NumPad6, new CharacterKeyMapping('6', Keys.Right)},
        {Keys.NumPad7, new CharacterKeyMapping('7', Keys.Home)},
        {Keys.NumPad8, new CharacterKeyMapping('8', Keys.Up)},
        {Keys.NumPad9, new CharacterKeyMapping('9', Keys.PageUp)},
    };

    /// <summary>
    /// Remaps any incoming key to a combination of character and key.
    /// </summary>
    public static readonly Dictionary<Keys, Keys> KeyRemapping = new()
    {
        { Keys.None, Keys.None},
        { Keys.Back, Keys.Back},
        { Keys.Tab, Keys.Tab},
        { Keys.Enter, Keys.Enter},
        { Keys.CapsLock, Keys.CapsLock},
        { Keys.Escape, Keys.Escape},
        { Keys.Space, Keys.Space},
        { Keys.PageUp, Keys.PageUp},
        { Keys.PageDown, Keys.PageDown},
        { Keys.End, Keys.End},
        { Keys.Home, Keys.Home},
        { Keys.Left, Keys.Left},
        { Keys.Up, Keys.Up},
        { Keys.Right, Keys.Right},
        { Keys.Down, Keys.Down},
        { Keys.Select, Keys.Select},
        { Keys.Print, Keys.Print},
        { Keys.Execute, Keys.Execute},
        { Keys.PrintScreen, Keys.PrintScreen},
        { Keys.Insert, Keys.Insert},
        { Keys.Delete, Keys.Delete},
        { Keys.Help, Keys.Help},
        { Keys.D0, Keys.D0},
        { Keys.D1, Keys.D1},
        { Keys.D2, Keys.D2},
        { Keys.D3, Keys.D3},
        { Keys.D4, Keys.D4},
        { Keys.D5, Keys.D5},
        { Keys.D6, Keys.D6},
        { Keys.D7, Keys.D7},
        { Keys.D8, Keys.D8},
        { Keys.D9, Keys.D9},
        { Keys.A, Keys.A},
        { Keys.B, Keys.B},
        { Keys.C, Keys.C},
        { Keys.D, Keys.D},
        { Keys.E, Keys.E},
        { Keys.F, Keys.F},
        { Keys.G, Keys.G},
        { Keys.H, Keys.H},
        { Keys.I, Keys.I},
        { Keys.J, Keys.J},
        { Keys.K, Keys.K},
        { Keys.L, Keys.L},
        { Keys.M, Keys.M},
        { Keys.N, Keys.N},
        { Keys.O, Keys.O},
        { Keys.P, Keys.P},
        { Keys.Q, Keys.Q},
        { Keys.R, Keys.R},
        { Keys.S, Keys.S},
        { Keys.T, Keys.T},
        { Keys.U, Keys.U},
        { Keys.V, Keys.V},
        { Keys.W, Keys.W},
        { Keys.X, Keys.X},
        { Keys.Y, Keys.Y},
        { Keys.Z, Keys.Z},
        { Keys.LeftWindows, Keys.LeftWindows},
        { Keys.RightWindows, Keys.RightWindows},
        { Keys.Apps, Keys.Apps},
        { Keys.Sleep, Keys.Sleep},
        { Keys.NumPad0, Keys.NumPad0},
        { Keys.NumPad1, Keys.NumPad1},
        { Keys.NumPad2, Keys.NumPad2},
        { Keys.NumPad3, Keys.NumPad3},
        { Keys.NumPad4, Keys.NumPad4},
        { Keys.NumPad5, Keys.NumPad5},
        { Keys.NumPad6, Keys.NumPad6},
        { Keys.NumPad7, Keys.NumPad7},
        { Keys.NumPad8, Keys.NumPad8},
        { Keys.NumPad9, Keys.NumPad9},
        { Keys.Multiply, Keys.Multiply},
        { Keys.Add, Keys.Add},
        { Keys.Separator, Keys.Separator},
        { Keys.Subtract, Keys.Subtract},
        { Keys.Decimal, Keys.Decimal},
        { Keys.Divide, Keys.Divide},
        { Keys.F1, Keys.F1},
        { Keys.F2, Keys.F2},
        { Keys.F3, Keys.F3},
        { Keys.F4, Keys.F4},
        { Keys.F5, Keys.F5},
        { Keys.F6, Keys.F6 },
        { Keys.F7, Keys.F7 },
        { Keys.F8, Keys.F8 },
        { Keys.F9, Keys.F9 },
        { Keys.F10, Keys.F10 },
        { Keys.F11, Keys.F11 },
        { Keys.F12, Keys.F12 },
        { Keys.F13, Keys.F13 },
        { Keys.F14, Keys.F14 },
        { Keys.F15, Keys.F15 },
        { Keys.F16, Keys.F16 },
        { Keys.F17, Keys.F17 },
        { Keys.F18, Keys.F18 },
        { Keys.F19, Keys.F19 },
        { Keys.F20, Keys.F20 },
        { Keys.F21, Keys.F21 },
        { Keys.F22, Keys.F22 },
        { Keys.F23, Keys.F23 },
        { Keys.F24, Keys.F24 },
        { Keys.NumLock, Keys.NumLock },
        { Keys.Scroll, Keys.Scroll },
        { Keys.LeftShift, Keys.LeftShift },
        { Keys.RightShift, Keys.RightShift },
        { Keys.LeftControl, Keys.LeftControl },
        { Keys.RightControl, Keys.RightControl },
        { Keys.LeftAlt, Keys.LeftAlt },
        { Keys.RightAlt, Keys.RightAlt },
        { Keys.BrowserBack, Keys.BrowserBack },
        { Keys.BrowserForward, Keys.BrowserForward },
        { Keys.BrowserRefresh, Keys.BrowserRefresh },
        { Keys.BrowserStop, Keys.BrowserStop },
        { Keys.BrowserSearch, Keys.BrowserSearch },
        { Keys.BrowserFavorites, Keys.BrowserFavorites },
        { Keys.BrowserHome, Keys.BrowserHome },
        { Keys.VolumeMute, Keys.VolumeMute },
        { Keys.VolumeDown, Keys.VolumeDown },
        { Keys.VolumeUp, Keys.VolumeUp },
        { Keys.MediaNextTrack, Keys.MediaNextTrack },
        { Keys.MediaPreviousTrack, Keys.MediaPreviousTrack },
        { Keys.MediaStop, Keys.MediaStop },
        { Keys.MediaPlayPause, Keys.MediaPlayPause },
        { Keys.LaunchMail, Keys.LaunchMail },
        { Keys.SelectMedia, Keys.SelectMedia },
        { Keys.LaunchApplication1, Keys.LaunchApplication1 },
        { Keys.LaunchApplication2, Keys.LaunchApplication2 },
        { Keys.OemSemicolon, Keys.OemSemicolon },
        { Keys.OemPlus, Keys.OemPlus },
        { Keys.OemComma, Keys.OemComma },
        { Keys.OemMinus, Keys.OemMinus },
        { Keys.OemPeriod, Keys.OemPeriod },
        { Keys.OemQuestion, Keys.OemQuestion },
        { Keys.OemTilde, Keys.OemTilde },
        { Keys.OemOpenBrackets, Keys.OemOpenBrackets },
        { Keys.OemPipe, Keys.OemPipe },
        { Keys.OemCloseBrackets, Keys.OemCloseBrackets },
        { Keys.OemQuotes, Keys.OemQuotes },
        { Keys.Oem8, Keys.Oem8 },
        { Keys.OemBackslash, Keys.OemBackslash },
        { Keys.ProcessKey, Keys.ProcessKey },
        { Keys.Attn, Keys.Attn },
        { Keys.Crsel, Keys.Crsel },
        { Keys.Exsel, Keys.Exsel },
        { Keys.EraseEof, Keys.EraseEof },
        { Keys.Play, Keys.Play },
        { Keys.Zoom, Keys.Zoom },
        { Keys.Pa1, Keys.Pa1 },
        { Keys.OemClear, Keys.OemClear },
        { Keys.ChatPadGreen, Keys.ChatPadGreen },
        { Keys.ChatPadOrange, Keys.ChatPadOrange },
        { Keys.Pause, Keys.Pause },
        { Keys.ImeConvert, Keys.ImeConvert },
        { Keys.ImeNoConvert, Keys.ImeNoConvert },
        { Keys.Kana, Keys.Kana },
        { Keys.Kanji, Keys.Kanji },
        { Keys.OemAuto, Keys.OemAuto },
        { Keys.OemCopy, Keys.OemCopy },
        { Keys.OemEnlW, Keys.OemEnlW },
    };

    /// <summary>
    /// The key from MonoGame or XNA.
    /// </summary>
    public Keys Key;

    /// <summary>
    /// The keyboard character of the key.
    /// </summary>
    public char Character;

    /// <summary>
    /// Total time the key has been held.
    /// </summary>
    public System.TimeSpan TimeHeld;

    /// <summary>
    /// Tracks if the key was previously held when calculating the <see cref="Keyboard.InitialRepeatDelay"/>.
    /// </summary>
    public bool PostInitialDelay;

    /// <summary>
    /// Fills out the fields based on the key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="shiftPressed">Helps identify which <see cref="Character"/> to use while the key is pressed.</param>
    /// <param name="state">Keyboard state to read from.</param>
    public void Fill(Keys key, bool shiftPressed, IKeyboardState state)
    {
        // Remap the key; defaults to the same key
        Key = KeyRemapping[key];

        // Check if the caps lock is on, and the key is in the caps lock transform dictionary
        if (state.CapsLock && CapsLockedKeys.Contains(key))
        {
            ShiftedCharacterMapping shiftedKey = ShiftKeyMappings[Key];

            // If the caps lock is on, and the shift key is on, it's inverted to unshifted
            Character = state.CapsLock & !shiftPressed ? shiftedKey.Shifted : shiftedKey.Unshifted;
        }

        // Check if the key has different states based on shifed or not shifted
        else if (ShiftKeyMappings.ContainsKey(Key))
        {
            ShiftedCharacterMapping shiftedKey = ShiftKeyMappings[Key];
            Character = shiftPressed ? shiftedKey.Shifted : shiftedKey.Unshifted;
        }

        // Check if the key is in the numberpad keys and then check for numlock state
        else if (NumberKeyMappings.ContainsKey(Key))
        {
            CharacterKeyMapping casesCur = NumberKeyMappings[Key];

            Character = state.NumLock ? casesCur.CharacterGlyph : (char)0;
            Key =       state.NumLock ? Key                     : casesCur.Key;
        }

        // Otherwise, there is no associated character
        else
            Character = (char)0;
    }

    /// <summary>
    /// Shortcut to get the <see cref="AsciiKey"/> for a specific MonoGame/XNA <see cref="Keys"/> type. Shift is considered not pressed.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="state">Keyboar state to read from.</param>
    /// <returns>The <see cref="AsciiKey"/> of the <see cref="Keys"/>.</returns>
    public static AsciiKey Get(Keys key, IKeyboardState state)
    {
        AsciiKey asciiKey = new AsciiKey();
        asciiKey.Fill(key, false, state);
        return asciiKey;
    }

    /// <summary>
    /// Shortcut to get the <see cref="AsciiKey"/> for a specific MonoGame/XNA <see cref="Keys"/> type.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="shiftPressed">If shift should be considered pressed or not.</param>
    /// <param name="state">Keyboar state to read from.</param>
    /// <returns>The <see cref="AsciiKey"/> of the <see cref="Keys"/>.</returns>
    public static AsciiKey Get(Keys key, bool shiftPressed, IKeyboardState state)
    {
        AsciiKey asciiKey = new AsciiKey();
        asciiKey.Fill(key, shiftPressed, state);
        return asciiKey;
    }

    /// <summary>
    /// Checks if the two <see cref="AsciiKey"/> types use the same <see cref="Key"/> if the <see cref="Character"/> is 0. If the <see cref="Character"/> is not 0, the <see cref="Character"/> is compared.
    /// </summary>
    /// <param name="left">First item to compare.</param>
    /// <param name="right">Second item to compare.</param>
    /// <returns></returns>
    public static bool operator ==(AsciiKey left, AsciiKey right)
    {
        if (left.Character == (char)0 && left.Character == right.Character)
            return left.Key == right.Key;

        return left.Character == right.Character;
    }

    /// <summary>
    /// Gets a hashcode based on the key and character.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Key.GetHashCode();
            hash = hash * 23 + Character.GetHashCode();
            hash = hash * 23 + TimeHeld.GetHashCode();
            hash = hash * 23 + PostInitialDelay.GetHashCode();
            return hash;
        }
    }

    /// <summary>
    /// Compares if the <see cref="Character"/> field of two <see cref="AsciiKey"/> instances are the same.
    /// </summary>
    /// <param name="left">First item to compare.</param>
    /// <param name="right">Second item to compare.</param>
    /// <returns></returns>
    public static bool operator !=(AsciiKey left, AsciiKey right) => left.Character != right.Character;


    /// <summary>
    /// Checks if the a <see cref="AsciiKey"/> type uses the indicated <see cref="Key"/>.
    /// </summary>
    /// <param name="left">The <see cref="AsciiKey"/> to compare.</param>
    /// <param name="right">The <see cref="Key"/> to compare.</param>
    /// <returns>True when <see cref="AsciiKey.Key"/> matches.</returns>
    public static bool operator ==(AsciiKey left, Keys right) => left.Key == right;

    /// <summary>
    /// Checks if the a <see cref="AsciiKey"/> type does not use the indicated <see cref="Key"/>.
    /// </summary>
    /// <param name="left">The <see cref="AsciiKey"/> to compare.</param>
    /// <param name="right">The <see cref="Key"/> to compare.</param>
    /// <returns>True when <see cref="AsciiKey.Key"/> does not match.</returns>
    public static bool operator !=(AsciiKey left, Keys right) => left.Key != right;

    /// <summary>
    /// Checks if the a <see cref="AsciiKey"/> type uses the indicated <see cref="Key"/>.
    /// </summary>
    /// <param name="left">The <see cref="Key"/> to compare.</param>
    /// <param name="right">The <see cref="AsciiKey"/> to compare.</param>
    /// <returns>True when <see cref="AsciiKey.Key"/> matches.</returns>
    public static bool operator ==(Keys left, AsciiKey right) => left == right.Key;

    /// <summary>
    /// Checks if the a <see cref="AsciiKey"/> type does not use the indicated <see cref="Key"/>.
    /// </summary>
    /// <param name="left">The <see cref="Key"/> to compare.</param>
    /// <param name="right">The <see cref="AsciiKey"/> to compare.</param>
    /// <returns>True when <see cref="AsciiKey.Key"/> does not match.</returns>
    public static bool operator !=(Keys left, AsciiKey right) => left != right.Key;

    /// <summary>
    /// Compares references.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is AsciiKey key)
        {
            return key == this;
        }

        return base.Equals(obj);
    }
}
