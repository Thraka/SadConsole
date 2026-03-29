using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Input;
using SadConsole.UI.Controls;

namespace SadConsole.Tests.UI;

[TestClass]
public class TextBoxTests
{
    private KeyboardTestHost _host;
    private MockKeyboardState _mockState;

    [TestInitialize]
    public void Setup()
    {
        _host = new KeyboardTestHost();
        _mockState = _host.MockState;
    }

    /// <summary>
    /// Simulates a single key press and returns the Keyboard object
    /// with the key registered in KeysPressed.
    /// </summary>
    private Input.Keyboard SimulateKeyPress(Keys key, bool shift = false)
    {
        _mockState.ClearKeys();
        if (shift) _mockState.SetKeyDown(Keys.LeftShift);
        _mockState.SetKeyDown(key);

        var keyboard = new Input.Keyboard();
        keyboard.Update(TimeSpan.Zero);
        return keyboard;
    }

    /// <summary>
    /// Regression test for GitHub issue #372.
    /// Cancelling TextBox.TextChangedPreview must not corrupt internal
    /// caret state, which previously caused an ArgumentOutOfRangeException
    /// on the next key press.
    /// </summary>
    [TestMethod]
    public void TextChangedPreview_Cancel_DoesNotCorruptCaretState()
    {
        var textBox = new TextBox(20);

        // Cancel any text change that contains 'a'
        textBox.TextChangedPreview += (sender, e) =>
        {
            if (e.NewValue.Contains('a'))
                e.IsCancelled = true;
        };

        // First key press: type 'a' — should be cancelled
        var keyboard1 = SimulateKeyPress(Keys.A);
        textBox.ProcessKeyboard(keyboard1);

        // Text should remain empty since the change was cancelled
        Assert.AreEqual("", textBox.Text, "Text should remain empty after cancelled change");

        // Second key press: type 'b' — this previously threw ArgumentOutOfRangeException
        // because _caretPos was 1 but _text was still ""
        var keyboard2 = SimulateKeyPress(Keys.B);
        textBox.ProcessKeyboard(keyboard2);

        // 'b' should be accepted since it doesn't contain 'a'
        Assert.AreEqual("b", textBox.Text, "Text should be 'b' after accepted change");
    }

    /// <summary>
    /// Verifies that cancelling TextChangedPreview preserves the caret position
    /// so it remains consistent with the actual text.
    /// </summary>
    [TestMethod]
    public void TextChangedPreview_Cancel_PreservesCaretPosition()
    {
        var textBox = new TextBox(20);

        // Set initial text
        textBox.Text = "hello";

        // Cancel all further changes
        textBox.TextChangedPreview += (sender, e) =>
        {
            e.IsCancelled = true;
        };

        // Try to type a character — should be cancelled
        var keyboard = SimulateKeyPress(Keys.X);
        textBox.ProcessKeyboard(keyboard);

        // Text should not have changed
        Assert.AreEqual("hello", textBox.Text, "Text should remain 'hello' after cancelled change");

        // Caret position should still be valid for the text length
        Assert.IsTrue(textBox.CaretPosition <= textBox.Text.Length,
            $"CaretPosition ({textBox.CaretPosition}) should not exceed text length ({textBox.Text.Length})");
    }

    /// <summary>
    /// Verifies that multiple cancelled key presses don't accumulate
    /// caret drift, which would eventually cause a crash.
    /// </summary>
    [TestMethod]
    public void TextChangedPreview_MultipleCancels_DoNotAccumulateCaretDrift()
    {
        var textBox = new TextBox(20);

        // Cancel everything
        textBox.TextChangedPreview += (sender, e) =>
        {
            e.IsCancelled = true;
        };

        // Simulate 10 cancelled key presses
        for (int i = 0; i < 10; i++)
        {
            var keyboard = SimulateKeyPress(Keys.A);
            textBox.ProcessKeyboard(keyboard);
        }

        // Text should still be empty
        Assert.AreEqual("", textBox.Text, "Text should remain empty after all cancelled changes");

        // Caret should be at 0 (consistent with empty text)
        Assert.AreEqual(0, textBox.CaretPosition,
            "CaretPosition should be 0 for empty text after cancelled changes");
    }

    // ══════════════════════════════════════════════════════════════
    //  Test Infrastructure
    // ══════════════════════════════════════════════════════════════

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

    private class KeyboardTestHost : BasicGameHost
    {
        public MockKeyboardState MockState { get; } = new();
        public override IKeyboardState GetKeyboardState() => MockState;
    }
}
