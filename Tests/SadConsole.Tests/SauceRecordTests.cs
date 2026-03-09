using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadConsole.Terminal;

namespace SadConsole.Tests;

/// <summary>
/// Tests for Terminal.SauceRecord — SAUCE metadata reader.
/// </summary>
[TestClass]
public class SauceRecordTests
{
    // ──────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────

    /// <summary>
    /// Builds a minimal SAUCE record (128 bytes) with the given fields.
    /// </summary>
    private static byte[] BuildSauceRecord(
        string title = "",
        string author = "",
        string group = "",
        string date = "20260101",
        uint fileSize = 0,
        byte dataType = 1,
        byte fileType = 1,
        ushort tInfo1 = 80,
        ushort tInfo2 = 25,
        byte commentCount = 0,
        byte flags = 0,
        string tInfoS = "")
    {
        byte[] record = new byte[128];

        // ID
        Encoding.ASCII.GetBytes("SAUCE", record.AsSpan(0, 5));
        // Version
        Encoding.ASCII.GetBytes("00", record.AsSpan(5, 2));
        // Title (35 bytes)
        Encoding.ASCII.GetBytes(title.PadRight(35).AsSpan(0, 35), record.AsSpan(7, 35));
        // Author (20 bytes)
        Encoding.ASCII.GetBytes(author.PadRight(20).AsSpan(0, 20), record.AsSpan(42, 20));
        // Group (20 bytes)
        Encoding.ASCII.GetBytes(group.PadRight(20).AsSpan(0, 20), record.AsSpan(62, 20));
        // Date (8 bytes)
        Encoding.ASCII.GetBytes(date.PadRight(8).AsSpan(0, 8), record.AsSpan(82, 8));
        // FileSize (4 bytes LE)
        BitConverter.TryWriteBytes(record.AsSpan(90, 4), fileSize);
        // DataType
        record[94] = dataType;
        // FileType
        record[95] = fileType;
        // TInfo1 (2 bytes LE)
        BitConverter.TryWriteBytes(record.AsSpan(96, 2), tInfo1);
        // TInfo2 (2 bytes LE)
        BitConverter.TryWriteBytes(record.AsSpan(98, 2), tInfo2);
        // Comments
        record[104] = commentCount;
        // Flags
        record[105] = flags;
        // TInfoS (22 bytes) — ZString: null-terminated, remainder is binary zero
        if (tInfoS.Length > 0)
        {
            int len = Math.Min(tInfoS.Length, 22);
            Encoding.ASCII.GetBytes(tInfoS.AsSpan(0, len), record.AsSpan(106, len));
            // rest stays 0x00 from initialization
        }

        return record;
    }

    /// <summary>
    /// Builds a SAUCE comment block: "COMNT" + N x 64-byte lines.
    /// </summary>
    private static byte[] BuildCommentBlock(params string[] lines)
    {
        byte[] block = new byte[5 + lines.Length * 64];
        Encoding.ASCII.GetBytes("COMNT", block.AsSpan(0, 5));
        for (int i = 0; i < lines.Length; i++)
        {
            string padded = lines[i].PadRight(64);
            Encoding.ASCII.GetBytes(padded.AsSpan(0, 64), block.AsSpan(5 + i * 64, 64));
        }
        return block;
    }

    /// <summary>
    /// Combines content + EOF(0x1A) + optional comments + SAUCE record into a single byte array.
    /// The EOF marker is always included — it's required for SAUCE detection.
    /// </summary>
    private static byte[] BuildFile(byte[] content, byte[] sauceRecord, byte[]? commentBlock = null)
    {
        int totalSize = content.Length + 1 + (commentBlock?.Length ?? 0) + sauceRecord.Length;
        byte[] file = new byte[totalSize];
        int offset = 0;

        Array.Copy(content, 0, file, offset, content.Length);
        offset += content.Length;

        // EOF marker — always present
        file[offset] = 0x1A;
        offset++;

        if (commentBlock != null)
        {
            Array.Copy(commentBlock, 0, file, offset, commentBlock.Length);
            offset += commentBlock.Length;
        }

        Array.Copy(sauceRecord, 0, file, offset, sauceRecord.Length);
        return file;
    }

    // ──────────────────────────────────────────────
    // 1. Basic parsing
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_ValidSauce_ReturnsRecord()
    {
        byte[] sauce = BuildSauceRecord(title: "Test Art", author: "Thraka", group: "ACiD");
        byte[] content = Encoding.ASCII.GetBytes("Hello ANSI");
        byte[] file = BuildFile(content, sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual("SAUCE", record.Id);
        Assert.AreEqual("00", record.Version);
        Assert.AreEqual("Test Art", record.Title);
        Assert.AreEqual("Thraka", record.Author);
        Assert.AreEqual("ACiD", record.Group);
    }

    [TestMethod]
    public void Read_NoSauce_ReturnsNull()
    {
        byte[] data = Encoding.ASCII.GetBytes("Just some content without SAUCE");
        SauceRecord? record = SauceRecord.Read(data);
        Assert.IsNull(record);
    }

    [TestMethod]
    public void Read_TooShort_ReturnsNull()
    {
        byte[] data = new byte[50]; // less than 128 bytes
        SauceRecord? record = SauceRecord.Read(data);
        Assert.IsNull(record);
    }

    [TestMethod]
    public void Read_InvalidMagic_ReturnsNull()
    {
        byte[] data = new byte[128];
        Encoding.ASCII.GetBytes("XXXXX", data.AsSpan(0, 5));
        SauceRecord? record = SauceRecord.Read(data);
        Assert.IsNull(record);
    }

    // ──────────────────────────────────────────────
    // 2. Character/ANSI metadata
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_AnsiType_WidthAndHeight()
    {
        byte[] sauce = BuildSauceRecord(dataType: 1, fileType: 1, tInfo1: 132, tInfo2: 50);
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.IsTrue(record.IsCharacter);
        Assert.IsTrue(record.IsAnsi);
        Assert.AreEqual(132, record.Width);
        Assert.AreEqual(50, record.Height);
    }

    [TestMethod]
    public void Read_NonCharacterType_IsCharacterFalse()
    {
        byte[] sauce = BuildSauceRecord(dataType: 5, fileType: 0); // Bitmap
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.IsFalse(record.IsCharacter);
        Assert.IsFalse(record.IsAnsi);
    }

    [TestMethod]
    public void Read_Date_ParsedCorrectly()
    {
        byte[] sauce = BuildSauceRecord(date: "20260305");
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual("20260305", record.Date);
    }

    [TestMethod]
    public void Read_FontName_FromTInfoS()
    {
        byte[] sauce = BuildSauceRecord(tInfoS: "IBM VGA");
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual("IBM VGA", record.FontName);
    }

    // ──────────────────────────────────────────────
    // 3. ANSiFlags
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_IceColors_FlagSet()
    {
        byte[] sauce = BuildSauceRecord(flags: 0x01); // bit 0 = iCE
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.IsTrue(record.IceColors);
    }

    [TestMethod]
    public void Read_IceColors_FlagClear()
    {
        byte[] sauce = BuildSauceRecord(flags: 0x00);
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.IsFalse(record.IceColors);
    }

    [TestMethod]
    public void Read_LetterSpacing_8Pixel()
    {
        byte[] sauce = BuildSauceRecord(flags: 0x02); // bits 1-2 = 01 (8px)
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(SauceLetterSpacing.EightPixel, record.LetterSpacing);
    }

    [TestMethod]
    public void Read_LetterSpacing_9Pixel()
    {
        byte[] sauce = BuildSauceRecord(flags: 0x04); // bits 1-2 = 10 (9px)
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(SauceLetterSpacing.NinePixel, record.LetterSpacing);
    }

    [TestMethod]
    public void Read_AspectRatio_Square()
    {
        byte[] sauce = BuildSauceRecord(flags: 0x10); // bits 3-4 = 10 (square)
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(SauceAspectRatio.Square, record.AspectRatio);
    }

    [TestMethod]
    public void Read_CombinedFlags_IceAndSpacing()
    {
        byte[] sauce = BuildSauceRecord(flags: 0x05); // iCE (0x01) + 9px (0x04)
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.IsTrue(record.IceColors);
        Assert.AreEqual(SauceLetterSpacing.NinePixel, record.LetterSpacing);
    }

    [TestMethod]
    public void Read_NonCharacterType_FlagsIgnored()
    {
        byte[] sauce = BuildSauceRecord(dataType: 5, flags: 0x01); // Not character type
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.IsFalse(record.IceColors); // Not character type, so flags ignored
        Assert.AreEqual(SauceLetterSpacing.Legacy, record.LetterSpacing);
    }

    // ──────────────────────────────────────────────
    // 4. Content length calculation
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_ContentLength_WithFileSize()
    {
        byte[] content = Encoding.ASCII.GetBytes("ABCDEFGHIJ"); // 10 bytes
        byte[] sauce = BuildSauceRecord(fileSize: 10);
        byte[] file = BuildFile(content, sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(10, record.ContentLength);
    }

    [TestMethod]
    public void Read_ContentLength_NoFileSize_Calculated()
    {
        byte[] content = Encoding.ASCII.GetBytes("ABCDEFGHIJ"); // 10 bytes
        byte[] sauce = BuildSauceRecord(fileSize: 0);
        byte[] file = BuildFile(content, sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(10, record.ContentLength);
    }

    [TestMethod]
    public void GetContent_ReturnsCorrectSlice()
    {
        byte[] content = Encoding.ASCII.GetBytes("Hello ANSI!");
        byte[] sauce = BuildSauceRecord(fileSize: (uint)content.Length);
        byte[] file = BuildFile(content, sauce);

        SauceRecord? record = SauceRecord.Read(file);
        Assert.IsNotNull(record);

        ReadOnlySpan<byte> actual = record.GetContent(file);
        Assert.AreEqual(content.Length, actual.Length);

        for (int i = 0; i < content.Length; i++)
            Assert.AreEqual(content[i], actual[i]);
    }

    // ──────────────────────────────────────────────
    // 5. Comment block
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_WithComments_ParsesCommentLines()
    {
        byte[] content = Encoding.ASCII.GetBytes("Content");
        byte[] comments = BuildCommentBlock("Line one", "Line two");
        byte[] sauce = BuildSauceRecord(commentCount: 2);
        byte[] file = BuildFile(content, sauce, comments);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(2, record.CommentCount);
        Assert.AreEqual(2, record.Comments.Length);
        Assert.AreEqual("Line one", record.Comments[0]);
        Assert.AreEqual("Line two", record.Comments[1]);
    }

    [TestMethod]
    public void Read_NoComments_EmptyArray()
    {
        byte[] sauce = BuildSauceRecord(commentCount: 0);
        byte[] file = BuildFile(new byte[10], sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(0, record.Comments.Length);
    }

    [TestMethod]
    public void Read_CommentBlockMissingComntMagic_EmptyComments()
    {
        byte[] content = Encoding.ASCII.GetBytes("Content");
        // Build a comment block but with wrong magic
        byte[] fakeComments = new byte[5 + 64];
        Encoding.ASCII.GetBytes("XXXXX", fakeComments.AsSpan(0, 5));
        byte[] sauce = BuildSauceRecord(commentCount: 1);
        byte[] file = BuildFile(content, sauce, fakeComments);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(0, record.Comments.Length); // Invalid COMNT, no comments parsed
    }

    [TestMethod]
    public void Read_ContentLength_WithComments()
    {
        byte[] content = Encoding.ASCII.GetBytes("ABCDE"); // 5 bytes
        byte[] comments = BuildCommentBlock("A comment line");
        byte[] sauce = BuildSauceRecord(fileSize: 0, commentCount: 1);
        byte[] file = BuildFile(content, sauce, comments);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        // Content length = total - 128 (SAUCE) - 69 (5 + 1*64 comments)
        Assert.AreEqual(5, record.ContentLength);
    }

    // ──────────────────────────────────────────────
    // 6. Edge cases
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_SauceOnly_NoContent()
    {
        // File is just EOF + SAUCE record, no preceding content
        byte[] sauce = BuildSauceRecord();
        byte[] file = BuildFile(Array.Empty<byte>(), sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(0, record.ContentLength);
    }

    [TestMethod]
    public void Read_NoEofMarker_StillDetected()
    {
        // A bare SAUCE record with no EOF marker is still detected — the SAUCE
        // record is always the last 128 bytes; EOF is optional per the spec.
        byte[] sauce = BuildSauceRecord(title: "No EOF");
        SauceRecord? record = SauceRecord.Read(sauce);

        Assert.IsNotNull(record);
        Assert.AreEqual("No EOF", record.Title);
        Assert.AreEqual(0, record.ContentLength);
    }

    [TestMethod]
    public void Read_EofPresent_MinimalFile()
    {
        byte[] sauce = BuildSauceRecord(title: "Minimal");
        byte[] file = BuildFile(Array.Empty<byte>(), sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual("Minimal", record.Title);
    }

    // ──────────────────────────────────────────────
    // 7. Spec-driven: EOF as content boundary
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_EofDriven_ContentEndsAtEof()
    {
        // Real file layout: content + EOF(0x1A) + SAUCE
        byte[] content = Encoding.ASCII.GetBytes("Hello ANSI Art");
        byte[] sauce = BuildSauceRecord(fileSize: 0); // FileSize not set
        byte[] file = BuildFile(content, sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(content.Length, record.ContentLength);
    }

    [TestMethod]
    public void Read_EofDriven_WithComments_ContentEndsAtEof()
    {
        byte[] content = Encoding.ASCII.GetBytes("Art data");
        byte[] comments = BuildCommentBlock("A comment");
        byte[] sauce = BuildSauceRecord(fileSize: 0, commentCount: 1);
        byte[] file = BuildFile(content, sauce, comments);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(content.Length, record.ContentLength);
    }

    [TestMethod]
    public void Read_EofPrioritized_OverIncorrectFileSize()
    {
        // Many real SAUCE files have wrong FileSize (spec note 8)
        byte[] content = Encoding.ASCII.GetBytes("Real content");
        byte[] sauce = BuildSauceRecord(fileSize: 999); // Incorrect FileSize
        byte[] file = BuildFile(content, sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        // EOF is canonical — should use it, not the bad FileSize
        Assert.AreEqual(content.Length, record.ContentLength);
    }

    // ──────────────────────────────────────────────
    // 8. Spec-driven: version validation
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_UnsupportedVersion_ReturnsNull()
    {
        // Per spec note 6: don't interpret if version isn't "00"
        byte[] record = new byte[128];
        Encoding.ASCII.GetBytes("SAUCE", record.AsSpan(0, 5));
        Encoding.ASCII.GetBytes("01", record.AsSpan(5, 2)); // Version "01" — unknown
        record[94] = 1; // DataType
        record[95] = 1; // FileType

        // Wrap with EOF + content so it's detectable
        byte[] file = new byte[1 + 128]; // EOF + SAUCE
        file[0] = 0x1A;
        Array.Copy(record, 0, file, 1, 128);

        SauceRecord? result = SauceRecord.Read(file);
        Assert.IsNull(result);
    }

    // ──────────────────────────────────────────────
    // 9. Spec-driven: null-terminated Character fields with garbage
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_NullTerminatedFieldWithGarbage_TruncatesAtNull()
    {
        // Per spec note 3: some SAUCE has null-terminated fields with garbage after
        byte[] sauce = BuildSauceRecord(title: "Clean");
        // Manually inject null + garbage into the title field (offset 7, 35 bytes)
        sauce[7 + 5] = 0x00; // null terminator after "Clean"
        sauce[7 + 6] = 0xFF; // garbage
        sauce[7 + 7] = 0xAB; // garbage

        byte[] file = BuildFile(new byte[5], sauce);
        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual("Clean", record.Title);
    }

    // ──────────────────────────────────────────────
    // 10. Large FileSize (>65535 — tests uint32 vs uint16 fix)
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_LargeFileSize_ReadsFull32Bits()
    {
        // FileSize = 100,000 — only works if we read all 4 bytes (uint32)
        uint bigSize = 100_000;
        byte[] content = new byte[10];
        byte[] sauce = BuildSauceRecord(fileSize: bigSize);
        byte[] file = BuildFile(content, sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(bigSize, record.FileSize);
    }

    // ──────────────────────────────────────────────
    // 11. 0x1A bytes inside SAUCE fields must not confuse detection
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Read_0x1A_InsideSauceField_NotConfusedWithEof()
    {
        // TInfo1 = 26 (0x001A) — the 0x1A byte inside the SAUCE record must
        // not be mistaken for the EOF marker.
        byte[] content = Encoding.ASCII.GetBytes("Real content");
        byte[] sauce = BuildSauceRecord(tInfo1: 26);
        byte[] file = BuildFile(content, sauce);

        SauceRecord? record = SauceRecord.Read(file);

        Assert.IsNotNull(record);
        Assert.AreEqual(26, record.TInfo1);
        Assert.AreEqual(content.Length, record.ContentLength);
    }
}
