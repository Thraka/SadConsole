using System;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace SadConsole.PerformanceTests;

public class ScreenSurfaceEffects
{
    private BasicGameHost _gameHost;

    [Params(10, 100, 200)]
    public int Size;

    [Params(true, false)]
    public bool Clone;

    private ICellSurface _surface = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _gameHost = new BasicGameHost();

        _surface = new CellSurface(Size, Size);
        _surface.FillWithRandomGarbage(255);

        var fadeEffect = new SadConsole.Effects.Fade
        {
            AutoReverse = true,
            DestinationForeground = new Gradient(Color.Blue, Color.Yellow),
            FadeForeground = true,
            UseCellForeground = false,
            //Repeat = true,
            FadeDuration = System.TimeSpan.FromSeconds(0.7d),
            RemoveOnFinished = true,
            CloneOnAdd = Clone,
            RestoreCellOnRemoved = true,
        };

        foreach (var cell in _surface)
            _surface.SetEffect(cell, fadeEffect);

        BenchmarkDotNet.Loggers.ConsoleLogger.Ascii.WriteLine(BenchmarkDotNet.Loggers.LogKind.Info, $"Count of effect instances {_surface.Effects.Count}");
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        BenchmarkDotNet.Loggers.ConsoleLogger.Ascii.WriteLine(BenchmarkDotNet.Loggers.LogKind.Info, $"Count of effect instances {_surface.Effects.Count}");
    }

    [Benchmark]
    public void UpdateHalfSecond()
    {
        _surface.Effects.UpdateEffects(TimeSpan.FromSeconds(0.5d));
    }

    [Benchmark]
    public void UpdateHalfSecond2Times()
    {
        
        _surface.Effects.UpdateEffects(TimeSpan.FromSeconds(0.5d));
        _surface.Effects.UpdateEffects(TimeSpan.FromSeconds(0.5d));
    }
}
