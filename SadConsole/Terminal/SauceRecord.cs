using System;
using System.Text;

namespace SadConsole.Terminal;

/// <summary>
/// Represents the SAUCE (Standard Architecture for Universal Comment Extensions) metadata
/// commonly appended to ANSI art files.
/// </summary>
/// <remarks>
/// <para>
/// A SAUCEd file has 4 parts: original content, an EOF character (0x1A),
/// an optional comment block ("COMNT" + N×64-byte lines), and the 128-byte SAUCE record.
/// </para>
/// <para>
/// Character fields in SAUCE are encoded in Code Page 437.  Fields shorter than their
/// maximum length are padded with spaces (or sometimes null-terminated with garbage after
/// the null — both cases are handled).
/// </para>
/// <para>
/// Use <see cref="Read"/> to parse, <see cref="GetContent"/> or <see cref="ContentLength"/>
/// to slice the content bytes for the parser so it never sees the SAUCE block.
/// </para>
/// </remarks>
public class SauceRecord
{
    /// <summary>The SAUCE record identifier. Always "SAUCE" for valid records.</summary>
    public string Id { get; init; } = "";

    /// <summary>The SAUCE version string (typically "00").</summary>
    public string Version { get; init; } = "";

    /// <summary>The title of the file (up to 35 characters).</summary>
    public string Title { get; init; } = "";

    /// <summary>The author of the file (up to 20 characters).</summary>
    public string Author { get; init; } = "";

    /// <summary>The group/organization (up to 20 characters).</summary>
    public string Group { get; init; } = "";

    /// <summary>The date in CCYYMMDD format (up to 8 characters).</summary>
    public string Date { get; init; } = "";

    /// <summary>The original file size (content bytes, excluding SAUCE and comment block).</summary>
    public uint FileSize { get; init; }

    /// <summary>The SAUCE data type. 1 = Character (for ANSI art).</summary>
    public byte DataType { get; init; }

    /// <summary>The SAUCE file type within the data type category. For Character: 0=ASCII, 1=ANSi, 2=ANSiMation.</summary>
    public byte FileType { get; init; }

    /// <summary>Type-dependent info 1. For Character types: width in columns (0 = use default 80).</summary>
    public ushort TInfo1 { get; init; }

    /// <summary>Type-dependent info 2. For Character types: number of lines (0 = not specified).</summary>
    public ushort TInfo2 { get; init; }

    /// <summary>Type-dependent info 3.</summary>
    public ushort TInfo3 { get; init; }

    /// <summary>Type-dependent info 4.</summary>
    public ushort TInfo4 { get; init; }

    /// <summary>Number of comment lines (0–255). Each line is 64 characters.</summary>
    public byte CommentCount { get; init; }

    /// <summary>
    /// Type-dependent flags. For Character data, this is the ANSi flags byte:
    /// bit 0 = iCE colors (non-blink), bits 1–2 = letter spacing, bits 3–4 = aspect ratio.
    /// </summary>
    public byte Flags { get; init; }

    /// <summary>Type-dependent info string (22 characters). For Character types: font name hint.</summary>
    public string TInfoS { get; init; } = "";

    /// <summary>Comment lines (each up to 64 characters), or empty if no comments.</summary>
    public string[] Comments { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Number of content bytes to parse (excludes SAUCE record, comment block, and EOF marker).
    /// Feed <c>data.Slice(0, ContentLength)</c> to the parser to avoid interpreting metadata as sequences.
    /// </summary>
    public int ContentLength { get; init; }

    // ═══════════════════════════════════════════════════════════
    //  Convenience properties for Character (ANSI) data
    // ═══════════════════════════════════════════════════════════

    /// <summary>Whether this is a Character-type SAUCE record (DataType == 1).</summary>
    public bool IsCharacter => DataType == 1;

    /// <summary>Whether the file type is ANSi (DataType == 1, FileType == 1).</summary>
    public bool IsAnsi => DataType == 1 && FileType == 1;

    /// <summary>Width in columns (TInfo1). Returns 0 if not specified.</summary>
    public int Width => TInfo1;

    /// <summary>Height in lines (TInfo2). Returns 0 if not specified.</summary>
    public int Height => TInfo2;

    /// <summary>iCE color mode (non-blink). When true, the blink SGR attribute means bright background instead of blinking.</summary>
    public bool IceColors => IsCharacter && (Flags & 0x01) != 0;

    /// <summary>Letter spacing hint from ANSiFlags bits 1–2.</summary>
    public SauceLetterSpacing LetterSpacing =>
        IsCharacter ? (SauceLetterSpacing)((Flags >> 1) & 0x03) : SauceLetterSpacing.Legacy;

    /// <summary>Aspect ratio hint from ANSiFlags bits 3–4.</summary>
    public SauceAspectRatio AspectRatio =>
        IsCharacter ? (SauceAspectRatio)((Flags >> 3) & 0x03) : SauceAspectRatio.Legacy;

    /// <summary>Font name from the TInfoS field (meaningful for Character types).</summary>
    public string FontName => TInfoS;

    // ═══════════════════════════════════════════════════════════
    //  Static factory
    // ═══════════════════════════════════════════════════════════

    private const int SauceRecordSize = 128;
    private const int CommentLineSize = 64;
    private const int CommentHeaderSize = 5; // "COMNT"

    /// <summary>
    /// Attempts to read a SAUCE record from the given file data.
    /// Returns <see langword="null"/> if no valid SAUCE record is found.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The file layout is: <c>[content] [EOF 0x1A] [COMNT block?] [SAUCE record]</c>.
    /// </para>
    /// <para>
    /// Detection is record-driven: the SAUCE record is always the last 128 bytes
    /// of the file.  We check for the "SAUCE" magic there first, then work backwards
    /// to locate the optional comment block and EOF marker.  This avoids false
    /// positives from 0x1A bytes that may appear inside SAUCE numeric fields.
    /// </para>
    /// </remarks>
    public static SauceRecord? Read(ReadOnlySpan<byte> data)
    {
        // ── Step 1: The SAUCE record is always the last 128 bytes ──
        if (data.Length < SauceRecordSize)
            return null;

        ReadOnlySpan<byte> record = data.Slice(data.Length - SauceRecordSize);

        // Validate "SAUCE" magic
        if (record[0] != (byte)'S' || record[1] != (byte)'A' ||
            record[2] != (byte)'U' || record[3] != (byte)'C' || record[4] != (byte)'E')
            return null;

        // ── Step 2: Parse the SAUCE record fields ──
        string id = ReadCharField(record.Slice(0, 5));
        string version = ReadCharField(record.Slice(5, 2));

        // Per spec note 6: if version isn't "00", don't interpret fields
        if (version != "00")
            return null;

        string title = ReadCharField(record.Slice(7, 35));
        string author = ReadCharField(record.Slice(42, 20));
        string group = ReadCharField(record.Slice(62, 20));
        string date = ReadCharField(record.Slice(82, 8));
        uint fileSize = BitConverter.ToUInt32(record.Slice(90, 4));
        byte dataType = record[94];
        byte fileType = record[95];
        ushort tInfo1 = BitConverter.ToUInt16(record.Slice(96, 2));
        ushort tInfo2 = BitConverter.ToUInt16(record.Slice(98, 2));
        ushort tInfo3 = BitConverter.ToUInt16(record.Slice(100, 2));
        ushort tInfo4 = BitConverter.ToUInt16(record.Slice(102, 2));
        byte commentCount = record[104];
        byte flags = record[105];
        string tInfoS = ReadZString(record.Slice(106, 22));

        // ── Step 3: Read optional comment block before the SAUCE record ──
        int metaStart = data.Length - SauceRecordSize;
        string[] comments = Array.Empty<string>();

        if (commentCount > 0)
        {
            int commentBlockSize = CommentHeaderSize + commentCount * CommentLineSize;
            int commentStart = metaStart - commentBlockSize;

            if (commentStart >= 0)
            {
                ReadOnlySpan<byte> commentBlock = data.Slice(commentStart, commentBlockSize);

                if (commentBlock[0] == (byte)'C' && commentBlock[1] == (byte)'O' &&
                    commentBlock[2] == (byte)'M' && commentBlock[3] == (byte)'N' &&
                    commentBlock[4] == (byte)'T')
                {
                    comments = new string[commentCount];
                    for (int i = 0; i < commentCount; i++)
                    {
                        comments[i] = ReadCharField(
                            commentBlock.Slice(CommentHeaderSize + i * CommentLineSize, CommentLineSize));
                    }

                    metaStart = commentStart;
                }
            }
        }

        // ── Step 4: Content length — strip the EOF marker if present ──
        int contentLength = metaStart;
        if (metaStart > 0 && data[metaStart - 1] == 0x1A)
            contentLength = metaStart - 1;

        return new SauceRecord
        {
            Id = id,
            Version = version,
            Title = title,
            Author = author,
            Group = group,
            Date = date,
            FileSize = fileSize,
            DataType = dataType,
            FileType = fileType,
            TInfo1 = tInfo1,
            TInfo2 = tInfo2,
            TInfo3 = tInfo3,
            TInfo4 = tInfo4,
            CommentCount = commentCount,
            Flags = flags,
            TInfoS = tInfoS,
            Comments = comments,
            ContentLength = contentLength,
        };
    }

    /// <summary>
    /// Reads a SAUCE Character field (CP437, space-padded).
    /// Handles the common case of null-termination followed by garbage.
    /// </summary>
    private static string ReadCharField(ReadOnlySpan<byte> field)
    {
        // Find first null byte — anything after is garbage
        int len = field.IndexOf((byte)0);
        if (len < 0) len = field.Length;

        // Decode as Latin-1 (preserves byte values 0-255 as-is, matching CP437 code points)
        // ASCII would clamp >127 to '?'. Latin-1 keeps the original values intact.
        ReadOnlySpan<byte> meaningful = field.Slice(0, len);
        string result = Encoding.Latin1.GetString(meaningful);

        return result.TrimEnd(' ');
    }

    /// <summary>
    /// Reads a SAUCE ZString field (null-terminated C string, CP437).
    /// Unused portion is filled with binary zero.
    /// </summary>
    private static string ReadZString(ReadOnlySpan<byte> field)
    {
        int len = field.IndexOf((byte)0);
        if (len < 0) len = field.Length;
        if (len == 0) return "";

        return Encoding.Latin1.GetString(field.Slice(0, len));
    }

    /// <summary>
    /// Returns just the content portion of the data (excluding SAUCE, comments, and EOF marker).
    /// Convenience wrapper around <see cref="ContentLength"/>.
    /// </summary>
    public ReadOnlySpan<byte> GetContent(ReadOnlySpan<byte> data)
    {
        if (ContentLength <= 0 || ContentLength > data.Length)
            return data;

        return data.Slice(0, ContentLength);
    }
}

/// <summary>
/// Letter spacing hint from SAUCE ANSiFlags bits 1–2.
/// </summary>
public enum SauceLetterSpacing
{
    /// <summary>No preference / legacy interpretation.</summary>
    Legacy = 0,
    /// <summary>Select 8-pixel font.</summary>
    EightPixel = 1,
    /// <summary>Select 9-pixel font.</summary>
    NinePixel = 2,
}

/// <summary>
/// Aspect ratio hint from SAUCE ANSiFlags bits 3–4.
/// </summary>
public enum SauceAspectRatio
{
    /// <summary>No preference / legacy interpretation.</summary>
    Legacy = 0,
    /// <summary>Stretch to legacy aspect ratio.</summary>
    Stretch = 1,
    /// <summary>Square pixel aspect ratio (modern).</summary>
    Square = 2,
}
