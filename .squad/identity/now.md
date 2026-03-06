---
updated_at: 2026-03-06T04:12:00Z
focus_area: Terminal overhaul Phase 1 complete — Parser, Writer, encoding support ready. Next: insert/delete/scroll ops, tab commands, DEC modes.
active_issues: []
---

# What We're Focused On

**Terminal Overhaul Status:**
- **Phase 0 (Parser):** ✅ Complete — ECMA-48 state machine with handler callbacks, zero-allocation dispatch. 87/87 tests pass.
- **Phase 1 (Writer):** ✅ Complete — Renders ANSI to ICellSurface with full SGR (256-color, truecolor, bold/dim/reverse), cursor control (CUU/CUD/CUF/CUB/CHA/CUP/ED/EL), tab stops, scroll regions, auto-wrap, encoding-aware glyph resolution (CP437/Unicode), line feed modes (Strict/Implicit). 160/160 tests pass.
- **Phase 1 Enhancements:** ✅ Complete — Measurer (ITerminalHandler that tracks dimensions), SauceRecord parser, auto-grow Writer support.
- **Parser Encoding Future:** Deferred enhancement — replace enum with System.Text.Encoding instance API

**Next Phases (5/6/8):**
- Phase 5: Insert/delete operations (ICH/DCH/IL/DL)
- Phase 6: Tab/repeat commands (CHT/CBT/TBC/HTS/REP)  
- Phase 8: DEC modes (DECSET/DECRST, auto-wrap, origin mode, cursor show/hide)

**Test Suite Status:** Parser: 87/87 ✅ | Writer: 160/160 ✅ | CellSurface: 228 tests ✅

