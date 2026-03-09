using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;

namespace SadConsole.Terminal;

/// <summary>
/// Converts SadConsole keyboard input into ANSI/VT terminal escape sequences.
/// Standalone class — not tied to any specific surface or Writer instance.
/// </summary>
public class KeyboardEncoder
{
    // Reusable buffer to avoid per-key allocations
    private readonly StringBuilder _buffer = new();

    /// <summary>
    /// When <see langword="true"/>, arrow keys send application-mode sequences
    /// (<c>\x1bOA</c>) instead of normal-mode (<c>\x1b[A</c>).
    /// Mirrors DECCKM (DEC Cursor Key Mode).
    /// </summary>
    public bool ApplicationCursorKeys { get; set; }

    /// <summary>
    /// When <see langword="true"/>, Enter sends <c>\r\n</c> instead of <c>\r</c>.
    /// Mirrors LNM (Line Feed / New Line Mode).
    /// </summary>
    public bool NewLineMode { get; set; }

    /// <summary>
    /// When <see langword="true"/>, Backspace sends <c>0x08</c> (BS) instead of <c>0x7F</c> (DEL).
    /// Default is <see langword="false"/> (modern terminal behavior: Backspace = DEL).
    /// </summary>
    public bool BackspaceSendsDel { get; set; } = true;

    /// <summary>
    /// Encodes all pressed keys from the current keyboard frame into terminal byte sequences.
    /// </summary>
    /// <param name="keyboard">The keyboard state for this frame.</param>
    /// <returns>Encoded bytes, or an empty array if no keys produced output.</returns>
    public byte[] Encode(Input.Keyboard keyboard)
    {
        _buffer.Clear();

        foreach (AsciiKey key in keyboard.KeysPressed)
            EncodeKey(key, keyboard, _buffer);

        if (_buffer.Length == 0)
            return Array.Empty<byte>();

        return Encoding.UTF8.GetBytes(_buffer.ToString());
    }

    /// <summary>
    /// Encodes a single <see cref="AsciiKey"/> into its terminal escape sequence representation.
    /// </summary>
    /// <param name="key">The key to encode.</param>
    /// <param name="keyboard">The keyboard state (for modifier detection).</param>
    /// <returns>The encoded string, or <see langword="null"/> if the key produces no output.</returns>
    public string? EncodeSingleKey(AsciiKey key, Input.Keyboard keyboard)
    {
        var sb = new StringBuilder();
        EncodeKey(key, keyboard, sb);
        return sb.Length > 0 ? sb.ToString() : null;
    }

    private void EncodeKey(AsciiKey key, Input.Keyboard keyboard, StringBuilder sb)
    {
        bool ctrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
        bool alt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
        bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

        // Skip modifier keys themselves — they don't produce output
        if (IsModifierKey(key.Key))
            return;

        // Ctrl+letter → control codes 0x01–0x1A
        if (ctrl && key.Key >= Keys.A && key.Key <= Keys.Z)
        {
            int code = key.Key - Keys.A + 1;
            sb.Append((char)code);
            return;
        }

        // Special keys (arrows, function keys, etc.)
        string? special = EncodeSpecialKey(key.Key, ctrl, alt, shift);
        if (special != null)
        {
            sb.Append(special);
            return;
        }

        // Printable characters (AsciiKey already resolved shift/caps)
        if (key.Character != '\0')
        {
            if (alt)
            {
                // Alt+char sends ESC prefix followed by the character
                sb.Append('\x1b');
            }
            sb.Append(key.Character);
        }
    }

    private string? EncodeSpecialKey(Keys key, bool ctrl, bool alt, bool shift)
    {
        // Modifier parameter for xterm-style sequences: CSI 1;modifier X
        int modifier = ComputeModifier(ctrl, alt, shift);

        switch (key)
        {
            // Arrow keys — DECCKM affects these
            case Keys.Up:
                return EncodeArrowKey('A', modifier);
            case Keys.Down:
                return EncodeArrowKey('B', modifier);
            case Keys.Right:
                return EncodeArrowKey('C', modifier);
            case Keys.Left:
                return EncodeArrowKey('D', modifier);

            // Editing keys — CSI code ~
            case Keys.Insert:
                return EncodeTildeKey(2, modifier);
            case Keys.Delete:
                return EncodeTildeKey(3, modifier);
            case Keys.PageUp:
                return EncodeTildeKey(5, modifier);
            case Keys.PageDown:
                return EncodeTildeKey(6, modifier);

            // Home/End — CSI H / CSI F (xterm-style)
            case Keys.Home:
                return EncodeHomeEnd('H', modifier);
            case Keys.End:
                return EncodeHomeEnd('F', modifier);

            // Function keys
            case Keys.F1:
                return EncodeFunctionKey(11, 'P', modifier);
            case Keys.F2:
                return EncodeFunctionKey(12, 'Q', modifier);
            case Keys.F3:
                return EncodeFunctionKey(13, 'R', modifier);
            case Keys.F4:
                return EncodeFunctionKey(14, 'S', modifier);
            case Keys.F5:
                return EncodeTildeKey(15, modifier);
            case Keys.F6:
                return EncodeTildeKey(17, modifier);
            case Keys.F7:
                return EncodeTildeKey(18, modifier);
            case Keys.F8:
                return EncodeTildeKey(19, modifier);
            case Keys.F9:
                return EncodeTildeKey(20, modifier);
            case Keys.F10:
                return EncodeTildeKey(21, modifier);
            case Keys.F11:
                return EncodeTildeKey(23, modifier);
            case Keys.F12:
                return EncodeTildeKey(24, modifier);

            // Enter
            case Keys.Enter:
                return NewLineMode ? "\r\n" : "\r";

            // Backspace
            case Keys.Back:
                return BackspaceSendsDel ? "\x7f" : "\x08";

            // Tab
            case Keys.Tab:
                if (shift)
                    return "\x1b[Z"; // Reverse tab (backtab)
                return "\t";

            // Escape
            case Keys.Escape:
                return "\x1b";

            default:
                return null;
        }
    }

    private string EncodeArrowKey(char direction, int modifier)
    {
        if (modifier > 1)
            return $"\x1b[1;{modifier}{direction}";

        return ApplicationCursorKeys
            ? $"\x1bO{direction}"
            : $"\x1b[{direction}";
    }

    private static string EncodeTildeKey(int code, int modifier)
    {
        if (modifier > 1)
            return $"\x1b[{code};{modifier}~";
        return $"\x1b[{code}~";
    }

    private string EncodeHomeEnd(char final, int modifier)
    {
        if (modifier > 1)
            return $"\x1b[1;{modifier}{final}";

        return ApplicationCursorKeys
            ? $"\x1bO{final}"
            : $"\x1b[{final}";
    }

    private static string EncodeFunctionKey(int code, char ssFinal, int modifier)
    {
        // F1-F4 use SS3 (ESC O) in normal mode, CSI with modifier when modified
        if (modifier > 1)
            return $"\x1b[1;{modifier}{ssFinal}";
        return $"\x1bO{ssFinal}";
    }

    /// <summary>
    /// Computes the xterm modifier parameter.
    /// 1 = none, 2 = Shift, 3 = Alt, 4 = Shift+Alt, 5 = Ctrl, 6 = Shift+Ctrl, 7 = Alt+Ctrl, 8 = Shift+Alt+Ctrl.
    /// Returns 0 when no modifiers are pressed (caller checks > 1).
    /// </summary>
    private static int ComputeModifier(bool ctrl, bool alt, bool shift)
    {
        int mod = 1;
        if (shift) mod += 1;
        if (alt) mod += 2;
        if (ctrl) mod += 4;
        return mod;
    }

    private static bool IsModifierKey(Keys key) =>
        key is Keys.LeftShift or Keys.RightShift
            or Keys.LeftControl or Keys.RightControl
            or Keys.LeftAlt or Keys.RightAlt
            or Keys.LeftWindows or Keys.RightWindows
            or Keys.CapsLock or Keys.NumLock or Keys.Scroll;
}
