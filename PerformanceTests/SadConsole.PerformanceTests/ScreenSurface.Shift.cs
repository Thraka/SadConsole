using BenchmarkDotNet.Attributes;

namespace SadConsole.PerformanceTests;

public class ScreenSurfaceShift
{
    private BasicGameHost _gameHost;

    [Params(10, 100, 500)]
    public int Size;

    [Params(true, false)]
    public bool Wrap;

    private ICellSurface _surface = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _gameHost = new BasicGameHost();

        _surface = new CellSurface(Size, Size);
        _surface.FillWithRandomGarbage(255);
    }

    #region Vertical Shifts
    [Benchmark]
    public ICellSurface ShiftUpOne()
    {
        _surface.ShiftUp(1, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftUpHalf()
    {
        _surface.ShiftUp(_surface.Height / 2, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftUpHeightMinusOne()
    {
        _surface.ShiftUp(_surface.Height - 1, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftDownOne()
    {
        _surface.ShiftDown(1, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftDownHalf()
    {
        _surface.ShiftDown(_surface.Height / 2, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftDownHeightMinusOne()
    {
        _surface.ShiftDown(_surface.Height - 1, Wrap);
        return _surface;
    }
    #endregion

    #region Horizontal Shifts
    [Benchmark]
    public ICellSurface ShiftRightOne()
    {
        _surface.ShiftRight(1, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftRightHalf()
    {
        _surface.ShiftRight(_surface.Width / 2, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftRightWidthMinusOne()
    {
        _surface.ShiftRight(_surface.Width - 1, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftLeftOne()
    {
        _surface.ShiftLeft(1, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftLeftHalf()
    {
        _surface.ShiftLeft(_surface.Width / 2, Wrap);
        return _surface;
    }

    [Benchmark]
    public ICellSurface ShiftLeftWidthMinusOne()
    {
        _surface.ShiftLeft(_surface.Width - 1, Wrap);
        return _surface;
    }
    #endregion
}
