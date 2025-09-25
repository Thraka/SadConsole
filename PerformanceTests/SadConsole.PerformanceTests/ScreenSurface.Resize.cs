using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using SadRogue.Primitives;

namespace SadConsole.PerformanceTests;

[SimpleJob(RunStrategy.Throughput)]
public class ScreenSurfaceResize
{
    private BasicGameHost _gameHost;

    [Params(500)]
    public int Size;

    [Params(true, false)]
    public bool Clear;

    private ResettableCellSurface _surface = null!;
    private CellSurface _originalSurface = null!;

    [GlobalCleanup]
    public void GlobalCleanup() =>
        _gameHost.Dispose();

    [GlobalSetup]
    public void GlobalSetup()
    {
        _gameHost = new BasicGameHost();

        _originalSurface = new CellSurface(Size, Size);
        _originalSurface.FillWithRandomGarbage(255);
        _surface = new ResettableCellSurface(Size, Size);
    }

    [Benchmark]
    public ICellSurface ResizeBigger_10()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width + 10, _surface.Height + 10, Clear);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ResizeBigger_HalfSize()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width + _surface.Width / 2, _surface.Height + _surface.Height / 2, Clear);
        return _surface;
    }
    [Benchmark]
    public ICellSurface ResizeBigger_DoubleSize()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width * 2, _surface.Height * 2, Clear);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ResizeBigger_SameWidth_10()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width, _surface.Height + 10, Clear);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ResizeBigger_SameWidth_HalfSize()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width, _surface.Height + _surface.Height / 2, Clear);
        return _surface;
    }
    [Benchmark]
    public ICellSurface ResizeBigger_SameWidth_DoubleSize()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width, _surface.Height * 2, Clear);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ResizeSmaller_10()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width - 10, _surface.Height - 10, Clear);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ResizeSmaller_HalfSize()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width - Size / 2, _surface.Height - _surface.Height / 2, Clear);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ResizeSmaller_SameWidth_10()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width, _surface.Height - 10, Clear);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ResizeSmaller_SameWidth_HalfSize()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width, _surface.Height - _surface.Height / 2, Clear);
        return _surface;
    }

    [Benchmark]
    public ICellSurface Resize_SameSize()
    {
        _surface.ResetCells(_originalSurface.Cells, Size, Size);
        _surface.Resize(10, 10, _surface.Width, _surface.Height, Clear);
        return _surface;
    }

    private class ResettableCellSurface : CellSurface
    {
        public ResettableCellSurface(int width, int height) : base(width, height)
        {
        }
        public void ResetCells(ColoredGlyphBase[] cells, int width, int height)
        {
            Cells = cells;
            _viewArea = new BoundedRectangle((0, 0, width, height),
                                 (0, 0, width, height));
        }
    }
}
