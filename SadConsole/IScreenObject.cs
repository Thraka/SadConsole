using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadConsole
{
    /// <summary>
    /// A generic object processed by SadConsole. Provides parent/child, components, and position.
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
        ObservableCollection<IComponent> Components { get; }

        /// <summary>
        /// Gets or sets the visibility of this object.
        /// </summary>
        bool IsEnabled { get; set; }

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
        /// Draws all <see cref="Components"/> and <see cref="Children"/>.
        /// </summary>
        /// <remarks>Only processes if <see cref="IsVisible"/> is <see langword="true"/>.</remarks>
        void Draw();

        /// <summary>
        /// Gets components of the specified types.
        /// </summary>
        /// <typeparam name="TComponent">THe component to find</typeparam>
        /// <returns>The components found.</returns>
        IComponent GetComponent<TComponent>() where TComponent : IComponent;

        /// <summary>
        /// Gets the first component of the specified type.
        /// </summary>
        /// <typeparam name="TComponent">THe component to find</typeparam>
        /// <returns>The component if found, otherwise null.</returns>
        IEnumerable<IComponent> GetComponents<TComponent>() where TComponent : IComponent;

        /// <summary>
        /// Called by the engine to process the keyboard.
        /// </summary>
        /// <param name="keyboard">Keyboard information.</param>
        /// <returns>True when the keyboard had data and this console did something with it.</returns>
        bool ProcessKeyboard(Keyboard keyboard);

        /// <summary>
        /// Updates all <see cref="Components"/> and <see cref="Children"/>.
        /// </summary>
        /// <remarks>Only processes if <see cref="IsEnabled"/> is <see langword="true"/>.</remarks>
        void Update();

        /// <summary>
        /// Sets a value for <see cref="AbsolutePosition"/> based on the <see cref="Position"/> of this instance and the <see cref="Parent"/> instance.
        /// </summary>
        void UpdateAbsolutePosition();
    }
}
