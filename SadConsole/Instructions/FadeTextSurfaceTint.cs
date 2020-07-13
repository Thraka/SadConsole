using System;
using SadRogue.Primitives;

namespace SadConsole.Instructions
{
    /// <summary>
    /// Animates a color change to <see cref="ScreenSurface.Tint"/>.
    /// </summary>
    public class FadeTextSurfaceTint : AnimatedValue
    {
        private IScreenSurface _objectSurface;
        private ColorGradient _colors;

        /// <summary>
        /// The color to fade the tint to.
        /// </summary>
        public ColorGradient Colors
        {
            get => _colors;
            set => _colors = value ?? throw new Exception($"{nameof(Colors)} can't be set to null");
        }

        /// <summary>
        /// Creates a new tint fade instruction that targets the specified console.
        /// </summary>
        /// <param name="objectSurface">The <see cref="IScreenSurface.Tint"/> to fade.</param>
        /// <param name="colors">The gradient pattern to fade through.</param>
        /// <param name="duration">How long the fade takes.</param>
        public FadeTextSurfaceTint(IScreenSurface objectSurface, ColorGradient colors, TimeSpan duration): base(duration, 0d, 1d)
        {
            Colors = colors;
            _objectSurface = objectSurface;
        }

        /// <summary>
        /// Creates a new tint fade instruction that uses the console passed to <see cref="SadConsole.Components.IComponent.Update(IScreenObject, TimeSpan)"/>.
        /// </summary>
        /// <param name="colors">The gradient pattern to fade through.</param>
        /// <param name="duration">How long the fade takes.</param>
        public FadeTextSurfaceTint(ColorGradient colors, TimeSpan duration) : base(duration, 0d, 1d)
        {
            Colors = colors;
        }

        /// <summary>
        /// Creates a new tint fade instruction with default settings that uses the console passed to <see cref="SadConsole.Components.IComponent.Update(IScreenObject, TimeSpan)"/>.
        /// </summary>
        /// <remarks>
        /// The default settings are:
        /// 
        ///   - <see cref="Colors"/>: <see cref="Color.White"/> to <see cref="Color.Black"/>
        ///   - Duration: 1 second
        /// </remarks>
        public FadeTextSurfaceTint() : base(TimeSpan.FromSeconds(1), 0d, 1d)
        {
            Colors = new ColorGradient(Color.White, Color.Transparent);
        }

        /// <inheritdoc />
        public override void Update(IScreenObject componentHost, TimeSpan delta)
        {
            if (!IsFinished)
            {
                base.Update(componentHost, delta);

                var target = _objectSurface ?? componentHost as IScreenSurface;

                target.Tint = Colors.Lerp((float)Value);
            }
        }
    }
}
