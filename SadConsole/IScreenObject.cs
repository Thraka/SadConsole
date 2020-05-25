using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// A generic object processed by SadConsole. Provides parent/child, components, position, and input callbacks.
    /// </summary>
    public interface IScreenObject
    {
        /// <summary>
        /// Raised when the <see cref="IsEnabled"/> property changes.
        /// </summary>
        event EventHandler EnabledChanged;

        /// <summary>
        /// Raised when the <see cref="Parent"/> property changes.
        /// </summary>
        event EventHandler<NewOldValueEventArgs<IScreenObject>> ParentChanged;

        /// <summary>
        /// Raised when the <see cref="Position"/> property changes.
        /// </summary>
        event EventHandler<NewOldValueEventArgs<Point>> PositionChanged;

        /// <summary>
        /// Raised when the <see cref="IsVisible"/> property changes.
        /// </summary>
        event EventHandler VisibleChanged;

        /// <summary>
        /// Raised when the <see cref="IsFocused"/> property is <see langword="false"/>.
        /// </summary>
        event EventHandler FocusLost;

        /// <summary>
        /// Raised when the <see cref="IsFocused"/> property is <see langword="true"/>.
        /// </summary>
        event EventHandler Focused;

        /// <summary>
        /// How the object should handle becoming active.
        /// </summary>
        FocusBehavior FocusedMode { get; set; }

        /// <summary>
        /// A position that is based on the current <see cref="Position"/> and <see cref="Parent"/> position, in pixels.
        /// </summary>
        Point AbsolutePosition { get; }

        /// <summary>
        /// The child objects of this instance.
        /// </summary>
        ScreenObjectCollection Children { get; }

        /// <summary>
        /// A collection of components processed by this console.
        /// </summary>
        ObservableCollection<IComponent> SadComponents { get; }

        /// <summary>
        /// Gets or sets the visibility of this object.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether or not this object has exclusive access to the mouse events.
        /// </summary>
        bool IsExclusiveMouse { get; set; }

        /// <summary>
        /// Gets or sets this console as the focused object for input.
        /// </summary>
        bool IsFocused { get; set; }

        /// <summary>
        /// Gets or sets the visibility of this object.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// The parent object that this instance is a child of.
        /// </summary>
        IScreenObject Parent { get; set; }

        /// <summary>
        /// The position of the object on the screen.
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// When <see langword="true"/>, this object will use the keyboard; otherwise <see langword="false"/>.
        /// </summary>
        bool UseKeyboard { get; set; }

        /// <summary>
        /// When <see langword="true"/>, this object will use the mouse; otherwise <see langword="false"/>.
        /// </summary>
        bool UseMouse { get; set; }

        /// <summary>
        /// Draws all <see cref="SadComponents"/> and <see cref="Children"/>.
        /// </summary>
        /// <param name="delta">The time that has elapsed since the last call.</param>
        /// <remarks>Only processes if <see cref="IsVisible"/> is <see langword="true"/>.</remarks>
        void Draw(TimeSpan delta);

        /// <summary>
        /// Called when this object is focused.
        /// </summary>
        void OnFocused();

        /// <summary>
        /// Called when this object's focus has been lost.
        /// </summary>
        void OnFocusLost();

        /// <summary>
        /// Gets components of the specified types.
        /// </summary>
        /// <typeparam name="TComponent">The component to find</typeparam>
        /// <returns>The components found.</returns>
        TComponent GetSadComponent<TComponent>() where TComponent : class, IComponent;

        /// <summary>
        /// Gets the first component of the specified type.
        /// </summary>
        /// <typeparam name="TComponent">The component to find.</typeparam>
        /// <returns>The component if found, otherwise null.</returns>
        IEnumerable<TComponent> GetSadComponents<TComponent>() where TComponent : class, IComponent;

        /// <summary>
        /// Indicates whether or not the component exists in the <see cref="SadComponents"/> collection.
        /// </summary>
        /// <typeparam name="TComponent">The component to find.</typeparam>
        /// <returns><see langword="true"/> when the component exists; otherwise <see langword="false"/>.</returns>
        bool HasSadComponent<TComponent>() where TComponent : IComponent;

        /// <summary>
        /// Called by the engine to process the keyboard.
        /// </summary>
        /// <param name="keyboard">Keyboard information.</param>
        /// <returns>True when the keyboard had data and this console did something with it.</returns>
        bool ProcessKeyboard(Keyboard keyboard);

        /// <summary>
        /// Processes the mouse.
        /// </summary>
        /// <param name="state">The mouse state related to this object.</param>
        /// <returns>True when this object should halt further mouse processing..</returns>
        bool ProcessMouse(MouseScreenObjectState state);

        /// <summary>
        /// Called when the mouse is being used by something else.
        /// </summary>
        /// <param name="state">The current state of the mouse based on this object.</param>
        void LostMouse(MouseScreenObjectState state);

        /// <summary>
        /// Updates all <see cref="SadComponents"/> and <see cref="Children"/>.
        /// </summary>
        /// <param name="delta">The time that has elapsed since the last call.</param>
        /// <remarks>Only processes if <see cref="IsEnabled"/> is <see langword="true"/>.</remarks>
        void Update(TimeSpan delta);

        /// <summary>
        /// Sets a value for <see cref="AbsolutePosition"/> based on the <see cref="Position"/> of this instance and the <see cref="Parent"/> instance.
        /// </summary>
        void UpdateAbsolutePosition();
    }
}
