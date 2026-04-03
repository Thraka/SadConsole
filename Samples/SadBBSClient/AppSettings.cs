using System;
using System.Collections.Generic;
using System.Text;

namespace SadBBSClient;

internal class AppSettings
{
    public int Width { get; set; } = 80;
    public int Height { get; set; } = 25;

    public static AppSettings Instance = new();
}
