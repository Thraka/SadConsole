using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.GameHelpers
{
    /// <summary>
    /// Produces values based on a minimum and maximum range.
    /// </summary>
    public struct RangeInt
    {
        /// <summary>
        /// The maximum value.
        /// </summary>
        public int Maximum;

        /// <summary>
        /// The minimum value.
        /// </summary>
        public int Minimum;

        /// <summary>
        /// Gets a random number between the <see cref="Minimum"/> and <see cref="Maximum"/>.
        /// </summary>
        /// <returns>A random number.</returns>
        public int Get()
        {
            return Maximum == Minimum ? Maximum : Global.Random.Next(Maximum, Minimum);
        }

        public static implicit operator int(RangeInt i)
        {
            return i.Get();
        }
        public static implicit operator RangeInt(int i)
        {
            return new RangeInt() { Maximum = i, Minimum = i };
        }
    }

    /// <summary>
    /// Produces values based on a minimum and maximum range.
    /// </summary>
    public struct RangeDouble
    {
        /// <summary>
        /// The maximum value.
        /// </summary>
        public double Maximum;

        /// <summary>
        /// The minimum value.
        /// </summary>
        public double Minimum;

        /// <summary>
        /// Gets a random number between the <see cref="Minimum"/> and <see cref="Maximum"/>.
        /// </summary>
        /// <returns>A random number.</returns>
        public double Get()
        {
            return Maximum == Minimum ? Maximum : Global.Random.NextDouble() * Maximum - Minimum;
        }

        public static implicit operator double(RangeDouble d)
        {
            return d.Get();
        }
        public static implicit operator RangeDouble(double d)
        {
            return new RangeDouble() { Maximum = d, Minimum = d };
        }
    }
}
