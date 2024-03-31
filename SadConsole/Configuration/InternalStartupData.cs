#nullable enable

using System;

namespace SadConsole.Configuration;

public class InternalStartupData : IConfigurator
{
    public int ScreenCellsX { get; set; } = 80;
    public int ScreenCellsY { get; set; } = 25;

    public bool? FocusStartingScreen { get; set; } = null;

    public void Run(Builder config, GameHost game)
    {
    }
}
