using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;

namespace SadConsole.Tests
{
    /// <summary>
    /// Records all handler calls for assertion in tests.
    /// Implements ITerminalHandler — the contract the parser dispatches to.
    /// </summary>
    internal class MockTerminalHandler : ITerminalHandler
    {
        public enum CallType { Print, C0Control, EscDispatch, CsiDispatch, OscDispatch, DcsDispatch }

        public record HandlerCall(CallType Type)
        {
            // OnPrint
            public char PrintChar { get; init; }

            // OnC0Control
            public byte ControlByte { get; init; }

            // OnEscDispatch
            public byte EscIntermediate { get; init; }
            public byte EscFinal { get; init; }

            // OnCsiDispatch
            public IReadOnlyList<int> CsiParams { get; init; }
            public IReadOnlyList<byte> CsiIntermediates { get; init; }
            public byte CsiFinal { get; init; }
            public byte? CsiPrivatePrefix { get; init; }

            // OnOscDispatch
            public string OscPayload { get; init; }

            // OnDcsDispatch
            public IReadOnlyList<int> DcsParams { get; init; }
            public IReadOnlyList<byte> DcsIntermediates { get; init; }
            public byte DcsFinal { get; init; }
            public string DcsPayload { get; init; }
        }

        public List<HandlerCall> Calls { get; } = new();

        public void OnPrint(char ch)
        {
            Calls.Add(new HandlerCall(CallType.Print) { PrintChar = ch });
        }

        public void OnC0Control(byte b)
        {
            Calls.Add(new HandlerCall(CallType.C0Control) { ControlByte = b });
        }

        public void OnEscDispatch(byte intermediate, byte final_)
        {
            Calls.Add(new HandlerCall(CallType.EscDispatch) { EscIntermediate = intermediate, EscFinal = final_ });
        }

        public void OnCsiDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte final, byte? privatePrefix)
        {
            Calls.Add(new HandlerCall(CallType.CsiDispatch)
            {
                CsiParams = parameters.ToArray(),
                CsiIntermediates = intermediates.ToArray(),
                CsiFinal = final,
                CsiPrivatePrefix = privatePrefix
            });
        }

        public void OnOscDispatch(ReadOnlySpan<byte> payload)
        {
            Calls.Add(new HandlerCall(CallType.OscDispatch)
            {
                OscPayload = Encoding.UTF8.GetString(payload)
            });
        }

        public void OnDcsDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte final_, ReadOnlySpan<byte> payload)
        {
            Calls.Add(new HandlerCall(CallType.DcsDispatch)
            {
                DcsParams = parameters.ToArray(),
                DcsIntermediates = intermediates.ToArray(),
                DcsFinal = final_,
                DcsPayload = Encoding.UTF8.GetString(payload)
            });
        }

        /// <summary>
        /// Filters recorded calls by type.
        /// </summary>
        public IReadOnlyList<HandlerCall> OfType(CallType type) =>
            Calls.Where(c => c.Type == type).ToList();

        public void Clear() => Calls.Clear();
    }

    [TestClass]
    public class TerminalParserTests
    {
        private MockTerminalHandler _handler;
        private Parser _parser;

        [TestInitialize]
        public void Setup()
        {
            _handler = new MockTerminalHandler();
            _parser = new Parser(_handler);
        }

        // Helper: feed a string as UTF-8 bytes
        private void Feed(string s)
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            _parser.Feed(data);
        }

        // Helper: feed raw bytes
        private void FeedBytes(params byte[] data)
        {
            _parser.Feed(data);
        }

        // ========================================================================
        // 1. Printable Characters (0x20-0x7E) → OnPrint
        // ========================================================================

        [TestMethod]
        public void PrintableAscii_Space()
        {
            FeedBytes(0x20);
            Assert.AreEqual(1, _handler.Calls.Count);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[0].Type);
            Assert.AreEqual(' ', _handler.Calls[0].PrintChar);
        }

        [TestMethod]
        public void PrintableAscii_Tilde()
        {
            FeedBytes(0x7E);
            Assert.AreEqual(1, _handler.Calls.Count);
            Assert.AreEqual('~', _handler.Calls[0].PrintChar);
        }

        [TestMethod]
        public void PrintableAscii_AllRange()
        {
            for (byte b = 0x20; b <= 0x7E; b++)
                FeedBytes(b);

            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.AreEqual(0x7E - 0x20 + 1, prints.Count);

            for (int i = 0; i < prints.Count; i++)
                Assert.AreEqual((char)(0x20 + i), prints[i].PrintChar);
        }

        [TestMethod]
        public void PrintableAscii_HelloWorld()
        {
            Feed("Hello");
            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.AreEqual(5, prints.Count);
            Assert.AreEqual('H', prints[0].PrintChar);
            Assert.AreEqual('e', prints[1].PrintChar);
            Assert.AreEqual('l', prints[2].PrintChar);
            Assert.AreEqual('l', prints[3].PrintChar);
            Assert.AreEqual('o', prints[4].PrintChar);
        }

        // ========================================================================
        // 2. C0 Controls → OnC0Control
        // ========================================================================

        [TestMethod]
        public void C0Control_BEL()
        {
            FeedBytes(0x07); // BEL
            Assert.AreEqual(1, _handler.Calls.Count);
            Assert.AreEqual(MockTerminalHandler.CallType.C0Control, _handler.Calls[0].Type);
            Assert.AreEqual(0x07, _handler.Calls[0].ControlByte);
        }

        [TestMethod]
        public void C0Control_BS()
        {
            FeedBytes(0x08); // BS
            Assert.AreEqual(1, _handler.Calls.Count);
            Assert.AreEqual(0x08, _handler.Calls[0].ControlByte);
        }

        [TestMethod]
        public void C0Control_HT()
        {
            FeedBytes(0x09); // HT
            Assert.AreEqual(1, _handler.Calls.Count);
            Assert.AreEqual(0x09, _handler.Calls[0].ControlByte);
        }

        [TestMethod]
        public void C0Control_LF()
        {
            FeedBytes(0x0A); // LF
            Assert.AreEqual(1, _handler.Calls.Count);
            Assert.AreEqual(0x0A, _handler.Calls[0].ControlByte);
        }

        [TestMethod]
        public void C0Control_CR()
        {
            FeedBytes(0x0D); // CR
            Assert.AreEqual(1, _handler.Calls.Count);
            Assert.AreEqual(0x0D, _handler.Calls[0].ControlByte);
        }

        [TestMethod]
        public void C0Control_MixedWithPrintable()
        {
            // "A\nB\r" → Print('A'), C0(0x0A), Print('B'), C0(0x0D)
            Feed("A\nB\r");
            Assert.AreEqual(4, _handler.Calls.Count);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[0].Type);
            Assert.AreEqual('A', _handler.Calls[0].PrintChar);
            Assert.AreEqual(MockTerminalHandler.CallType.C0Control, _handler.Calls[1].Type);
            Assert.AreEqual(0x0A, _handler.Calls[1].ControlByte);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[2].Type);
            Assert.AreEqual('B', _handler.Calls[2].PrintChar);
            Assert.AreEqual(MockTerminalHandler.CallType.C0Control, _handler.Calls[3].Type);
            Assert.AreEqual(0x0D, _handler.Calls[3].ControlByte);
        }

        // ========================================================================
        // 3. Simple ESC Sequences → OnEscDispatch
        // ========================================================================

        [TestMethod]
        public void Esc_DECSC_Save()
        {
            // ESC 7 → OnEscDispatch(0, '7')
            FeedBytes(0x1B, (byte)'7');
            var escs = _handler.OfType(MockTerminalHandler.CallType.EscDispatch);
            Assert.AreEqual(1, escs.Count);
            Assert.AreEqual(0, escs[0].EscIntermediate);
            Assert.AreEqual((byte)'7', escs[0].EscFinal);
        }

        [TestMethod]
        public void Esc_DECRC_Restore()
        {
            // ESC 8 → OnEscDispatch(0, '8')
            FeedBytes(0x1B, (byte)'8');
            var escs = _handler.OfType(MockTerminalHandler.CallType.EscDispatch);
            Assert.AreEqual(1, escs.Count);
            Assert.AreEqual(0, escs[0].EscIntermediate);
            Assert.AreEqual((byte)'8', escs[0].EscFinal);
        }

        [TestMethod]
        public void Esc_RIS_Reset()
        {
            // ESC c → OnEscDispatch(0, 'c')
            FeedBytes(0x1B, (byte)'c');
            var escs = _handler.OfType(MockTerminalHandler.CallType.EscDispatch);
            Assert.AreEqual(1, escs.Count);
            Assert.AreEqual(0, escs[0].EscIntermediate);
            Assert.AreEqual((byte)'c', escs[0].EscFinal);
        }

        [TestMethod]
        public void Esc_NEL_NextLine()
        {
            // ESC E → OnEscDispatch(0, 'E')
            FeedBytes(0x1B, (byte)'E');
            var escs = _handler.OfType(MockTerminalHandler.CallType.EscDispatch);
            Assert.AreEqual(1, escs.Count);
            Assert.AreEqual(0, escs[0].EscIntermediate);
            Assert.AreEqual((byte)'E', escs[0].EscFinal);
        }

        [TestMethod]
        public void Esc_RI_ReverseIndex()
        {
            // ESC M → OnEscDispatch(0, 'M')
            FeedBytes(0x1B, (byte)'M');
            var escs = _handler.OfType(MockTerminalHandler.CallType.EscDispatch);
            Assert.AreEqual(1, escs.Count);
            Assert.AreEqual(0, escs[0].EscIntermediate);
            Assert.AreEqual((byte)'M', escs[0].EscFinal);
        }

        [TestMethod]
        public void Esc_HTS_HorizontalTabSet()
        {
            // ESC H → OnEscDispatch(0, 'H')
            FeedBytes(0x1B, (byte)'H');
            var escs = _handler.OfType(MockTerminalHandler.CallType.EscDispatch);
            Assert.AreEqual(1, escs.Count);
            Assert.AreEqual(0, escs[0].EscIntermediate);
            Assert.AreEqual((byte)'H', escs[0].EscFinal);
        }

        // ========================================================================
        // 4. CSI Sequences — Case Sensitivity (CRITICAL)
        // ========================================================================

        [TestMethod]
        public void Csi_CUP_UpperH()
        {
            // ESC [ H → CUP, final='H'
            FeedBytes(0x1B, (byte)'[', (byte)'H');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'H', csis[0].CsiFinal);
        }

        [TestMethod]
        public void Csi_DECSET_LowerH()
        {
            // ESC [ h → DECSET-related, final='h' — DIFFERENT from 'H'
            FeedBytes(0x1B, (byte)'[', (byte)'h');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'h', csis[0].CsiFinal);
        }

        [TestMethod]
        public void Csi_CaseSensitive_H_vs_h()
        {
            // Both in sequence: ESC [ H then ESC [ h
            FeedBytes(0x1B, (byte)'[', (byte)'H');
            FeedBytes(0x1B, (byte)'[', (byte)'h');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(2, csis.Count);
            Assert.AreEqual((byte)'H', csis[0].CsiFinal, "First should be uppercase H (CUP)");
            Assert.AreEqual((byte)'h', csis[1].CsiFinal, "Second should be lowercase h (DECSET)");
        }

        [TestMethod]
        public void Csi_DL_UpperM()
        {
            // ESC [ M → DL (delete line), final='M'
            FeedBytes(0x1B, (byte)'[', (byte)'M');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'M', csis[0].CsiFinal);
        }

        [TestMethod]
        public void Csi_SGR_LowerM()
        {
            // ESC [ m → SGR (select graphic rendition), final='m'
            FeedBytes(0x1B, (byte)'[', (byte)'m');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'m', csis[0].CsiFinal);
        }

        [TestMethod]
        public void Csi_CaseSensitive_M_vs_m()
        {
            FeedBytes(0x1B, (byte)'[', (byte)'M');
            FeedBytes(0x1B, (byte)'[', (byte)'m');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(2, csis.Count);
            Assert.AreEqual((byte)'M', csis[0].CsiFinal, "DL is uppercase M");
            Assert.AreEqual((byte)'m', csis[1].CsiFinal, "SGR is lowercase m");
        }

        [TestMethod]
        public void Csi_SU_UpperS()
        {
            // ESC [ S → SU (scroll up), final='S'
            FeedBytes(0x1B, (byte)'[', (byte)'S');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'S', csis[0].CsiFinal);
        }

        [TestMethod]
        public void Csi_SaveCursor_LowerS()
        {
            // ESC [ s → save cursor, final='s'
            FeedBytes(0x1B, (byte)'[', (byte)'s');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'s', csis[0].CsiFinal);
        }

        [TestMethod]
        public void Csi_CaseSensitive_S_vs_s()
        {
            FeedBytes(0x1B, (byte)'[', (byte)'S');
            FeedBytes(0x1B, (byte)'[', (byte)'s');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(2, csis.Count);
            Assert.AreEqual((byte)'S', csis[0].CsiFinal, "SU is uppercase S");
            Assert.AreEqual((byte)'s', csis[1].CsiFinal, "save cursor is lowercase s");
        }

        // ========================================================================
        // 5. CSI with Parameters
        // ========================================================================

        [TestMethod]
        public void Csi_Params_CUP_10_20()
        {
            // ESC [ 10 ; 20 H → params=[10, 20], final='H'
            Feed("\x1b[10;20H");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'H', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 10, 20 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_Params_CUU_5()
        {
            // ESC [ 5 A → params=[5], final='A'
            Feed("\x1b[5A");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'A', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 5 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_Params_SGR_NoParams()
        {
            // ESC [ m → params=[], final='m' (SGR reset)
            Feed("\x1b[m");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'m', csis[0].CsiFinal);
            Assert.AreEqual(0, csis[0].CsiParams.Count, "No params for bare SGR");
        }

        [TestMethod]
        public void Csi_Params_EmptyDefaults()
        {
            // ESC [ ; ; m → empty params default to 0: params=[0, 0, 0]
            Feed("\x1b[;;m");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'m', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 0, 0, 0 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_Params_SingleSemicolon()
        {
            // ESC [ ; H → params=[0, 0], final='H'
            Feed("\x1b[;H");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'H', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 0, 0 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_Params_MultipleValues()
        {
            // ESC [ 1;2;3;4;5 m → params=[1,2,3,4,5]
            Feed("\x1b[1;2;3;4;5m");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_Params_TrueColor_SGR()
        {
            // ESC [ 38;2;255;128;0 m → 256-color/truecolor SGR
            Feed("\x1b[38;2;255;128;0m");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'m', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 38, 2, 255, 128, 0 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        // ========================================================================
        // 6. CSI with Private Prefix
        // ========================================================================

        [TestMethod]
        public void Csi_PrivatePrefix_DECSET_AutoWrap()
        {
            // ESC [ ? 7 h → privatePrefix='?', params=[7], final='h'
            Feed("\x1b[?7h");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'?', csis[0].CsiPrivatePrefix);
            CollectionAssert.AreEqual(new[] { 7 }, (System.Collections.ICollection)csis[0].CsiParams);
            Assert.AreEqual((byte)'h', csis[0].CsiFinal);
        }

        [TestMethod]
        public void Csi_PrivatePrefix_DECRST_CursorHide()
        {
            // ESC [ ? 25 l → privatePrefix='?', params=[25], final='l'
            Feed("\x1b[?25l");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'?', csis[0].CsiPrivatePrefix);
            CollectionAssert.AreEqual(new[] { 25 }, (System.Collections.ICollection)csis[0].CsiParams);
            Assert.AreEqual((byte)'l', csis[0].CsiFinal);
        }

        [TestMethod]
        public void Csi_PrivatePrefix_Equals()
        {
            // ESC [ = 1 h → privatePrefix='=', params=[1], final='h'
            Feed("\x1b[=1h");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'=', csis[0].CsiPrivatePrefix);
            CollectionAssert.AreEqual(new[] { 1 }, (System.Collections.ICollection)csis[0].CsiParams);
            Assert.AreEqual((byte)'h', csis[0].CsiFinal);
        }

        [TestMethod]
        public void Csi_NoPrivatePrefix_DefaultIsZero()
        {
            // Regular CSI without private prefix: prefix should be 0
            Feed("\x1b[5A");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.IsNull(csis[0].CsiPrivatePrefix, "No private prefix should be null");
        }

        [TestMethod]
        public void Csi_PrivatePrefix_MultipleParams()
        {
            // ESC [ ? 1;3;4 h → set multiple DEC modes
            Feed("\x1b[?1;3;4h");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'?', csis[0].CsiPrivatePrefix);
            CollectionAssert.AreEqual(new[] { 1, 3, 4 }, (System.Collections.ICollection)csis[0].CsiParams);
            Assert.AreEqual((byte)'h', csis[0].CsiFinal);
        }

        // ========================================================================
        // 7. CSI with Intermediates
        // ========================================================================

        [TestMethod]
        public void Csi_Intermediate_SL_ScrollLeft()
        {
            // ESC [ 5 SP @ → SL: intermediates=[0x20], final='@'
            FeedBytes(0x1B, (byte)'[', (byte)'5', 0x20, (byte)'@');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'@', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new byte[] { 0x20 }, (System.Collections.ICollection)csis[0].CsiIntermediates);
            CollectionAssert.AreEqual(new[] { 5 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_Intermediate_SR_ScrollRight()
        {
            // ESC [ 5 SP A → SR: intermediates=[0x20], final='A'
            FeedBytes(0x1B, (byte)'[', (byte)'5', 0x20, (byte)'A');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'A', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new byte[] { 0x20 }, (System.Collections.ICollection)csis[0].CsiIntermediates);
            CollectionAssert.AreEqual(new[] { 5 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_Intermediate_NoIntermediates_IsEmpty()
        {
            // Standard CSI without intermediates
            Feed("\x1b[5A");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual(0, csis[0].CsiIntermediates.Count, "No intermediates expected");
        }

        [TestMethod]
        public void Csi_Intermediate_DECSCUSR()
        {
            // ESC [ 2 SP q → DECSCUSR (set cursor style): intermediates=[0x20], final='q'
            FeedBytes(0x1B, (byte)'[', (byte)'2', 0x20, (byte)'q');
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'q', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new byte[] { 0x20 }, (System.Collections.ICollection)csis[0].CsiIntermediates);
            CollectionAssert.AreEqual(new[] { 2 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        // ========================================================================
        // 8. OSC Strings → OnOscDispatch
        // ========================================================================

        [TestMethod]
        public void Osc_PaletteColor_ST()
        {
            // ESC ] 4;1;rgb:ff/00/00 ESC \ → OnOscDispatch
            byte[] data = new byte[] {
                0x1B, (byte)']',
                (byte)'4', (byte)';', (byte)'1', (byte)';',
                (byte)'r', (byte)'g', (byte)'b', (byte)':',
                (byte)'f', (byte)'f', (byte)'/', (byte)'0', (byte)'0', (byte)'/', (byte)'0', (byte)'0',
                0x1B, (byte)'\\'
            };
            FeedBytes(data);
            var oscs = _handler.OfType(MockTerminalHandler.CallType.OscDispatch);
            Assert.AreEqual(1, oscs.Count);
            Assert.AreEqual("4;1;rgb:ff/00/00", oscs[0].OscPayload);
        }

        [TestMethod]
        public void Osc_ResetColor_ST()
        {
            // ESC ] 104 ESC \ → OnOscDispatch with payload "104"
            byte[] data = new byte[] {
                0x1B, (byte)']',
                (byte)'1', (byte)'0', (byte)'4',
                0x1B, (byte)'\\'
            };
            FeedBytes(data);
            var oscs = _handler.OfType(MockTerminalHandler.CallType.OscDispatch);
            Assert.AreEqual(1, oscs.Count);
            Assert.AreEqual("104", oscs[0].OscPayload);
        }

        [TestMethod]
        public void Osc_TerminatedByBEL()
        {
            // ESC ] 0;My Title BEL → OSC terminated by BEL (0x07)
            byte[] data = new byte[] {
                0x1B, (byte)']',
                (byte)'0', (byte)';', (byte)'M', (byte)'y', (byte)' ', (byte)'T', (byte)'i', (byte)'t', (byte)'l', (byte)'e',
                0x07
            };
            FeedBytes(data);
            var oscs = _handler.OfType(MockTerminalHandler.CallType.OscDispatch);
            Assert.AreEqual(1, oscs.Count);
            Assert.AreEqual("0;My Title", oscs[0].OscPayload);
        }

        [TestMethod]
        public void Osc_EmptyPayload()
        {
            // ESC ] ESC \ → empty OSC
            FeedBytes(0x1B, (byte)']', 0x1B, (byte)'\\');
            var oscs = _handler.OfType(MockTerminalHandler.CallType.OscDispatch);
            Assert.AreEqual(1, oscs.Count);
            Assert.AreEqual("", oscs[0].OscPayload);
        }

        [TestMethod]
        public void Osc_SetWindowTitle()
        {
            // ESC ] 2;Hello World ESC \
            string title = "2;Hello World";
            List<byte> data = new List<byte> { 0x1B, (byte)']' };
            data.AddRange(Encoding.ASCII.GetBytes(title));
            data.Add(0x1B);
            data.Add((byte)'\\');
            FeedBytes(data.ToArray());

            var oscs = _handler.OfType(MockTerminalHandler.CallType.OscDispatch);
            Assert.AreEqual(1, oscs.Count);
            Assert.AreEqual(title, oscs[0].OscPayload);
        }

        // ========================================================================
        // 9. DCS Strings → OnDcsDispatch
        // ========================================================================

        [TestMethod]
        public void Dcs_BasicString()
        {
            // ESC P ... ESC \ → OnDcsDispatch
            // DCS q payload ESC \ (a Sixel-like string)
            List<byte> data = new List<byte> { 0x1B, (byte)'P', (byte)'q' };
            data.AddRange(Encoding.ASCII.GetBytes("payload data"));
            data.Add(0x1B);
            data.Add((byte)'\\');
            FeedBytes(data.ToArray());

            var dcss = _handler.OfType(MockTerminalHandler.CallType.DcsDispatch);
            Assert.AreEqual(1, dcss.Count);
        }

        [TestMethod]
        public void Dcs_WithParams()
        {
            // ESC P 1;2 q payload ESC \ → DCS with params
            List<byte> data = new List<byte>
            {
                0x1B, (byte)'P',
                (byte)'1', (byte)';', (byte)'2',
                (byte)'q'
            };
            data.AddRange(Encoding.ASCII.GetBytes("test"));
            data.Add(0x1B);
            data.Add((byte)'\\');
            FeedBytes(data.ToArray());

            var dcss = _handler.OfType(MockTerminalHandler.CallType.DcsDispatch);
            Assert.AreEqual(1, dcss.Count);
        }

        // ========================================================================
        // 10. Edge Cases
        // ========================================================================

        [TestMethod]
        public void EdgeCase_EscFollowedByInvalidByte_Recovery()
        {
            // ESC followed by an invalid byte (e.g., 0x80) should recover to ground
            // and subsequent characters should still be printed
            FeedBytes(0x1B, 0x80);
            FeedBytes((byte)'A');

            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.IsTrue(prints.Count >= 1, "Parser should recover and print 'A'");
            Assert.AreEqual('A', prints[prints.Count - 1].PrintChar);
        }

        [TestMethod]
        public void EdgeCase_CAN_AbortsSequence()
        {
            // CAN (0x18) aborts current escape sequence
            // ESC [ 5 CAN A → CAN aborts the CSI, then 'A' is printed
            FeedBytes(0x1B, (byte)'[', (byte)'5', 0x18, (byte)'A');

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(0, csis.Count, "CSI should not dispatch — aborted by CAN");

            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.IsTrue(prints.Any(p => p.PrintChar == 'A'), "'A' should be printed after CAN abort");
        }

        [TestMethod]
        public void EdgeCase_SUB_AbortsSequence()
        {
            // SUB (0x1A) aborts current escape sequence
            FeedBytes(0x1B, (byte)'[', (byte)'5', 0x1A, (byte)'B');

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(0, csis.Count, "CSI should not dispatch — aborted by SUB");

            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.IsTrue(prints.Any(p => p.PrintChar == 'B'), "'B' should be printed after SUB abort");
        }

        [TestMethod]
        public void EdgeCase_EscWithinSequence_StartsNew()
        {
            // ESC within a CSI sequence starts a new escape sequence
            // ESC [ 5 ESC [ 3 A → the first CSI is interrupted, a new CSI starts
            FeedBytes(0x1B, (byte)'[', (byte)'5', 0x1B, (byte)'[', (byte)'3', (byte)'A');

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            // The interrupted CSI should NOT dispatch. The new CSI [ 3 A should dispatch.
            Assert.AreEqual(1, csis.Count, "Only the second (complete) CSI should dispatch");
            Assert.AreEqual((byte)'A', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 3 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void EdgeCase_C0_WithinCsiSequence()
        {
            // C0 controls within CSI sequences should still be processed
            // ESC [ BEL 5 A → BEL dispatched as C0, CSI continues
            FeedBytes(0x1B, (byte)'[', 0x07, (byte)'5', (byte)'A');

            var c0s = _handler.OfType(MockTerminalHandler.CallType.C0Control);
            Assert.IsTrue(c0s.Any(c => c.ControlByte == 0x07), "BEL should be dispatched mid-CSI");

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count, "CSI should still complete after embedded C0");
            Assert.AreEqual((byte)'A', csis[0].CsiFinal);
        }

        [TestMethod]
        public void EdgeCase_VeryLargeParameter()
        {
            // Very large parameter value — parser should handle without overflow/crash
            Feed("\x1b[99999A");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'A', csis[0].CsiFinal);
            Assert.IsTrue(csis[0].CsiParams[0] >= 99999, "Large param value should be preserved");
        }

        [TestMethod]
        public void EdgeCase_ManyParameters()
        {
            // Many parameters — stress test the param buffer
            // 32 params: ESC [ 1;2;3;...;32 m
            string paramStr = string.Join(";", Enumerable.Range(1, 32));
            Feed($"\x1b[{paramStr}m");

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'m', csis[0].CsiFinal);
            // At minimum, the parser shouldn't crash. It may cap or collect all 32.
            Assert.IsTrue(csis[0].CsiParams.Count > 0, "Should have collected at least some params");
        }

        [TestMethod]
        public void EdgeCase_ManyParameters_Exact16()
        {
            // 16 parameters — common buffer size boundary
            string paramStr = string.Join(";", Enumerable.Range(1, 16));
            Feed($"\x1b[{paramStr}m");

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual(16, csis[0].CsiParams.Count);
            for (int i = 0; i < 16; i++)
                Assert.AreEqual(i + 1, csis[0].CsiParams[i]);
        }

        [TestMethod]
        public void EdgeCase_UTF8_MultiByte_2Byte()
        {
            // UTF-8 2-byte character: é (U+00E9) → 0xC3 0xA9
            _parser.Encoding = ParserEncoding.Utf8;
            FeedBytes(0xC3, 0xA9);
            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.AreEqual(1, prints.Count);
            Assert.AreEqual('é', prints[0].PrintChar);
        }

        [TestMethod]
        public void EdgeCase_UTF8_MultiByte_3Byte()
        {
            // UTF-8 3-byte character: ★ (U+2605) → 0xE2 0x98 0x85
            _parser.Encoding = ParserEncoding.Utf8;
            FeedBytes(0xE2, 0x98, 0x85);
            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.AreEqual(1, prints.Count);
            Assert.AreEqual('★', prints[0].PrintChar);
        }

        [TestMethod]
        public void EdgeCase_UTF8_MultiByte_4Byte()
        {
            // UTF-8 4-byte character: 😀 (U+1F600) → 0xF0 0x9F 0x98 0x80
            // This is outside BMP, so might be handled as a surrogate pair or single codepoint
            _parser.Encoding = ParserEncoding.Utf8;
            FeedBytes(0xF0, 0x9F, 0x98, 0x80);
            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.IsTrue(prints.Count >= 1, "Should produce at least one print call for emoji");
        }

        [TestMethod]
        public void EdgeCase_UTF8_MixedWithEscapes()
        {
            // UTF-8 character followed by an escape sequence
            // é ESC [ A → Print('é'), CSI dispatch
            _parser.Encoding = ParserEncoding.Utf8;
            FeedBytes(0xC3, 0xA9, 0x1B, (byte)'[', (byte)'A');
            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.AreEqual(1, prints.Count);
            Assert.AreEqual('é', prints[0].PrintChar);

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'A', csis[0].CsiFinal);
        }

        // ========================================================================
        // Parser Reset
        // ========================================================================

        [TestMethod]
        public void Reset_ClearsState()
        {
            // Start a partial CSI sequence, then reset, then feed new data
            FeedBytes(0x1B, (byte)'[', (byte)'5');
            _parser.Reset();
            Feed("A");

            // After reset, the incomplete CSI should not dispatch
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(0, csis.Count);

            // 'A' should be printed normally
            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.IsTrue(prints.Any(p => p.PrintChar == 'A'));
        }

        [TestMethod]
        public void Reset_AllowsReuse()
        {
            Feed("\x1b[5A");
            _handler.Clear();
            _parser.Reset();

            Feed("\x1b[10B");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'B', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 10 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        // ========================================================================
        // Single-byte Feed API
        // ========================================================================

        [TestMethod]
        public void Feed_SingleByte_PrintableChar()
        {
            _parser.Feed((byte)'X');
            Assert.AreEqual(1, _handler.Calls.Count);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[0].Type);
            Assert.AreEqual('X', _handler.Calls[0].PrintChar);
        }

        [TestMethod]
        public void Feed_SingleByte_C0Control()
        {
            _parser.Feed(0x07);
            Assert.AreEqual(1, _handler.Calls.Count);
            Assert.AreEqual(MockTerminalHandler.CallType.C0Control, _handler.Calls[0].Type);
            Assert.AreEqual(0x07, _handler.Calls[0].ControlByte);
        }

        [TestMethod]
        public void Feed_SingleByte_CsiSequence()
        {
            // Feed a CSI sequence one byte at a time
            _parser.Feed(0x1B);
            _parser.Feed((byte)'[');
            _parser.Feed((byte)'3');
            _parser.Feed((byte)'A');

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'A', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 3 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        // ========================================================================
        // Mixed / Integration Scenarios
        // ========================================================================

        [TestMethod]
        public void Mixed_PrintAndCsi()
        {
            // "AB\x1b[1mCD" → Print(A), Print(B), CSI(m,params=[1]), Print(C), Print(D)
            Feed("AB\x1b[1mCD");
            Assert.AreEqual(5, _handler.Calls.Count);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[0].Type);
            Assert.AreEqual('A', _handler.Calls[0].PrintChar);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[1].Type);
            Assert.AreEqual('B', _handler.Calls[1].PrintChar);
            Assert.AreEqual(MockTerminalHandler.CallType.CsiDispatch, _handler.Calls[2].Type);
            Assert.AreEqual((byte)'m', _handler.Calls[2].CsiFinal);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[3].Type);
            Assert.AreEqual('C', _handler.Calls[3].PrintChar);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[4].Type);
            Assert.AreEqual('D', _handler.Calls[4].PrintChar);
        }

        [TestMethod]
        public void Mixed_PrintAndEsc()
        {
            // "X\x1b7Y" → Print(X), EscDispatch(0,'7'), Print(Y)
            Feed("X\x1b" + "7Y");
            Assert.AreEqual(3, _handler.Calls.Count);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[0].Type);
            Assert.AreEqual('X', _handler.Calls[0].PrintChar);
            Assert.AreEqual(MockTerminalHandler.CallType.EscDispatch, _handler.Calls[1].Type);
            Assert.AreEqual((byte)'7', _handler.Calls[1].EscFinal);
            Assert.AreEqual(MockTerminalHandler.CallType.Print, _handler.Calls[2].Type);
            Assert.AreEqual('Y', _handler.Calls[2].PrintChar);
        }

        [TestMethod]
        public void Mixed_MultipleCsiSequences()
        {
            // ESC[31m ESC[42m → two CSI dispatches
            Feed("\x1b[31m\x1b[42m");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(2, csis.Count);
            Assert.AreEqual((byte)'m', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 31 }, (System.Collections.ICollection)csis[0].CsiParams);
            Assert.AreEqual((byte)'m', csis[1].CsiFinal);
            CollectionAssert.AreEqual(new[] { 42 }, (System.Collections.ICollection)csis[1].CsiParams);
        }

        [TestMethod]
        public void Mixed_CrLfWithCsi()
        {
            // "\x1b[31mHello\r\n" → CSI, prints, C0s
            Feed("\x1b[31mHello\r\n");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);

            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.AreEqual(5, prints.Count); // H, e, l, l, o

            var c0s = _handler.OfType(MockTerminalHandler.CallType.C0Control);
            Assert.AreEqual(2, c0s.Count); // CR, LF
        }

        [TestMethod]
        public void Mixed_ComplexAnsiArt()
        {
            // Typical ANSI art preamble: reset + set colors + print
            Feed("\x1b[0m\x1b[38;5;196m*\x1b[0m");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(3, csis.Count);

            // First: SGR reset
            Assert.AreEqual((byte)'m', csis[0].CsiFinal);
            Assert.AreEqual(0, csis[0].CsiParams.Count > 0 ? csis[0].CsiParams[0] : 0);

            // Second: 256-color foreground
            Assert.AreEqual((byte)'m', csis[1].CsiFinal);
            CollectionAssert.AreEqual(new[] { 38, 5, 196 }, (System.Collections.ICollection)csis[1].CsiParams);

            // Third: SGR reset
            Assert.AreEqual((byte)'m', csis[2].CsiFinal);

            // Print
            var prints = _handler.OfType(MockTerminalHandler.CallType.Print);
            Assert.AreEqual(1, prints.Count);
            Assert.AreEqual('*', prints[0].PrintChar);
        }

        // ========================================================================
        // Additional CSI Final Characters
        // ========================================================================

        [TestMethod]
        public void Csi_ICH_InsertCharacter()
        {
            // ESC [ 3 @ → ICH: final='@', params=[3]
            Feed("\x1b[3@");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'@', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 3 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_CUD_CursorDown()
        {
            Feed("\x1b[5B");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'B', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 5 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_CUF_CursorForward()
        {
            Feed("\x1b[10C");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'C', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 10 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_CUB_CursorBack()
        {
            Feed("\x1b[2D");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'D', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 2 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_ED_EraseDisplay()
        {
            Feed("\x1b[2J");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'J', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 2 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_EL_EraseLine()
        {
            Feed("\x1b[1K");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'K', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 1 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_IL_InsertLine()
        {
            Feed("\x1b[3L");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'L', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 3 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_DCH_DeleteCharacter()
        {
            Feed("\x1b[4P");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'P', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 4 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_DECSTBM_ScrollMargins()
        {
            // ESC [ 5;20 r → set scrolling region
            Feed("\x1b[5;20r");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'r', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 5, 20 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_CHA_CursorColumn()
        {
            Feed("\x1b[15G");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'G', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 15 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_CNL_CursorNextLine()
        {
            Feed("\x1b[3E");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'E', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 3 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Csi_CPL_CursorPrevLine()
        {
            Feed("\x1b[2F");
            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'F', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 2 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        // ========================================================================
        // Incremental Feeding (span boundary tests)
        // ========================================================================

        [TestMethod]
        public void Feed_SplitAcrossSpans_CsiSequence()
        {
            // Feed a CSI sequence in two separate chunks
            FeedBytes(0x1B, (byte)'[');
            FeedBytes((byte)'1', (byte)'0', (byte)'A');

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'A', csis[0].CsiFinal);
            CollectionAssert.AreEqual(new[] { 10 }, (System.Collections.ICollection)csis[0].CsiParams);
        }

        [TestMethod]
        public void Feed_SplitAcrossSpans_OscSequence()
        {
            // Feed an OSC sequence in multiple chunks
            FeedBytes(0x1B, (byte)']');
            FeedBytes((byte)'0', (byte)';');
            FeedBytes((byte)'T', (byte)'e', (byte)'s', (byte)'t');
            FeedBytes(0x1B, (byte)'\\');

            var oscs = _handler.OfType(MockTerminalHandler.CallType.OscDispatch);
            Assert.AreEqual(1, oscs.Count);
            Assert.AreEqual("0;Test", oscs[0].OscPayload);
        }

        [TestMethod]
        public void Feed_SplitAtEsc()
        {
            // ESC sent alone, then [ and rest follows
            FeedBytes(0x1B);
            FeedBytes((byte)'[', (byte)'m');

            var csis = _handler.OfType(MockTerminalHandler.CallType.CsiDispatch);
            Assert.AreEqual(1, csis.Count);
            Assert.AreEqual((byte)'m', csis[0].CsiFinal);
        }

        // ========================================================================
        // DEL character (0x7F) — should be ignored in most states
        // ========================================================================

        [TestMethod]
        public void DEL_IsIgnored()
        {
            // DEL (0x7F) should generally be ignored, not printed or dispatched as C0
            FeedBytes(0x7F);
            Assert.AreEqual(0, _handler.Calls.Count, "DEL (0x7F) should be ignored");
        }

        // ========================================================================
        // Empty input
        // ========================================================================

        [TestMethod]
        public void Feed_EmptySpan()
        {
            _parser.Feed(ReadOnlySpan<byte>.Empty);
            Assert.AreEqual(0, _handler.Calls.Count);
        }
    }
}
