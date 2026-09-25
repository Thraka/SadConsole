using System;
using SkiaSharp;

namespace SadConsole.Host;

internal static class Global
{
    internal static GpuDevice? Device { get; set; }

    internal static GpuDevice RequireDevice() =>
        Device ?? throw new InvalidOperationException("The Skia GPU device has not been initialized.");

    internal static SKCanvas? Canvas { get; set; }

    internal static SKSamplingOptions Sampling { get; } = new(SKFilterMode.Nearest, SKMipmapMode.None);

    public static SKPaint SKPaint_SrcOver { get; } = new() { BlendMode = SKBlendMode.SrcOver };
    public static SKPaint SKPaint_Src { get; } = new() { BlendMode = SKBlendMode.Src };
}
