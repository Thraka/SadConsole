using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole.Configuration;
using SadConsole.Input;
using SadRogue.Primitives;
// ReSharper disable MemberCanBePrivate.Global

namespace SadConsole.Configuration
{
    /// <summary>
    /// Extensions class for configuring focused object tint debugging.
    /// </summary>
    public static class DebugExtensionsFocusedTint
    {
        /// <summary>
        /// Adds a <see cref="GameHost.RootComponents"/> component that tints the currently focused object.
        /// </summary>
        /// <param name="builder">The config builder.</param>
        /// <returns>The config builder.</returns>
        public static Builder EnableFocusedObjectTintDebug(this Builder builder) =>
            EnableFocusedObjectTintDebug(builder, Color.Yellow.SetAlpha(128));

        /// <summary>
        /// Adds a <see cref="GameHost.RootComponents"/> component that tints the object under the mouse.
        /// </summary>
        /// <param name="builder">The config builder.</param>
        /// <param name="tintColor">The color to tint objects.</param>
        /// <returns>The config builder.</returns>
        public static Builder EnableFocusedObjectTintDebug(this Builder builder, Color tintColor)
        {
            Components.DebugFocusedObjectTint config = builder.GetOrCreateConfig<Components.DebugFocusedObjectTint>();
            config.TintColor = tintColor;
            return builder;
        }
    }
}

namespace SadConsole.Components
{
    /// <summary>
    /// Tints a surface when that surface is focused. Helps debug which object is focused in <see cref="GameHost.FocusedScreenObjects"/>.
    /// </summary>
    public class DebugFocusedObjectTint : RootComponent, IConfigurator
    {
        private IScreenSurface? _previousObject;
        private Color _previousObjectTint = Color.Transparent;
        private readonly Mouse _mouse = new Mouse();
        private bool _isEnabled = true;

        /// <summary>
        /// The tint color to apply to the focused object.
        /// </summary>
        public Color TintColor { get; set; } = Color.Yellow.SetAlpha(128);

        /// <summary>
        /// When <see langword="false"/>, disables this component and clears the tinted object.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;

                if (!value && _previousObject != null)
                {
                    _previousObject.Tint = _previousObjectTint;
                    _previousObject = null;
                }
            }
        }

        /// <inheritdoc />
        public override void Run(TimeSpan delta)
        {
            if (!IsEnabled) return;

            // Build a list of all screen objects
            if (_previousObject is not null)
                _previousObject.Tint = _previousObjectTint;

            // Set new object
            if (GameHost.Instance.FocusedScreenObjects.ScreenObject is IScreenSurface screenSurface)
            {
                _previousObject = screenSurface;
                _previousObjectTint = _previousObject.Tint;
                _previousObject.Tint = TintColor;
            }
            else
                _previousObject = null;
        }

        void IConfigurator.Run(Builder config, GameHost game) =>
            game.RootComponents.Add(this);
    }
}
