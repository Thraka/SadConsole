using System;
using SkiaSharp;

namespace SadConsole.Host;

internal sealed class GpuDevice
{
    private readonly nint _contextHandle;

    internal GRContext Context { get; }

    internal GpuDevice(GRContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Backend != GRBackend.OpenGL)
            throw new NotSupportedException($"The Skia host requires an OpenGL GPU context. The supplied context uses {context.Backend}.");

        if (context.IsAbandoned)
            throw new InvalidOperationException("The supplied OpenGL GPU context has been abandoned.");

        Context = context;
        _contextHandle = context.Handle;

        using SKSurface validationSurface = CreateSurface(1, 1);
        using SKImage validationImage = validationSurface.Snapshot();
        if (!validationImage.IsTextureBacked)
            throw new InvalidOperationException("The supplied OpenGL context did not create a GPU-backed Skia surface.");
    }

    internal SKSurface CreateSurface(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        ValidateContext(Context);

        if (width > Context.MaxRenderTargetSize || height > Context.MaxRenderTargetSize)
            throw new ArgumentOutOfRangeException(nameof(width), $"The requested {width}x{height} surface exceeds the GPU's {Context.MaxRenderTargetSize} pixel render-target limit.");

        var imageInfo = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        SKSurface surface = SKSurface.Create(Context, true, imageInfo)
            ?? throw new InvalidOperationException($"OpenGL could not allocate a {width}x{height} GPU render target.");

        if (surface.Context is not GRContext surfaceContext || surfaceContext.Handle != _contextHandle)
        {
            surface.Dispose();
            throw new InvalidOperationException("Skia created a surface on a different GPU context.");
        }

        return surface;
    }

    internal void ValidateCanvas(SKCanvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (canvas.Context is not GRContext context)
            throw new InvalidOperationException("The destination canvas is not backed by a direct GPU context.");

        ValidateContext(context);
    }

    internal void ValidateContext(GRContext context)
    {
        if (context.Handle != _contextHandle)
            throw new InvalidOperationException("The active Skia canvas uses a different GPU context than the one supplied at startup.");

        if (context.Backend != GRBackend.OpenGL)
            throw new InvalidOperationException("The active Skia GPU context is not OpenGL-backed.");

        if (context.IsAbandoned)
            throw new InvalidOperationException("The OpenGL GPU context has been abandoned. Recreate the SadConsole game and its GPU resources.");
    }
}
