using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using SFMLKeys = SFML.Window.Keyboard.Key;

namespace SadConsole.Input
{
    public static class Extensions
    {
        public static SFMLKeys ToSFML(this Input.Keys key)
        {
            // A-Z
            if (key >= Input.Keys.A && key <= Input.Keys.Z) return (SFMLKeys)(key - Input.Keys.A);

            // NUMPAD
            if (key >= Input.Keys.NumPad0 && key <= Input.Keys.NumPad9) return (SFMLKeys)(key - ((int)Input.Keys.NumPad0 - (int)SFMLKeys.Numpad0));

            // F-KEYS
            if (key >= Input.Keys.F1 && key <= Input.Keys.F15) return (SFMLKeys)(key - ((int)Input.Keys.F1 - (int)SFMLKeys.F1));

            // Numbers
            if (key >= Input.Keys.D0 && key <= Input.Keys.D9) return (SFMLKeys)(key - ((int)Input.Keys.D0 - (int)SFMLKeys.Num0));


            return key switch
            {
                Input.Keys.Back => SFMLKeys.Backspace,
                Input.Keys.Tab => SFMLKeys.Tab,
                Input.Keys.Enter => SFMLKeys.Enter,
                Input.Keys.Escape => SFMLKeys.Escape,
                Input.Keys.Space => SFMLKeys.Space,
                Input.Keys.PageUp => SFMLKeys.PageUp,
                Input.Keys.PageDown => SFMLKeys.PageDown,
                Input.Keys.End => SFMLKeys.End,
                Input.Keys.Home => SFMLKeys.Home,
                Input.Keys.Left => SFMLKeys.Left,
                Input.Keys.Up => SFMLKeys.Up,
                Input.Keys.Right => SFMLKeys.Right,
                Input.Keys.Down => SFMLKeys.Down,
                Input.Keys.Insert => SFMLKeys.Insert,
                Input.Keys.Delete => SFMLKeys.Delete,
                Input.Keys.Multiply => SFMLKeys.Multiply,
                Input.Keys.Add => SFMLKeys.Add,
                Input.Keys.Subtract => SFMLKeys.Subtract,
                Input.Keys.Decimal => SFMLKeys.Period,
                Input.Keys.Divide => SFMLKeys.Divide,
                Input.Keys.LeftShift => SFMLKeys.LShift,
                Input.Keys.RightShift => SFMLKeys.RShift,
                Input.Keys.LeftControl => SFMLKeys.LControl,
                Input.Keys.RightControl => SFMLKeys.RControl,
                Input.Keys.LeftAlt => SFMLKeys.LAlt,
                Input.Keys.RightAlt => SFMLKeys.RAlt,
                Input.Keys.OemSemicolon => SFMLKeys.Semicolon,
                Input.Keys.OemPlus => SFMLKeys.Equal,
                Input.Keys.OemComma => SFMLKeys.Comma,
                Input.Keys.OemMinus => SFMLKeys.Hyphen,
                Input.Keys.OemPeriod => SFMLKeys.Period,
                Input.Keys.OemQuestion => SFMLKeys.Slash,
                Input.Keys.OemTilde => SFMLKeys.Tilde,
                Input.Keys.OemOpenBrackets => SFMLKeys.LBracket,
                Input.Keys.OemPipe => SFMLKeys.Backslash,
                Input.Keys.OemCloseBrackets => SFMLKeys.RBracket,
                Input.Keys.OemQuotes => SFMLKeys.Quote,
                Input.Keys.OemBackslash => SFMLKeys.Backslash,
                Input.Keys.Pause => SFMLKeys.Pause,
                _ => SFMLKeys.Unknown
            };

            /*
                Input.Keys.Select => SFMLKeys.Unknown,
                Input.Keys.CapsLock => SFMLKeys.Unknown,
                Input.Keys.Print => SFMLKeys.Unknown,
                Input.Keys.PrintScreen => SFMLKeys.Unknown,
                Input.Keys.Execute => SFMLKeys.Unknown,
                Input.Keys.Help => SFMLKeys.Unknown,
                Input.Keys.D0 => SFMLKeys.Unknown,
                Input.Keys.D1 => SFMLKeys.Unknown,
                Input.Keys.D2 => SFMLKeys.Unknown,
                Input.Keys.D3 => SFMLKeys.Unknown,
                Input.Keys.D4 => SFMLKeys.Unknown,
                Input.Keys.D5 => SFMLKeys.Unknown,
                Input.Keys.D6 => SFMLKeys.Unknown,
                Input.Keys.D7 => SFMLKeys.Unknown,
                Input.Keys.D8 => SFMLKeys.Unknown,
                Input.Keys.D9 => SFMLKeys.Unknown,
                Input.Keys.Separator => SFMLKeys.Unknown
                Input.Keys.F16 => SFMLKeys.Unknown,
                Input.Keys.F17 => SFMLKeys.Unknown,
                Input.Keys.F18 => SFMLKeys.Unknown,
                Input.Keys.F19 => SFMLKeys.Unknown,
                Input.Keys.F20 => SFMLKeys.Unknown,
                Input.Keys.F21 => SFMLKeys.Unknown,
                Input.Keys.F22 => SFMLKeys.Unknown,
                Input.Keys.F23 => SFMLKeys.Unknown,
                Input.Keys.F24 => SFMLKeys.Unknown,
                Input.Keys.Attn => SFMLKeys.Unknown,
                Input.Keys.Crsel => SFMLKeys.Unknown,
                Input.Keys.Exsel => SFMLKeys.Unknown,
                Input.Keys.EraseEof => SFMLKeys.Unknown,
                Input.Keys.Play => SFMLKeys.Unknown,
                Input.Keys.Zoom => SFMLKeys.Unknown,
                Input.Keys.Pa1 => SFMLKeys.Unknown,
                Input.Keys.OemClear => SFMLKeys.Unknown,
                Input.Keys.ChatPadGreen => SFMLKeys.Unknown,
                Input.Keys.ChatPadOrange => SFMLKeys.Unknown,
                Input.Keys.ImeConvert => SFMLKeys.Unknown,
                Input.Keys.ImeNoConvert => SFMLKeys.Unknown,
                Input.Keys.Kana => SFMLKeys.Unknown,
                Input.Keys.Kanji => SFMLKeys.Unknown,
                Input.Keys.OemAuto => SFMLKeys.Unknown,
                Input.Keys.OemCopy => SFMLKeys.Unknown,
                Input.Keys.OemEnlW => SFMLKeys.Unknown,
                Input.Keys.BrowserBack => SFMLKeys.Unknown,
                Input.Keys.BrowserForward => SFMLKeys.Unknown,
                Input.Keys.BrowserRefresh => SFMLKeys.Unknown,
                Input.Keys.BrowserStop => SFMLKeys.Unknown,
                Input.Keys.BrowserSearch => SFMLKeys.Unknown,
                Input.Keys.BrowserFavorites => SFMLKeys.Unknown,
                Input.Keys.BrowserHome => SFMLKeys.Unknown,
                Input.Keys.VolumeMute => SFMLKeys.Unknown,
                Input.Keys.VolumeDown => SFMLKeys.Unknown,
                Input.Keys.VolumeUp => SFMLKeys.Unknown,
                Input.Keys.MediaNextTrack => SFMLKeys.Unknown,
                Input.Keys.MediaPreviousTrack => SFMLKeys.Unknown,
                Input.Keys.MediaStop => SFMLKeys.Unknown,
                Input.Keys.MediaPlayPause => SFMLKeys.Unknown,
                Input.Keys.LaunchMail => SFMLKeys.Unknown,
                Input.Keys.SelectMedia => SFMLKeys.Unknown,
                Input.Keys.LaunchApplication1 => SFMLKeys.Unknown,
                Input.Keys.LaunchApplication2 => SFMLKeys.Unknown,
                Input.Keys.Oem8 => SFMLKeys.Unknown,
                Input.Keys.ProcessKey => SFMLKeys.Unknown,
                Input.Keys.LeftWindows => SFMLKeys.Unknown,
                Input.Keys.RightWindows => SFMLKeys.Unknown,
                Input.Keys.Apps => SFMLKeys.Unknown,
                Input.Keys.Sleep => SFMLKeys.Unknown,
                Input.Keys.NumLock => SFMLKeys.Unknown,
                Input.Keys.Scroll => SFMLKeys.Unknown,
            */
        }

        public static void SaveTexture(SFML.Graphics.Texture texture, string output)
        {
            using (var image = texture.CopyToImage())
                image.SaveToFile(output);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Input.Keys ToSadConsole(this SFMLKeys key)
        {
            // A-Z
            if (key >= SFMLKeys.A && key <= SFMLKeys.Z) return (int)key + Input.Keys.A;

            // NUMPAD
            if (key >= SFMLKeys.Numpad0 && key <= SFMLKeys.Numpad9) return (Input.Keys)(int)key + ((int)Input.Keys.NumPad0 - (int)SFMLKeys.Numpad0);

            // F-KEYS
            if (key >= SFMLKeys.F1 && key <= SFMLKeys.F15) return (Input.Keys)(int)key + ((int)Input.Keys.F1 - (int)SFMLKeys.F1);

            // Numbers
            if (key >= SFMLKeys.Num0 && key <= SFMLKeys.Num9) return (Input.Keys)(int)key + ((int)Input.Keys.D0 - (int)SFMLKeys.Num0);


            return key switch
            {
                SFMLKeys.Backspace => Input.Keys.Back,
                SFMLKeys.Tab => Input.Keys.Tab,
                SFMLKeys.Enter => Input.Keys.Enter,
                SFMLKeys.Escape => Input.Keys.Escape,
                SFMLKeys.Space => Input.Keys.Space,
                SFMLKeys.PageUp => Input.Keys.PageUp,
                SFMLKeys.PageDown => Input.Keys.PageDown,
                SFMLKeys.End => Input.Keys.End,
                SFMLKeys.Home => Input.Keys.Home,
                SFMLKeys.Left => Input.Keys.Left,
                SFMLKeys.Up => Input.Keys.Up,
                SFMLKeys.Right => Input.Keys.Right,
                SFMLKeys.Down => Input.Keys.Down,
                SFMLKeys.Insert => Input.Keys.Insert,
                SFMLKeys.Delete => Input.Keys.Delete,
                SFMLKeys.Multiply => Input.Keys.Multiply,
                SFMLKeys.Add => Input.Keys.Add,
                SFMLKeys.Subtract => Input.Keys.Subtract,
                SFMLKeys.Hyphen => Input.Keys.OemMinus,
                SFMLKeys.Equal => Input.Keys.OemPlus,
                SFMLKeys.Period => Input.Keys.OemPeriod,
                SFMLKeys.Divide => Input.Keys.Divide,
                SFMLKeys.LShift => Input.Keys.LeftShift,
                SFMLKeys.RShift => Input.Keys.RightShift,
                SFMLKeys.LControl => Input.Keys.LeftControl,
                SFMLKeys.RControl => Input.Keys.RightControl,
                SFMLKeys.LAlt => Input.Keys.LeftAlt,
                SFMLKeys.RAlt => Input.Keys.RightAlt,
                SFMLKeys.Semicolon => Input.Keys.OemSemicolon,
                SFMLKeys.Comma => Input.Keys.OemComma,
                SFMLKeys.Slash => Input.Keys.OemQuestion,
                SFMLKeys.Tilde => Input.Keys.OemTilde,
                SFMLKeys.LBracket => Input.Keys.OemOpenBrackets,
                SFMLKeys.Backslash => Input.Keys.OemPipe,
                SFMLKeys.RBracket => Input.Keys.OemCloseBrackets,
                SFMLKeys.Quote => Input.Keys.OemQuotes,
                SFMLKeys.Pause => Input.Keys.Pause,
                _ => Input.Keys.None
            };

            /*
                Already handled by the non OEM versions.
                SFMLKeys.Add => Input.Keys.OemPlus,
                SFMLKeys.Subtract => Input.Keys.OemMinus,
                SFMLKeys.Period => Input.Keys.OemPeriod,
                SFMLKeys.Backslash => Input.Keys.OemBackslash,
            */
        }
    }
}
