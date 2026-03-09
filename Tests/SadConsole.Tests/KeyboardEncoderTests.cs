using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Input;
using SadConsole.Terminal;

namespace SadConsole.Tests;

/// <summary>
/// Tests for SadConsole.Terminal.KeyboardEncoder — converts SadConsole keyboard input
/// into ANSI/VT terminal escape sequences. Covers printable characters, special keys,
/// control codes, modifier combinations, and mode switches (DECCKM, LNM, backspace).
/// </summary>
[TestClass]
public class KeyboardEncoderTests
{
    private KeyboardEncoder _encoder;
    private KeyboardTestHost _host;
    private MockKeyboardState _mockState;

    [TestInitialize]
    public void Setup()
    {
        _host = new KeyboardTestHost();
        _mockState = _host.MockState;
        _encoder = new KeyboardEncoder();
    }

    // ══════════════════════════════════════════════════════════════
    //  Test Infrastructure
    // ══════════════════════════════════════════════════════════════

    /// <summary>Mock keyboard state for controlling which keys are pressed.</summary>
    private class MockKeyboardState : IKeyboardState
    {
        private readonly List<Keys> _downKeys = new();

        public bool CapsLock { get; set; }
        public bool NumLock { get; set; }

        public void SetKeyDown(Keys key)
        {
            if (!_downKeys.Contains(key)) _downKeys.Add(key);
        }

        public void ClearKeys()
        {
            _downKeys.Clear();
            CapsLock = false;
            NumLock = false;
        }

        public bool IsKeyDown(Keys key) => _downKeys.Contains(key);
        public bool IsKeyUp(Keys key) => !_downKeys.Contains(key);
        public Keys[] GetPressedKeys() => _downKeys.ToArray();
        public void Refresh() { }
    }

    /// <summary>Game host that provides a controllable keyboard state for testing.</summary>
    private class KeyboardTestHost : BasicGameHost
    {
        public MockKeyboardState MockState { get; } = new();
        public override IKeyboardState GetKeyboardState() => MockState;
    }

    /// <summary>
    /// Simulates a single key press with optional modifiers, returns the encoded string
    /// via EncodeSingleKey for precise per-key testing.
    /// </summary>
    private string? EncodeKey(Keys key, bool shift = false, bool ctrl = false, bool alt = false)
    {
        _mockState.ClearKeys();
        if (shift) _mockState.SetKeyDown(Keys.LeftShift);
        if (ctrl) _mockState.SetKeyDown(Keys.LeftControl);
        if (alt) _mockState.SetKeyDown(Keys.LeftAlt);
        _mockState.SetKeyDown(key);

        var keyboard = new Input.Keyboard();
        keyboard.Update(TimeSpan.Zero);

        var asciiKey = keyboard.KeysPressed.FirstOrDefault(k => k.Key == key);
        return _encoder.EncodeSingleKey(asciiKey, keyboard);
    }

    /// <summary>
    /// Simulates a full keyboard frame via Encode(), returns the raw encoded bytes.
    /// </summary>
    private byte[] EncodeFrame(Keys[] keys, bool shift = false, bool ctrl = false, bool alt = false)
    {
        _mockState.ClearKeys();
        if (shift) _mockState.SetKeyDown(Keys.LeftShift);
        if (ctrl) _mockState.SetKeyDown(Keys.LeftControl);
        if (alt) _mockState.SetKeyDown(Keys.LeftAlt);
        foreach (var k in keys) _mockState.SetKeyDown(k);

        var keyboard = new Input.Keyboard();
        keyboard.Update(TimeSpan.Zero);

        return _encoder.Encode(keyboard);
    }

    // ══════════════════════════════════════════════════════════════
    //  1. Defaults
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Defaults_ApplicationCursorKeys_IsFalse()
    {
        Assert.IsFalse(_encoder.ApplicationCursorKeys);
    }

    [TestMethod]
    public void Defaults_NewLineMode_IsFalse()
    {
        Assert.IsFalse(_encoder.NewLineMode);
    }

    [TestMethod]
    public void Defaults_BackspaceSendsDel_IsTrue()
    {
        Assert.IsTrue(_encoder.BackspaceSendsDel);
    }

    // ══════════════════════════════════════════════════════════════
    //  2. Printable Characters
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Printable_LowercaseLetter_ProducesSelf()
    {
        Assert.AreEqual("a", EncodeKey(Keys.A));
    }

    [TestMethod]
    public void Printable_ShiftedLetter_ProducesUppercase()
    {
        Assert.AreEqual("A", EncodeKey(Keys.A, shift: true));
    }

    [TestMethod]
    public void Printable_Digit_ProducesSelf()
    {
        Assert.AreEqual("1", EncodeKey(Keys.D1));
    }

    [TestMethod]
    public void Printable_Space_ProducesSpace()
    {
        Assert.AreEqual(" ", EncodeKey(Keys.Space));
    }

    [TestMethod]
    public void Printable_EqualsSign_ProducesSelf()
    {
        Assert.AreEqual("=", EncodeKey(Keys.OemPlus));
    }

    [TestMethod]
    public void Printable_ShiftedSymbol_ProducesShiftedVariant()
    {
        Assert.AreEqual("+", EncodeKey(Keys.OemPlus, shift: true));
    }

    // ══════════════════════════════════════════════════════════════
    //  3. Arrow Keys — Normal Mode
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Arrow_Up_NormalMode_ProducesCSIA()
    {
        Assert.AreEqual("\x1b[A", EncodeKey(Keys.Up));
    }

    [TestMethod]
    public void Arrow_Down_NormalMode_ProducesCSIB()
    {
        Assert.AreEqual("\x1b[B", EncodeKey(Keys.Down));
    }

    [TestMethod]
    public void Arrow_Right_NormalMode_ProducesCSIC()
    {
        Assert.AreEqual("\x1b[C", EncodeKey(Keys.Right));
    }

    [TestMethod]
    public void Arrow_Left_NormalMode_ProducesCSID()
    {
        Assert.AreEqual("\x1b[D", EncodeKey(Keys.Left));
    }

    // ══════════════════════════════════════════════════════════════
    //  4. Arrow Keys — Application Cursor Mode (DECCKM)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Arrow_Up_ApplicationMode_ProducesSS3A()
    {
        _encoder.ApplicationCursorKeys = true;
        Assert.AreEqual("\x1bOA", EncodeKey(Keys.Up));
    }

    [TestMethod]
    public void Arrow_Down_ApplicationMode_ProducesSS3B()
    {
        _encoder.ApplicationCursorKeys = true;
        Assert.AreEqual("\x1bOB", EncodeKey(Keys.Down));
    }

    [TestMethod]
    public void Arrow_Right_ApplicationMode_ProducesSS3C()
    {
        _encoder.ApplicationCursorKeys = true;
        Assert.AreEqual("\x1bOC", EncodeKey(Keys.Right));
    }

    [TestMethod]
    public void Arrow_Left_ApplicationMode_ProducesSS3D()
    {
        _encoder.ApplicationCursorKeys = true;
        Assert.AreEqual("\x1bOD", EncodeKey(Keys.Left));
    }

    // ══════════════════════════════════════════════════════════════
    //  5. Function Keys (F1–F12)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void FKey_F1_ProducesSS3P()
    {
        Assert.AreEqual("\x1bOP", EncodeKey(Keys.F1));
    }

    [TestMethod]
    public void FKey_F2_ProducesSS3Q()
    {
        Assert.AreEqual("\x1bOQ", EncodeKey(Keys.F2));
    }

    [TestMethod]
    public void FKey_F3_ProducesSS3R()
    {
        Assert.AreEqual("\x1bOR", EncodeKey(Keys.F3));
    }

    [TestMethod]
    public void FKey_F4_ProducesSS3S()
    {
        Assert.AreEqual("\x1bOS", EncodeKey(Keys.F4));
    }

    [TestMethod]
    public void FKey_F5_ProducesCSI15Tilde()
    {
        Assert.AreEqual("\x1b[15~", EncodeKey(Keys.F5));
    }

    [TestMethod]
    public void FKey_F6_ProducesCSI17Tilde()
    {
        Assert.AreEqual("\x1b[17~", EncodeKey(Keys.F6));
    }

    [TestMethod]
    public void FKey_F7_ProducesCSI18Tilde()
    {
        Assert.AreEqual("\x1b[18~", EncodeKey(Keys.F7));
    }

    [TestMethod]
    public void FKey_F8_ProducesCSI19Tilde()
    {
        Assert.AreEqual("\x1b[19~", EncodeKey(Keys.F8));
    }

    [TestMethod]
    public void FKey_F9_ProducesCSI20Tilde()
    {
        Assert.AreEqual("\x1b[20~", EncodeKey(Keys.F9));
    }

    [TestMethod]
    public void FKey_F10_ProducesCSI21Tilde()
    {
        Assert.AreEqual("\x1b[21~", EncodeKey(Keys.F10));
    }

    [TestMethod]
    public void FKey_F11_ProducesCSI23Tilde()
    {
        Assert.AreEqual("\x1b[23~", EncodeKey(Keys.F11));
    }

    [TestMethod]
    public void FKey_F12_ProducesCSI24Tilde()
    {
        Assert.AreEqual("\x1b[24~", EncodeKey(Keys.F12));
    }

    // ══════════════════════════════════════════════════════════════
    //  6. Navigation Keys
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Nav_Home_ProducesCSIH()
    {
        Assert.AreEqual("\x1b[H", EncodeKey(Keys.Home));
    }

    [TestMethod]
    public void Nav_End_ProducesCSIF()
    {
        Assert.AreEqual("\x1b[F", EncodeKey(Keys.End));
    }

    [TestMethod]
    public void Nav_Insert_ProducesCSI2Tilde()
    {
        Assert.AreEqual("\x1b[2~", EncodeKey(Keys.Insert));
    }

    [TestMethod]
    public void Nav_Delete_ProducesCSI3Tilde()
    {
        Assert.AreEqual("\x1b[3~", EncodeKey(Keys.Delete));
    }

    [TestMethod]
    public void Nav_PageUp_ProducesCSI5Tilde()
    {
        Assert.AreEqual("\x1b[5~", EncodeKey(Keys.PageUp));
    }

    [TestMethod]
    public void Nav_PageDown_ProducesCSI6Tilde()
    {
        Assert.AreEqual("\x1b[6~", EncodeKey(Keys.PageDown));
    }

    // ══════════════════════════════════════════════════════════════
    //  7. Control Characters
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Control_Enter_ProducesCR()
    {
        Assert.AreEqual("\r", EncodeKey(Keys.Enter));
    }

    [TestMethod]
    public void Control_Tab_ProducesHT()
    {
        Assert.AreEqual("\t", EncodeKey(Keys.Tab));
    }

    [TestMethod]
    public void Control_Escape_ProducesESC()
    {
        Assert.AreEqual("\x1b", EncodeKey(Keys.Escape));
    }

    [TestMethod]
    public void Control_Backspace_Default_ProducesDEL()
    {
        Assert.AreEqual("\x7f", EncodeKey(Keys.Back));
    }

    // ══════════════════════════════════════════════════════════════
    //  8. Ctrl+Letter — Control Codes (0x01–0x1A)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void CtrlLetter_A_Produces0x01()
    {
        Assert.AreEqual("\x01", EncodeKey(Keys.A, ctrl: true));
    }

    [TestMethod]
    public void CtrlLetter_C_Produces0x03()
    {
        Assert.AreEqual("\x03", EncodeKey(Keys.C, ctrl: true));
    }

    [TestMethod]
    public void CtrlLetter_L_Produces0x0C()
    {
        Assert.AreEqual("\x0c", EncodeKey(Keys.L, ctrl: true));
    }

    [TestMethod]
    public void CtrlLetter_Z_Produces0x1A()
    {
        Assert.AreEqual("\x1a", EncodeKey(Keys.Z, ctrl: true));
    }

    // ══════════════════════════════════════════════════════════════
    //  9. Mode Toggles
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void NewLineMode_Enter_ProducesCRLF()
    {
        _encoder.NewLineMode = true;
        Assert.AreEqual("\r\n", EncodeKey(Keys.Enter));
    }

    [TestMethod]
    public void NewLineMode_Off_Enter_ProducesCR()
    {
        _encoder.NewLineMode = false;
        Assert.AreEqual("\r", EncodeKey(Keys.Enter));
    }

    [TestMethod]
    public void BackspaceMode_False_ProducesBS()
    {
        _encoder.BackspaceSendsDel = false;
        Assert.AreEqual("\x08", EncodeKey(Keys.Back));
    }

    [TestMethod]
    public void BackspaceMode_True_ProducesDEL()
    {
        _encoder.BackspaceSendsDel = true;
        Assert.AreEqual("\x7f", EncodeKey(Keys.Back));
    }

    // ══════════════════════════════════════════════════════════════
    //  10. Modifier Combinations (xterm-style CSI 1;modifier X)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Modifier_ShiftUp_ProducesModifier2()
    {
        Assert.AreEqual("\x1b[1;2A", EncodeKey(Keys.Up, shift: true));
    }

    [TestMethod]
    public void Modifier_AltUp_ProducesModifier3()
    {
        Assert.AreEqual("\x1b[1;3A", EncodeKey(Keys.Up, alt: true));
    }

    [TestMethod]
    public void Modifier_CtrlUp_ProducesModifier5()
    {
        Assert.AreEqual("\x1b[1;5A", EncodeKey(Keys.Up, ctrl: true));
    }

    [TestMethod]
    public void Modifier_ShiftCtrlUp_ProducesModifier6()
    {
        Assert.AreEqual("\x1b[1;6A", EncodeKey(Keys.Up, shift: true, ctrl: true));
    }

    [TestMethod]
    public void Modifier_ShiftAltCtrlUp_ProducesModifier8()
    {
        Assert.AreEqual("\x1b[1;8A", EncodeKey(Keys.Up, shift: true, ctrl: true, alt: true));
    }

    [TestMethod]
    public void Modifier_AltChar_ProducesEscPrefix()
    {
        // \x1b followed by 'a' — use \u001b to avoid C# \x consuming the hex digit 'a'
        Assert.AreEqual("\u001ba", EncodeKey(Keys.A, alt: true));
    }

    [TestMethod]
    public void Modifier_ShiftTab_ProducesBacktab()
    {
        Assert.AreEqual("\x1b[Z", EncodeKey(Keys.Tab, shift: true));
    }

    [TestMethod]
    public void Modifier_CtrlHome_ProducesModifiedCSI()
    {
        Assert.AreEqual("\x1b[1;5H", EncodeKey(Keys.Home, ctrl: true));
    }

    [TestMethod]
    public void Modifier_ShiftDelete_ProducesModifiedTilde()
    {
        Assert.AreEqual("\x1b[3;2~", EncodeKey(Keys.Delete, shift: true));
    }

    [TestMethod]
    public void Modifier_ShiftF1_ProducesModifiedCSI()
    {
        Assert.AreEqual("\x1b[1;2P", EncodeKey(Keys.F1, shift: true));
    }

    [TestMethod]
    public void Modifier_CtrlF5_ProducesModifiedTilde()
    {
        Assert.AreEqual("\x1b[15;5~", EncodeKey(Keys.F5, ctrl: true));
    }

    // ══════════════════════════════════════════════════════════════
    //  11. Application Mode — Home/End
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Nav_Home_ApplicationMode_ProducesSS3H()
    {
        _encoder.ApplicationCursorKeys = true;
        Assert.AreEqual("\x1bOH", EncodeKey(Keys.Home));
    }

    [TestMethod]
    public void Nav_End_ApplicationMode_ProducesSS3F()
    {
        _encoder.ApplicationCursorKeys = true;
        Assert.AreEqual("\x1bOF", EncodeKey(Keys.End));
    }

    // ══════════════════════════════════════════════════════════════
    //  12. Edge Cases
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Edge_ModifierKeyAlone_ProducesNull()
    {
        Assert.IsNull(EncodeKey(Keys.LeftShift));
    }

    [TestMethod]
    public void Edge_LeftControlAlone_ProducesNull()
    {
        Assert.IsNull(EncodeKey(Keys.LeftControl));
    }

    [TestMethod]
    public void Edge_LeftAltAlone_ProducesNull()
    {
        Assert.IsNull(EncodeKey(Keys.LeftAlt));
    }

    [TestMethod]
    public void Edge_WindowsKeyAlone_ProducesNull()
    {
        Assert.IsNull(EncodeKey(Keys.LeftWindows));
    }

    [TestMethod]
    public void Edge_EmptyKeyboard_ProducesEmptyArray()
    {
        _mockState.ClearKeys();
        var keyboard = new Input.Keyboard();
        keyboard.Update(TimeSpan.Zero);

        byte[] result = _encoder.Encode(keyboard);
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void Edge_ApplicationMode_WithModifier_UsesCSINotSS3()
    {
        _encoder.ApplicationCursorKeys = true;
        Assert.AreEqual("\x1b[1;2A", EncodeKey(Keys.Up, shift: true));
    }

    [TestMethod]
    public void Edge_CapsLock_ProducesUppercase()
    {
        _mockState.ClearKeys();
        _mockState.CapsLock = true;
        _mockState.SetKeyDown(Keys.A);

        var keyboard = new Input.Keyboard();
        keyboard.Update(TimeSpan.Zero);

        var asciiKey = keyboard.KeysPressed.First(k => k.Key == Keys.A);
        string? result = _encoder.EncodeSingleKey(asciiKey, keyboard);
        Assert.AreEqual("A", result);
    }

    // ══════════════════════════════════════════════════════════════
    //  13. Encode() — Full Frame Integration
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void Encode_SinglePrintableKey_ReturnsCorrectBytes()
    {
        byte[] result = EncodeFrame(new[] { Keys.A });
        string text = Encoding.UTF8.GetString(result);
        Assert.AreEqual("a", text);
    }

    [TestMethod]
    public void Encode_SpecialKey_ReturnsEscapeSequence()
    {
        byte[] result = EncodeFrame(new[] { Keys.Up });
        string text = Encoding.UTF8.GetString(result);
        Assert.AreEqual("\x1b[A", text);
    }

    [TestMethod]
    public void Encode_CtrlLetter_ModifierSkipped_OnlyControlCodeEncoded()
    {
        byte[] result = EncodeFrame(new[] { Keys.A }, ctrl: true);
        string text = Encoding.UTF8.GetString(result);
        Assert.AreEqual("\x01", text);
    }

    [TestMethod]
    public void Encode_MultipleKeys_AllEncoded()
    {
        byte[] result = EncodeFrame(new[] { Keys.A, Keys.B });
        string text = Encoding.UTF8.GetString(result);
        Assert.IsTrue(text.Contains('a'), "Should contain 'a'");
        Assert.IsTrue(text.Contains('b'), "Should contain 'b'");
        Assert.AreEqual(2, text.Length);
    }
}
