using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;

namespace SadConsole.Tests;

/// <summary>
/// Tests for ITerminalOutput interface, Writer.Output property, and TerminalConsole output integration.
/// Validates the response channel architecture: null = data-stream mode (silent),
/// set = interactive mode (responses sent through channel).
/// </summary>
[TestClass]
public class TerminalOutputTests
{
    [TestInitialize]
    public void Setup()
    {
        new BasicGameHost();
    }

    // ══════════════════════════════════════════════════════════════
    //  Test Infrastructure
    // ══════════════════════════════════════════════════════════════

    /// <summary>Mock ITerminalOutput that captures all written data for verification.</summary>
    private class MockTerminalOutput : ITerminalOutput
    {
        public List<byte[]> ByteWrites { get; } = new();
        public List<string> StringWrites { get; } = new();

        public void Write(byte[] data)
        {
            ByteWrites.Add(data);
        }

        public void Write(string text)
        {
            StringWrites.Add(text);
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  1. ITerminalOutput Contract (mock validates interface)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void MockOutput_WriteBytes_CapturesData()
    {
        var output = new MockTerminalOutput();
        byte[] data = new byte[] { 0x1b, 0x5b, 0x36, 0x6e }; // ESC[6n (DSR)
        output.Write(data);

        Assert.AreEqual(1, output.ByteWrites.Count);
        CollectionAssert.AreEqual(data, output.ByteWrites[0]);
    }

    [TestMethod]
    public void MockOutput_WriteString_CapturesText()
    {
        var output = new MockTerminalOutput();
        output.Write("\x1b[1;1R"); // CPR response

        Assert.AreEqual(1, output.StringWrites.Count);
        Assert.AreEqual("\x1b[1;1R", output.StringWrites[0]);
    }

    [TestMethod]
    public void MockOutput_MultipleWrites_AllCaptured()
    {
        var output = new MockTerminalOutput();
        output.Write("first");
        output.Write("second");
        output.Write(new byte[] { 0x41 });

        Assert.AreEqual(2, output.StringWrites.Count);
        Assert.AreEqual(1, output.ByteWrites.Count);
    }

    // ══════════════════════════════════════════════════════════════
    //  2. Writer.Output Property
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void WriterOutput_Default_IsNull()
    {
        var surface = new SadConsole.CellSurface(80, 25);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);

        Assert.IsNull(writer.Output);
    }

    [TestMethod]
    public void WriterOutput_CanBeSet()
    {
        var surface = new SadConsole.CellSurface(80, 25);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);
        var output = new MockTerminalOutput();

        writer.Output = output;

        Assert.AreSame(output, writer.Output);
    }

    [TestMethod]
    public void WriterOutput_CanBeCleared()
    {
        var surface = new SadConsole.CellSurface(80, 25);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);
        var output = new MockTerminalOutput();

        writer.Output = output;
        writer.Output = null;

        Assert.IsNull(writer.Output);
    }

    [TestMethod]
    public void WriterOutput_CanBeReplaced()
    {
        var surface = new SadConsole.CellSurface(80, 25);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);
        var output1 = new MockTerminalOutput();
        var output2 = new MockTerminalOutput();

        writer.Output = output1;
        writer.Output = output2;

        Assert.AreSame(output2, writer.Output);
    }

    // ══════════════════════════════════════════════════════════════
    //  3. Data-Stream Mode (Output = null → silent)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void DataStreamMode_NullOutput_NoExceptionOnFeed()
    {
        var surface = new SadConsole.CellSurface(80, 25);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);
        writer.Output = null;

        // Plain text — should not throw
        writer.Feed("Hello World");
    }

    [TestMethod]
    public void DataStreamMode_NullOutput_NoExceptionOnAnsiSequences()
    {
        var surface = new SadConsole.CellSurface(80, 25);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);
        writer.Output = null;

        // Color sequences — should not throw even without an output channel
        writer.Feed("\x1b[0m\x1b[31mHello\x1b[0m");
    }

    [TestMethod]
    public void DataStreamMode_NullOutput_TextStillRendered()
    {
        var surface = new SadConsole.CellSurface(80, 25);
        var writer = new Writer(surface, GameHost.Instance.EmbeddedFont);
        writer.Output = null;

        writer.Feed("AB");

        Assert.AreEqual((int)'A', surface[0, 0].Glyph);
        Assert.AreEqual((int)'B', surface[1, 0].Glyph);
    }

    // ══════════════════════════════════════════════════════════════
    //  4. TerminalConsole.Output Property (delegates to Writer.Output)
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void TerminalConsole_Output_Default_IsNull()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsNull(terminal.Output);
    }

    [TestMethod]
    public void TerminalConsole_Output_SetDelegatesToWriter()
    {
        var terminal = new TerminalConsole(80, 25);
        var output = new MockTerminalOutput();

        terminal.Output = output;

        Assert.AreSame(output, terminal.Writer.Output);
        Assert.AreSame(output, terminal.Output);
    }

    [TestMethod]
    public void TerminalConsole_Output_ClearDelegatesToWriter()
    {
        var terminal = new TerminalConsole(80, 25);
        var output = new MockTerminalOutput();

        terminal.Output = output;
        terminal.Output = null;

        Assert.IsNull(terminal.Writer.Output);
        Assert.IsNull(terminal.Output);
    }

    // ══════════════════════════════════════════════════════════════
    //  5. TerminalConsole.KeyboardEncoder
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void TerminalConsole_KeyboardEncoder_IsNotNull()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsNotNull(terminal.KeyboardEncoder);
    }

    [TestMethod]
    public void TerminalConsole_KeyboardEncoder_DefaultApplicationCursorKeys_IsFalse()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsFalse(terminal.KeyboardEncoder.ApplicationCursorKeys);
    }

    [TestMethod]
    public void TerminalConsole_KeyboardEncoder_DefaultNewLineMode_IsFalse()
    {
        var terminal = new TerminalConsole(80, 25);
        Assert.IsFalse(terminal.KeyboardEncoder.NewLineMode);
    }

    [TestMethod]
    public void TerminalConsole_DECCKM_Enable_SetsWriterState()
    {
        var terminal = new TerminalConsole(80, 25);

        // CSI ? 1 h enables DECCKM on the Writer's State
        terminal.Feed("\x1b[?1h");

        Assert.IsTrue(terminal.Writer.State.CursorKeyMode,
            "Writer.State.CursorKeyMode should be true after DECCKM enable");
    }

    [TestMethod]
    public void TerminalConsole_DECCKM_Disable_ClearsWriterState()
    {
        var terminal = new TerminalConsole(80, 25);

        terminal.Feed("\x1b[?1h"); // enable
        terminal.Feed("\x1b[?1l"); // disable

        Assert.IsFalse(terminal.Writer.State.CursorKeyMode,
            "Writer.State.CursorKeyMode should be false after DECCKM disable");
    }

    // ══════════════════════════════════════════════════════════════
    //  6. Multiple Instances — Isolation
    // ══════════════════════════════════════════════════════════════

    [TestMethod]
    public void MultipleInstances_IndependentEncoders()
    {
        var t1 = new TerminalConsole(80, 25);
        var t2 = new TerminalConsole(80, 25);

        t1.KeyboardEncoder.ApplicationCursorKeys = true;

        Assert.IsTrue(t1.KeyboardEncoder.ApplicationCursorKeys);
        Assert.IsFalse(t2.KeyboardEncoder.ApplicationCursorKeys);
    }

    [TestMethod]
    public void MultipleInstances_IndependentOutputs()
    {
        var t1 = new TerminalConsole(80, 25);
        var t2 = new TerminalConsole(80, 25);
        var output = new MockTerminalOutput();

        t1.Output = output;

        Assert.AreSame(output, t1.Output);
        Assert.IsNull(t2.Output);
    }

    [TestMethod]
    public void MultipleInstances_EncoderNotSameInstance()
    {
        var t1 = new TerminalConsole(80, 25);
        var t2 = new TerminalConsole(80, 25);

        Assert.AreNotSame(t1.KeyboardEncoder, t2.KeyboardEncoder);
    }
}
