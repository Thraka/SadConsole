using System;
using System.IO;
using SadConsole.Instructions;

namespace SadConsole.Terminal;

/// <summary>
/// An instruction that reads bytes from a <see cref="Stream"/> and feeds them to a <see cref="Writer"/>
/// on each update tick. Supports throttling via <see cref="BytesPerSecond"/> to simulate slow connections.
/// </summary>
/// <remarks>
/// <para>
/// Add this as a component to a <see cref="IScreenObject"/> and it will automatically
/// feed data from the stream to the terminal writer every frame:
/// </para>
/// <code>
/// var reader = new Reader(writer, fileStream) { BytesPerSecond = 2400 };
/// screenObject.SadComponents.Add(reader);
/// </code>
/// <para>
/// Alternatively, call <see cref="InstructionBase.Update"/> manually each frame without
/// attaching to a screen object.
/// </para>
/// </remarks>
[System.Diagnostics.DebuggerDisplay("Instruction: TerminalReader (BPS={BytesPerSecond}, Finished={IsFinished})")]
public class Reader : InstructionBase
{
    private Stream? _stream;
    private byte[] _readBuffer = new byte[4096];
    private double _byteDebt;
    private bool _started;

    /// <summary>
    /// The <see cref="Writer"/> that receives parsed terminal data.
    /// </summary>
    public Writer Writer { get; }

    /// <summary>
    /// The source stream to read from. May be replaced between updates (or after <see cref="Reset"/>).
    /// When set to <see langword="null"/>, the reader idles until a new stream is assigned.
    /// </summary>
    public Stream? Stream
    {
        get => _stream;
        set
        {
            _stream = value;
            _byteDebt = 0;
        }
    }

    /// <summary>
    /// The maximum number of bytes to feed per second. Set to <c>0</c> for unlimited
    /// (all remaining bytes are sent every frame). Use the <see cref="BaudRate"/> constants
    /// for classic modem speeds.
    /// </summary>
    public int BytesPerSecond { get; set; }

    /// <summary>
    /// Predefined bytes-per-second values for classic modem and connection speeds.
    /// Baud rates are divided by 10 (start bit + 8 data bits + stop bit) to get the
    /// effective byte throughput.
    /// </summary>
    public static class BaudRate
    {
        /// <summary>110 baud — Teletype ASR-33 era.</summary>
        public const int Baud110 = 11;

        /// <summary>300 baud — early acoustic coupler modems.</summary>
        public const int Baud300 = 30;

        /// <summary>1200 baud — common early-80s BBS speed.</summary>
        public const int Baud1200 = 120;

        /// <summary>2400 baud — mid-80s BBS standard.</summary>
        public const int Baud2400 = 240;

        /// <summary>4800 baud.</summary>
        public const int Baud4800 = 480;

        /// <summary>9600 baud — late-80s high-speed modem.</summary>
        public const int Baud9600 = 960;

        /// <summary>14400 baud (14.4k) — early-90s V.32bis modem.</summary>
        public const int Baud14400 = 1440;

        /// <summary>19200 baud (19.2k).</summary>
        public const int Baud19200 = 1920;

        /// <summary>28800 baud (28.8k) — V.34 modem.</summary>
        public const int Baud28800 = 2880;

        /// <summary>33600 baud (33.6k) — V.34+ modem.</summary>
        public const int Baud33600 = 3360;

        /// <summary>56000 baud (56k) — V.90/V.92 modem.</summary>
        public const int Baud56000 = 5600;

        /// <summary>Unlimited — no throttling, all available bytes are sent every frame.</summary>
        public const int Unlimited = 0;
    }

    /// <summary>
    /// The total number of bytes that have been fed to the writer since the last <see cref="Reset"/>.
    /// </summary>
    public long TotalBytesRead { get; private set; }

    /// <summary>
    /// Creates a new <see cref="Reader"/> that feeds data from a stream to a terminal writer.
    /// </summary>
    /// <param name="writer">The terminal writer to feed data into.</param>
    /// <param name="stream">The source stream, or <see langword="null"/> to assign later.</param>
    public Reader(Writer writer, Stream? stream = null)
    {
        Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _stream = stream;
    }

    /// <summary>
    /// Creates a new <see cref="Reader"/> from a file path.
    /// </summary>
    /// <param name="writer">The terminal writer to feed data into.</param>
    /// <param name="filePath">Path to the file to read.</param>
    /// <returns>A new <see cref="Reader"/> with the file opened for reading.</returns>
    public static Reader FromFile(Writer writer, string filePath) =>
        new(writer, new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));

    /// <summary>
    /// Creates a new <see cref="Reader"/> from a byte array.
    /// </summary>
    /// <param name="writer">The terminal writer to feed data into.</param>
    /// <param name="data">The byte data to read.</param>
    /// <returns>A new <see cref="Reader"/> backed by a <see cref="MemoryStream"/>.</returns>
    public static Reader FromBytes(Writer writer, byte[] data) =>
        new(writer, new MemoryStream(data, writable: false));

    /// <inheritdoc />
    public override void Update(IScreenObject componentHost, TimeSpan delta)
    {
        if (!IsFinished)
        {
            if (!_started)
            {
                _started = true;
                OnStarted();
            }

            if (_stream is not null && _stream.CanRead)
            {
                int bytesToRead = CalculateByteBudget(delta);

                if (bytesToRead > 0)
                {
                    if (bytesToRead > _readBuffer.Length)
                        _readBuffer = new byte[bytesToRead];

                    int bytesRead = _stream.Read(_readBuffer, 0, bytesToRead);

                    if (bytesRead > 0)
                    {
                        Writer.Feed(_readBuffer.AsSpan(0, bytesRead));
                        TotalBytesRead += bytesRead;
                    }

                    if (bytesRead == 0 || !_stream.CanRead)
                        IsFinished = true;
                }
            }
            else if (_stream is not null)
            {
                // Stream exists but can't be read — we're done.
                IsFinished = true;
            }
        }

        base.Update(componentHost, delta);
    }

    /// <inheritdoc />
    public override void Reset()
    {
        _started = false;
        _byteDebt = 0;
        TotalBytesRead = 0;

        base.Reset();
    }

    /// <summary>
    /// Calculates how many bytes to read this frame based on <see cref="BytesPerSecond"/> and elapsed time.
    /// </summary>
    private int CalculateByteBudget(TimeSpan delta)
    {
        if (BytesPerSecond <= 0)
            return (int)Math.Min(int.MaxValue, _readBuffer.Length);

        _byteDebt += BytesPerSecond * delta.TotalSeconds;

        int budget = (int)_byteDebt;
        _byteDebt -= budget;

        return budget;
    }
}
