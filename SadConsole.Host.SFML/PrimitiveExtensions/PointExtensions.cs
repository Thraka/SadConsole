using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SadRogue.Primitives;
using SFML.System;
using SadRoguePoint = SadRogue.Primitives.Point;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for <see cref="SadRogue.Primitives.Point"/> that enable operations involving
    /// similar types from SFML.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class SadRoguePointExtensions
    {
        /// <summary>
        /// Converts from <see cref="SadRogue.Primitives.Point"/> to <see cref="SFML.System.Vector2i"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i ToVector2i(this SadRoguePoint self) => new Vector2i(self.X, self.Y);

        /// <summary>
        /// Converts from <see cref="SadRogue.Primitives.Point"/> to <see cref="SFML.System.Vector2u"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u ToVector2u(this SadRoguePoint self) => new Vector2u((uint)self.X, (uint)self.Y);

        /// <summary>
        /// Converts from <see cref="SadRogue.Primitives.Point"/> to <see cref="SFML.System.Vector2f"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f ToVector2f(this SadRoguePoint self) => new Vector2f(self.X, self.Y);

        /// <summary>
        /// Adds a <see cref="SFML.System.Vector2i"/> to a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Add(this SadRoguePoint self, Vector2i other)
            => new SadRoguePoint(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Adds a <see cref="SFML.System.Vector2u"/> to a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Add(this SadRoguePoint self, Vector2u other)
            => new SadRoguePoint(self.X + (int)other.X, self.Y + (int)other.Y);

        /// <summary>
        /// Adds a <see cref="SFML.System.Vector2f"/> to a <see cref="SadRogue.Primitives.Point"/>, and rounds the X
        /// and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Add(this SadRoguePoint self, Vector2f other)
            => new SadRoguePoint((int)Math.Round(self.X + other.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y + other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Subtracts a <see cref="SFML.System.Vector2i"/> from a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Subtract(this SadRoguePoint self, Vector2i other)
            => new SadRoguePoint(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Subtracts a <see cref="SFML.System.Vector2u"/> from a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Subtract(this SadRoguePoint self, Vector2u other)
            => new SadRoguePoint(self.X - (int)other.X, self.Y - (int)other.Y);

        /// <summary>
        /// Subtracts a <see cref="SFML.System.Vector2f"/> from a <see cref="SadRogue.Primitives.Point"/>, and rounds
        /// the resulting X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Subtract(this SadRoguePoint self, Vector2f other)
            => new SadRoguePoint((int)Math.Round(self.X - other.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y - other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Multiplies a <see cref="SadRogue.Primitives.Point"/> by a <see cref="SFML.System.Vector2i"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Multiply(this SadRoguePoint self, Vector2i other)
            => new SadRoguePoint(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Multiplies a <see cref="SadRogue.Primitives.Point"/> by a <see cref="SFML.System.Vector2u"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Multiply(this SadRoguePoint self, Vector2u other)
            => new SadRoguePoint(self.X * (int)other.X, self.Y * (int)other.Y);

        /// <summary>
        /// Multiplies a <see cref="SadRogue.Primitives.Point"/> by a <see cref="SFML.System.Vector2f"/>, and rounds
        /// the resulting X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Multiply(this SadRoguePoint self, Vector2f other)
            => new SadRoguePoint((int)Math.Round(self.X * other.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y * other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides a <see cref="SadRogue.Primitives.Point"/> by a <see cref="SFML.System.Vector2i"/>, and rounds the
        /// resulting X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Divide(this SadRoguePoint self, Vector2i other)
            => new SadRoguePoint((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides a <see cref="SadRogue.Primitives.Point"/> by a <see cref="SFML.System.Vector2u"/>, and rounds the
        /// resulting X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Divide(this SadRoguePoint self, Vector2u other)
            => new SadRoguePoint((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides a <see cref="SadRogue.Primitives.Point"/> by a <see cref="SFML.System.Vector2f"/>, and rounds the
        /// resulting X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint Divide(this SadRoguePoint self, Vector2f other)
            => new SadRoguePoint((int)Math.Round(self.X / other.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y / other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Point"/> to a <see cref="SFML.System.Vector2i"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRoguePoint self, Vector2i other) => self.X == other.X && self.Y == other.Y;

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Point"/> to a <see cref="SFML.System.Vector2u"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRoguePoint self, Vector2u other) => self.X == other.X && self.Y == other.Y;

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Point"/> to a <see cref="SFML.System.Vector2f"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRoguePoint self, Vector2f other)
            => Math.Abs(self.X - other.X) < 0.0000000001 && Math.Abs(self.Y - other.Y) < 0.0000000001;
    }
}

namespace SFML.System
{
    /// <summary>
    /// Extension methods for <see cref="SFML.System.Vector2i"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Point"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Vector2iExtensions
    {
        /// <summary>
        /// Converts from <see cref="SFML.System.Vector2i"/> to <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint ToPoint(this Vector2i self) => new SadRoguePoint(self.X, self.Y);

        /// <summary>
        /// Adds a <see cref="SadRogue.Primitives.Point"/> to a <see cref="SFML.System.Vector2i"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Add(this Vector2i self, SadRoguePoint other)
            => new Vector2i(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Adds an integer to both the X and Y values of a <see cref="SFML.System.Vector2i"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Add(this Vector2i self, int i) => new Vector2i(self.X + i, self.Y + i);

        /// <summary>
        /// Adds a <see cref="SadRogue.Primitives.Direction"/> to a <see cref="SFML.System.Vector2i"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="dir"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Add(this Vector2i self, Direction dir)
            => new Vector2i(self.X + dir.DeltaX, self.Y + dir.DeltaY);

        /// <summary>
        /// Subtracts a <see cref="SadRogue.Primitives.Point"/> from a <see cref="SFML.System.Vector2i"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Subtract(this Vector2i self, SadRoguePoint other)
            => new Vector2i(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Subtracts an integer from both the X and Y values of a <see cref="SFML.System.Vector2i"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Subtract(this Vector2i self, int i) => new Vector2i(self.X - i, self.Y - i);

        /// <summary>
        /// Subtracts a <see cref="SadRogue.Primitives.Direction"/> from a <see cref="SFML.System.Vector2i"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="dir"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Subtract(this Vector2i self, Direction dir)
            => new Vector2i(self.X - dir.DeltaX, self.Y - dir.DeltaY);

        /// <summary>
        /// Multiplies a <see cref="SFML.System.Vector2i"/> by a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Multiply(this Vector2i self, SadRoguePoint other)
            => new Vector2i(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Multiplies the X and Y values of a <see cref="SFML.System.Vector2i"/> by an integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Multiply(this Vector2i self, int i) => new Vector2i(self.X * i, self.Y * i);

        /// <summary>
        /// Multiplies the X and Y values of a <see cref="SFML.System.Vector2i"/> by a double, then rounds the
        /// values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Multiply(this Vector2i self, double d)
            => new Vector2i((int)Math.Round(self.X * d, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y * d, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides a <see cref="SFML.System.Vector2i"/> by a <see cref="SadRogue.Primitives.Point"/>, and rounds the
        /// resulting X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Divide(this Vector2i self, SadRoguePoint other)
            => new Vector2i((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides the X and Y values of a <see cref="SFML.System.Vector2i"/> by a double, then rounds the
        /// values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Divide(this Vector2i self, double d)
            => new Vector2i((int)Math.Round(self.X / d, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y / d, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Compares a <see cref="SFML.System.Vector2i"/> to a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this Vector2i self, SadRoguePoint other) => self.X == other.X && self.Y == other.Y;
    }

    /// <summary>
    /// Extension methods for <see cref="SFML.System.Vector2u"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Point"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Vector2uExtensions
    {
        /// <summary>
        /// Converts from <see cref="SFML.System.Vector2u"/> to <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint ToPoint(this Vector2u self) => new SadRoguePoint((int)self.X, (int)self.Y);

        /// <summary>
        /// Adds a <see cref="SadRogue.Primitives.Point"/> to a <see cref="SFML.System.Vector2u"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Add(this Vector2u self, SadRoguePoint other)
            => new Vector2u(self.X + (uint)other.X, self.Y + (uint)other.Y);

        /// <summary>
        /// Adds an integer to both the X and Y values of a <see cref="SFML.System.Vector2u"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Add(this Vector2u self, int i) => new Vector2u(self.X + (uint)i, self.Y + (uint)i);

        /// <summary>
        /// Adds a <see cref="SadRogue.Primitives.Direction"/> to a <see cref="SFML.System.Vector2u"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="dir"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Add(this Vector2u self, Direction dir)
            => new Vector2u((uint)(self.X + dir.DeltaX), (uint)(self.Y + dir.DeltaY));

        /// <summary>
        /// Subtracts a <see cref="SadRogue.Primitives.Point"/> from a <see cref="SFML.System.Vector2u"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Subtract(this Vector2u self, SadRoguePoint other)
            => new Vector2u((uint)(self.X - other.X), (uint)(self.Y - other.Y));

        /// <summary>
        /// Subtracts an integer from both the X and Y values of a <see cref="SFML.System.Vector2u"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Subtract(this Vector2u self, int i)
            => new Vector2u((uint)(self.X - i), (uint)(self.Y - i));

        /// <summary>
        /// Subtracts a <see cref="SadRogue.Primitives.Direction"/> from a <see cref="SFML.System.Vector2u"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="dir"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Subtract(this Vector2u self, Direction dir)
            => new Vector2u((uint)(self.X - dir.DeltaX), (uint)(self.Y - dir.DeltaY));

        /// <summary>
        /// Multiplies a <see cref="SFML.System.Vector2u"/> by a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Multiply(this Vector2u self, SadRoguePoint other)
            => new Vector2u((uint)(self.X * other.X), (uint)(self.Y * other.Y));

        /// <summary>
        /// Multiplies the X and Y values of a <see cref="SFML.System.Vector2u"/> by an integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Multiply(this Vector2u self, int i)
            => new Vector2u((uint)(self.X * i), (uint)(self.Y * i));

        /// <summary>
        /// Multiplies the X and Y values of a <see cref="SFML.System.Vector2u"/> by a double, then rounds the
        /// values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Multiply(this Vector2u self, double d)
            => new Vector2u((uint)Math.Round(self.X * d, MidpointRounding.AwayFromZero),
                (uint)Math.Round(self.Y * d, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides a <see cref="SFML.System.Vector2u"/> by a <see cref="SadRogue.Primitives.Point"/>, and rounds the
        /// resulting X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Divide(this Vector2u self, SadRoguePoint other)
            => new Vector2u((uint)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero),
                (uint)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides the X and Y values of a <see cref="SFML.System.Vector2u"/> by a double, then rounds the
        /// values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2u Divide(this Vector2u self, double d)
            => new Vector2u((uint)Math.Round(self.X / d, MidpointRounding.AwayFromZero),
                (uint)Math.Round(self.Y / d, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Compares a <see cref="SFML.System.Vector2u"/> to a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this Vector2u self, SadRoguePoint other) => self.X == other.X && self.Y == other.Y;
    }

    /// <summary>
    /// Extension methods for <see cref="SFML.System.Vector2f"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Point"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Vector2fExtensions
    {
        /// <summary>
        /// Converts from <see cref="SFML.System.Vector2f"/> to <see cref="SadRogue.Primitives.Point"/>,
        /// rounding the X and Y values to the nearest integer.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRoguePoint ToPoint(this Vector2f self)
            => new SadRoguePoint((int)Math.Round(self.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(self.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Adds a <see cref="SadRogue.Primitives.Point"/> to a <see cref="SFML.System.Vector2f"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Add(this Vector2f self, SadRoguePoint other)
            => new Vector2f(self.X + other.X, self.Y + other.Y);

        /// <summary>
        /// Adds an integer to both the X and Y values of a <see cref="SFML.System.Vector2f"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Add(this Vector2f self, int i) => new Vector2f(self.X + i, self.Y + i);

        /// <summary>
        /// Adds a <see cref="SadRogue.Primitives.Direction"/> to a <see cref="SFML.System.Vector2f"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="dir"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Add(this Vector2f self, Direction dir)
            => new Vector2f(self.X + dir.DeltaX, self.Y + dir.DeltaY);

        /// <summary>
        /// Subtracts a <see cref="SadRogue.Primitives.Point"/> from a <see cref="SFML.System.Vector2f"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Subtract(this Vector2f self, SadRoguePoint other)
            => new Vector2f(self.X - other.X, self.Y - other.Y);

        /// <summary>
        /// Subtracts an integer from both the X and Y values of a <see cref="SFML.System.Vector2f"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Subtract(this Vector2f self, int i) => new Vector2f(self.X - i, self.Y - i);

        /// <summary>
        /// Subtracts a <see cref="SadRogue.Primitives.Direction"/> from a <see cref="SFML.System.Vector2f"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="dir"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Subtract(this Vector2f self, Direction dir)
            => new Vector2f(self.X - dir.DeltaX, self.Y - dir.DeltaY);

        /// <summary>
        /// Multiplies a <see cref="SFML.System.Vector2f"/> by a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Multiply(this Vector2f self, SadRoguePoint other)
            => new Vector2f(self.X * other.X, self.Y * other.Y);

        /// <summary>
        /// Multiplies the X and Y values of a <see cref="SFML.System.Vector2i"/> by an integer.
        /// </summary>
        /// <param name="self"/>
        /// <param name="i"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Multiply(this Vector2f self, int i) => new Vector2f(self.X * i, self.Y * i);

        /// <summary>
        /// Multiplies the X and Y values of a <see cref="SFML.System.Vector2f"/> by a double.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Multiply(this Vector2f self, double d)
            => new Vector2f(self.X * (float)d, self.Y * (float)d);

        /// <summary>
        /// Divides a <see cref="SFML.System.Vector2f"/> by a <see cref="SadRogue.Primitives.Point"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Divide(this Vector2f self, SadRoguePoint other)
            => new Vector2f(self.X / other.X, self.Y / other.Y);

        /// <summary>
        /// Divides the X and Y values of a <see cref="SFML.System.Vector2f"/> by a double.
        /// </summary>
        /// <param name="self"/>
        /// <param name="d"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Divide(this Vector2f self, double d)
            => new Vector2f(self.X / (float)d, self.Y / (float)d);

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Point"/> to a <see cref="SFML.System.Vector2f"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [global::System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this Vector2f self, SadRoguePoint other)
            => Math.Abs(self.X - other.X) < 0.0000000001 && Math.Abs(self.Y - other.Y) < 0.0000000001;
    }
}
