using FrameworkPoint = Microsoft.Xna.Framework.Point;
using FrameworkRect = Microsoft.Xna.Framework.Rectangle;
using FrameworkColor = Microsoft.Xna.Framework.Color;

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SadConsole.SerializedTypes
{
    [DataContract]
    public class ConsoleSerialized
    {
        [DataMember]
        public PointSerialized Position;
        [DataMember]
        public bool IsVisible;
        [DataMember]
        public bool IsPaused;
        [DataMember]
        public bool AutoCursorOnFocus;
        [DataMember]
        public bool MoveToFrontOnMouseClick;
        [DataMember]
        public bool UseKeyboard;
        [DataMember]
        public bool UseMouse;
        [DataMember]
        public SadConsole.Console.ActiveBehavior FocusedMode;
        [DataMember]
        public bool UsePixelPositioning;

        public static implicit operator ConsoleSerialized(SadConsole.Console console)
        {
            return new ConsoleSerialized()
            {
                Position = console.Position,
                IsVisible = console.IsVisible,
                IsPaused = console.IsPaused,
                AutoCursorOnFocus = console.AutoCursorOnFocus,
                MoveToFrontOnMouseClick = console.MoveToFrontOnMouseClick,
                UseKeyboard = console.UseKeyboard,
                UseMouse = console.UseMouse,
                FocusedMode = console.FocusedMode,
                UsePixelPositioning = console.UsePixelPositioning
            };
        }

        public static implicit operator SadConsole.Console(ConsoleSerialized console)
        {
            return new SadConsole.Console(1, 1)
            {
                Position = console.Position,
                IsVisible = console.IsVisible,
                IsPaused = console.IsPaused,
                AutoCursorOnFocus = console.AutoCursorOnFocus,
                MoveToFrontOnMouseClick = console.MoveToFrontOnMouseClick,
                UseKeyboard = console.UseKeyboard,
                UseMouse = console.UseMouse,
                FocusedMode = console.FocusedMode,
                UsePixelPositioning = console.UsePixelPositioning
            };
        }
    }

}
