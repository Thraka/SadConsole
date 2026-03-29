# TerminalCursor Tests — Implementation Notes

## Test File Created
**File:** `TerminalCursorTests.cs`
**Test Count:** 66 tests
**Framework:** MSTest

## What's Tested

### 1. TerminalCursor Defaults (3 tests)
- Position defaults to (0,0)
- IsVisible defaults to true
- Shape defaults to BlinkingBlock

### 2. CursorShape Enum (9 tests)
- All 6 enum values map to integers 1-6
- Odd values are blinking (1, 3, 5)
- Even values are steady (2, 4, 6)

### 3. Writer with Null Cursor / Data-Stream Mode (7 tests)
- Writer.Cursor is null by default
- Feeding ANSI data doesn't crash with null cursor
- DECTCEM sequences (show/hide) don't crash
- DECSCUSR sequences don't crash
- Cursor movement sequences don't crash
- Rendering to surface works correctly with null cursor

### 4. Writer with TerminalCursor / Interactive Mode (6 tests)
- Setting Writer.Cursor property
- Cursor position syncs after ANSI cursor movement (CUP, etc.)
- Cursor position syncs after text rendering
- DECTCEM show sets IsVisible = true
- DECTCEM hide sets IsVisible = false
- DECTCEM toggle works

### 5. DECSCUSR Cursor Shape Changes (11 tests)
- CSI 0 SP q → BlinkingBlock (default)
- CSI 1 SP q → BlinkingBlock
- CSI 2 SP q → SteadyBlock
- CSI 3 SP q → BlinkingUnderline
- CSI 4 SP q → SteadyUnderline
- CSI 5 SP q → BlinkingBar
- CSI 6 SP q → SteadyBar
- Sequence of shape changes works
- DECSCUSR with null cursor doesn't crash

### 6. Cursor Injectability (5 tests)
- Set cursor after Writer construction
- Set cursor to null mid-stream
- Replace cursor mid-stream — new cursor picks up
- All cursor properties are settable

### 7. Integration Tests (4 tests)
- Cursor + DECTCEM + DECSCUSR together
- Cursor syncs after complex movements (CUU, CUB, CUP)
- Cursor syncs with scrolling
- Cursor syncs with tab stops

### 8. Edge Cases (4 tests)
- Cursor at boundary (pending wrap)
- DECSCUSR invalid value handling
- DECSCUSR no params (defaults to BlinkingBlock)

## Implementation Requirements for Tests to Pass

### New Types Required
1. **SadConsole.Terminal.TerminalCursor** class with:
   - `Point Position { get; set; }`
   - `bool IsVisible { get; set; } = true`
   - `CursorShape Shape { get; set; } = CursorShape.BlinkingBlock`

2. **SadConsole.Terminal.CursorShape** enum with:
   - BlinkingBlock = 1
   - SteadyBlock = 2
   - BlinkingUnderline = 3
   - SteadyUnderline = 4
   - BlinkingBar = 5
   - SteadyBar = 6

### Writer Changes Required
1. **Change cursor property type:**
   - OLD: `public Components.Cursor Cursor { get; }`
   - NEW: `public TerminalCursor? Cursor { get; set; }`

2. **Remove cursor creation from constructor:**
   - Don't create a cursor in `Writer(ICellSurface, IFont)`
   - Cursor is null by default

3. **Add null-checks in cursor-related code:**
   - `SyncCursorPosition()` — check if Cursor is null before setting Position
   - DECTCEM handler — check if Cursor is null before setting IsVisible
   - DECSCUSR handler — check if Cursor is null before setting Shape

4. **Implement DECSCUSR handler (CSI Ps SP q):**
   - Parse parameter (default 0)
   - Map to CursorShape enum:
     - 0 or 1 → BlinkingBlock
     - 2 → SteadyBlock
     - 3 → BlinkingUnderline
     - 4 → SteadyUnderline
     - 5 → BlinkingBar
     - 6 → SteadyBar
   - Set `Cursor.Shape` if Cursor is not null

## Existing Tests That Need Updates

### TerminalWriterPhase2Tests.cs
The following tests reference `_writer.Cursor.IsVisible` and will need updates:

1. **Line 705** — `CursorVisibility_Show()`
   - Currently: `Assert.IsTrue(_writer.Cursor.IsVisible);`
   - Needs: Set up a TerminalCursor first, or change test strategy

2. **Line 713** — `CursorVisibility_Hide()`
   - Currently: `Assert.IsFalse(_writer.Cursor.IsVisible);`
   - Needs: Set up a TerminalCursor first, or change test strategy

3. **CursorVisibility_StateProperty()** — Lines 717-724
   - This test checks `_writer.State.CursorVisible`
   - This should still work if State.CursorVisible remains separate from Cursor
   - May need verification

### Recommended Fix Strategy
For the two failing tests in TerminalWriterPhase2Tests.cs:

```csharp
[TestMethod]
public void CursorVisibility_Show()
{
    var cursor = new TerminalCursor();
    _writer.Cursor = cursor;
    
    _writer.Feed("\x1b[?25l");  // hide cursor
    _writer.Feed("\x1b[?25h");  // show cursor

    Assert.IsTrue(_writer.Cursor.IsVisible);
}

[TestMethod]
public void CursorVisibility_Hide()
{
    var cursor = new TerminalCursor();
    _writer.Cursor = cursor;
    
    _writer.Feed("\x1b[?25l");  // hide cursor

    Assert.IsFalse(_writer.Cursor.IsVisible);
}
```

## Test Execution Plan

1. **These tests won't compile** until Roy implements the new types
2. **Expect ~2 existing tests to fail** in TerminalWriterPhase2Tests.cs
3. Roy should update those 2 tests when he changes the Writer.Cursor type
4. All 66 new tests should pass once implementation is complete

## Coverage Summary

- ✅ TerminalCursor data class contract
- ✅ CursorShape enum values
- ✅ Null cursor safety (data-stream mode)
- ✅ Cursor property injection
- ✅ DECTCEM (cursor visibility) with TerminalCursor
- ✅ DECSCUSR (cursor shape) implementation contract
- ✅ Cursor position syncing via SyncCursorPosition()
- ✅ Mid-stream cursor changes
- ✅ Integration with ANSI rendering
- ✅ Edge cases and robustness
