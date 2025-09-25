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
    /// Extensions class for configuring mouse tint debugging.
    /// </summary>
    public static class DebugExtensionsMouseTint
    {
        /// <summary>
        /// Adds a <see cref="GameHost.RootComponents"/> component that tints the object under the mouse <see cref="Color.Purple"/>.
        /// </summary>
        /// <param name="builder">The config builder.</param>
        /// <returns>The config builder.</returns>
        public static Builder EnableMouseTintDebug(this Builder builder) =>
            EnableMouseTintDebug(builder, Color.Purple.SetAlpha(128));

        /// <summary>
        /// Adds a <see cref="GameHost.RootComponents"/> component that tints the object under the mouse.
        /// </summary>
        /// <param name="builder">The config builder.</param>
        /// <param name="tintColor">The color to tint objects.</param>
        /// <returns>The config builder.</returns>
        public static Builder EnableMouseTintDebug(this Builder builder, Color tintColor)
        {
            Components.DebugMouseTint config = builder.GetOrCreateConfig<Components.DebugMouseTint>();
            config.TintColor = tintColor;
            return builder;
        }
    }
}

namespace SadConsole.Components
{
    /// <summary>
    /// Tints a surface when that surface would use the mouse. Helps debug which object is receiving mouse input as you move the mouse around.
    /// </summary>
    public class DebugMouseTint : RootComponent, IConfigurator
    {
        private IScreenSurface? _previousObject;
        private Color _previousObjectTint = Color.Transparent;
        private readonly Mouse _mouse = new Mouse();
        private bool _isEnabled = true;

        /// <summary>
        /// The tint color to apply to the object under the mouse.
        /// </summary>
        public Color TintColor { get; set; } = Color.Purple.SetAlpha(128);

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
            _mouse.Update(delta);

            // Build a list of all screen objects
            var screenObjects = new List<IScreenObject>();
            GetConsoles(GameHost.Instance.Screen!, ref screenObjects);

            // Process top-most screen objects first.
            screenObjects.Reverse();

            IScreenSurface? newObject = (from screenObj in screenObjects
                                         let state = new MouseScreenObjectState(screenObj, _mouse)
                                         where state.IsOnScreenObject
                                         select (IScreenSurface)screenObj).FirstOrDefault();

            // Reset old object
            if (_previousObject is not null)
                _previousObject.Tint = _previousObjectTint;

            // Set new object
            if (newObject is not null)
            {
                _previousObject = newObject;
                _previousObjectTint = _previousObject.Tint;
                _previousObject.Tint = TintColor;
            }
            else
                _previousObject = null;
        }

        private static void GetConsoles(IScreenObject screen, ref List<IScreenObject> list)
        {
            if (!screen.IsVisible) return;

            if (screen.UseMouse)
                list.Add(screen);

            foreach (var child in screen.Children)
            {
                GetConsoles(child, ref list);
            }
        }

        void IConfigurator.Run(BuilderBase config, GameHost game) =>
            game.RootComponents.Add(this);
    }
}
