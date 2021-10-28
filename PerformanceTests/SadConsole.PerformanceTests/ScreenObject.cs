using System;
using BenchmarkDotNet.Attributes;
using SadConsole.Components;
using SadConsole.Input;

namespace SadConsole.PerformanceTests
{
    /// <summary>
    /// Test component that does nothing except ensures it is processed in the Update, Render, ProcessKeyboard, and ProcessMouse loops, for performance testing
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
    /// <remarks>
    /// The current tests only test the main-game-loop oriented functions of ScreenObject.  These tests have the following notable characteristics:
    ///     - The children are all themselves ScreenObject instances, with 0 components and 0 children
    ///         - This means that optimizations to the ScreenObject functions recursively cause performance gains when children are processed
    ///     - The components are processed in all 4 elements of the main game loop (Update, Render, ProcessKeyboard, ProcessMouse), but the functions called for these
    ///       events do nothing (blank implementation).
    ///         - This is clearly not a realistic use case; it means the tests provide a gauge as to what the OVERHEAD of children/component iteration is, without taking into account
    ///           whether or not that value is significant relative to the costs of the actual handling functions in a realistic use case.
    ///     - No changes are made to the Children or components lists during the actual measured performance test; only in the initial setup which is NOT timed.
    /// </remarks>
    public class ScreenObject
    {
        #region Test Data

        [Params(0, 5, 10)]
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

            for (int i = 0; i < NumChildren; i++)
            {
                var obj = new SadConsole.ScreenObject();

                for (int i2 = 0; i2 < NumComponents; i2++)
                    obj.SadComponents.Add(new TestComponent());

                _mainObject.Children.Add(obj);
            }

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
