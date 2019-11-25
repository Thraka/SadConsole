using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole
{
    public interface IScreenObject
    {
        event EventHandler EnabledChanged;
        event EventHandler<NewOldValueEventArgs<IScreenObject>> ParentChanged;
        event EventHandler<NewOldValueEventArgs<Point>> PositionChanged;
        event EventHandler VisibleChanged;

        Point AbsolutePosition { get; }
        ScreenObjectCollection Children { get; }
        ObservableCollection<IComponent> Components { get; }
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }
        IScreenObject Parent { get; set; }
        Point Position { get; set; }
        bool UseKeyboard { get; set; }
        bool UseMouse { get; set; }
        void Draw();
        IComponent GetComponent<TComponent>() where TComponent : IComponent;
        IEnumerable<IComponent> GetComponents<TComponent>() where TComponent : IComponent;
        bool ProcessKeyboard(Keyboard keyboard);
        void Update();
        void UpdateAbsolutePosition();
    }
}
