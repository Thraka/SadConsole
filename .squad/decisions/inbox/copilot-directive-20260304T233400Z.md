### 2026-03-04T23:34:00Z: User directive
**By:** Thraka (via Copilot)
**What:** Future enhancement: Replace Parser's `ParserEncoding` enum with accepting a `System.Text.Encoding` instance. Default to null (passthrough: `(char)b` for raw byte-as-glyph, the current CP437 behavior). When an Encoding is provided, use its Decoder for byte→char conversion. This enables `Encoding.GetEncoding(437)`, `Encoding.UTF8`, or any custom encoding. Requires `System.Text.Encoding.CodePages` NuGet for CP437. Not blocking — current enum works fine for now.
**Why:** User request — cleaner API, deferred to avoid scope creep.
