using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using SadRogue.Primitives;
using MonoPoint = Microsoft.Xna.Framework.Point;
using SadRoguePoint = SadRogue.Primitives.Point;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for <see cref="SadRogue.Primitives.Point"/> that enable operations involving
    /// <see cref="Microsoft.Xna.Framework.Point"/>.
    /// </summary>
    public static class SadRoguePointExtensions
    {
        /// <summary>
        /// Converts from <see cref="SadRogue.Primitives.Point"/> to <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint ToMonoPoint(this SadRoguePoint self) => new MonoPoint(self.X, self.Y);

        /// <summary>
        /// Adds a <see cref="Microsoft.Xna.Framework.Point"/> to a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Add(this SadRoguePoint self, MonoPoint other)
            => new SadRoguePoint(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Subtracts a <see cref="Microsoft.Xna.Framework.Point"/> from a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Subtract(this SadRoguePoint self, MonoPoint other)
            => new SadRoguePoint(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Multiplies a <see cref="SadRogue.Primitives.Point"/> by a <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Multiply(this SadRoguePoint self, MonoPoint other)
            => new SadRoguePoint(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Divides a <see cref="SadRogue.Primitives.Point"/> by a <see cref="Microsoft.Xna.Framework.Point"/>, and
        /// rounds the resulting X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Divide(this SadRoguePoint self, MonoPoint other)
            => new SadRoguePoint((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Point"/> to a <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRoguePoint self, MonoPoint other) => self.X == other.X && self.Y == other.Y;
    }
}

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Extension methods for <see cref="Microsoft.Xna.Framework.Point"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Point"/>.
    /// </summary>
    public static class MonoPointExtensions
    {
        /// <summary>
        /// Converts from <see cref="Microsoft.Xna.Framework.Point"/> to <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint ToPoint(this MonoPoint self) => new SadRoguePoint(self.X, self.Y);

        /// <summary>
        /// Adds a <see cref="SadRogue.Primitives.Point"/> to a <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Add(this MonoPoint self, SadRoguePoint other)
            => new MonoPoint(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Adds an integer to both the X and Y values of a <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Add(this MonoPoint self, int i) => new MonoPoint(self.X + i, self.Y + i);

        /// <summary>
        /// Adds a <see cref="SadRogue.Primitives.Direction"/> to a <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="dir"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Add(this MonoPoint self, Direction dir)
            => new MonoPoint(self.X + dir.DeltaX, self.Y + dir.DeltaY);

        /// <summary>
        /// Subtracts a <see cref="SadRogue.Primitives.Point"/> from a <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Subtract(this MonoPoint self, SadRoguePoint other)
            => new MonoPoint(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Subtracts an integer from both the X and Y values of a <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Subtract(this MonoPoint self, int i) => new MonoPoint(self.X - i, self.Y - i);

        /// <summary>
        /// Subtracts a <see cref="SadRogue.Primitives.Direction"/> from a <see cref="Microsoft.Xna.Framework.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="dir"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Subtract(this MonoPoint self, Direction dir)
            => new MonoPoint(self.X - dir.DeltaX, self.Y - dir.DeltaY);

        /// <summary>
        /// Multiplies a <see cref="Microsoft.Xna.Framework.Point"/> by a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Multiply(this MonoPoint self, SadRoguePoint other)
            => new MonoPoint(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Multiplies the X and Y values of a <see cref="Microsoft.Xna.Framework.Point"/> by an integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Multiply(this MonoPoint self, int i) => new MonoPoint(self.X * i, self.Y * i);

        /// <summary>
        /// Multiplies the X and Y values of a <see cref="Microsoft.Xna.Framework.Point"/> by a double, then rounds the
        /// values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Multiply(this MonoPoint self, double d)
            => new MonoPoint((int)Math.Round(self.X * d, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y * d, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides a <see cref="Microsoft.Xna.Framework.Point"/> by a <see cref="SadRogue.Primitives.Point"/>, and
        /// rounds the resulting X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Divide(this MonoPoint self, SadRoguePoint other)
            => new MonoPoint((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides the X and Y values of a <see cref="Microsoft.Xna.Framework.Point"/> by a double, then rounds the
        /// values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MonoPoint Divide(this MonoPoint self, double d)
            => new MonoPoint((int)Math.Round(self.X / d, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y / d, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Compares a <see cref="Microsoft.Xna.Framework.Point"/> to a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this MonoPoint self, SadRoguePoint other) => self.X == other.X && self.Y == other.Y;
    }
}
