using System;
using BenchmarkDotNet.Attributes;
using SadConsole.Components;
using SadConsole.Input;

namespace SadConsole.PerformanceTests
{
    public class ScreenSurface
    {
        [Params(50, 500, 1000)]
        public int Size;

        private BasicGameHost _gameHost;
        private SadConsole.ScreenSurface _mainSurface;
        private TimeSpan _delta;
        private const string FILE = "test.consolez";

        [GlobalSetup]
        public void GlobalSetup()
        {
            _gameHost = new BasicGameHost();

            _mainSurface = new SadConsole.ScreenSurface(Size, Size);
            _mainSurface.Surface.FillWithRandomGarbage(255);
        }

        [Benchmark]
        public SadConsole.ScreenSurface Serialize()
        {
            SadConsole.Settings.SerializationIsCompressed = true;
            SadConsole.Serializer.Save(_mainSurface, FILE, false);
            return _mainSurface;
        }

        [Benchmark]
        public SadConsole.ScreenSurface Deserialize()
        {
            SadConsole.ScreenSurface result = Serializer.Load<SadConsole.ScreenSurface>(FILE, false);
            return result;
        }
    }
}
