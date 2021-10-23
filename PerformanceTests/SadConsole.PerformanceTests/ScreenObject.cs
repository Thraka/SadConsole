using System;
using BenchmarkDotNet.Attributes;
using SadConsole.Components;
using SadConsole.Input;

namespace SadConsole.PerformanceTests
{
    /// <summary>
    /// Test component that does nothing except ensures it is processed in the Update/Render loops, for performance testing
    /// </summary>
    public class TestComponent : IComponent
    {
        public uint SortOrder { get; set; }

        public void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled) =>
            handled = false;

        public void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
            handled = false;

        public void Update(IScreenObject host, TimeSpan delta)
        { }

        public void Render(IScreenObject host, TimeSpan delta)
        { }

        public virtual void OnAdded(IScreenObject host)
        { }

        public virtual void OnRemoved(IScreenObject host)
        { }

        uint IComponent.SortOrder => SortOrder;

        bool IComponent.IsUpdate => true;

        bool IComponent.IsRender => true;

        bool IComponent.IsMouse => true;

        bool IComponent.IsKeyboard => true;
    }

    /// <summary>
    /// Series of benchmarks for various functionality of ScreenObject.  Benchmark functions that call methods which return void will instead return
    /// the screen object being benchmarked, in an effort to ensure that the compiler will not optimize anything out.
    /// </summary>
    public class ScreenObject
    {
        #region Test Data

        [Params(0, 1, 100, 1000)]
        public int NumComponents;

        [Params(0, 1, 100, 1000)]
        public int NumChildren;

        #endregion

        private BasicGameHost _gameHost;
        private SadConsole.ScreenObject _mainObject;
        private TimeSpan _delta;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _gameHost = new BasicGameHost();
            _mainObject = new SadConsole.ScreenObject();
            for (int i = 0; i < NumComponents; i++)
                _mainObject.SadComponents.Add(new TestComponent());

            for (int i = 0; i < NumChildren; i++)
                _mainObject.Children.Add(new SadConsole.ScreenObject());

            _delta = TimeSpan.FromMilliseconds(5);
        }

        #region Main Game Loop
        [Benchmark]
        public bool ProcessKeyboard()
        {
            return _mainObject.ProcessKeyboard(GameHost.Instance.Keyboard);
        }

        [Benchmark]
        public bool ProcessMouse()
        {
            return _mainObject.ProcessMouse(new MouseScreenObjectState(_mainObject, GameHost.Instance.Mouse));
        }

        [Benchmark]
        public SadConsole.ScreenObject Update()
        {
            _mainObject.Update(_delta);

            return _mainObject;
        }

        [Benchmark]
        public SadConsole.ScreenObject Render()
        {
            _mainObject.Render(_delta);

            return _mainObject;
        }
        #endregion
    }
}
