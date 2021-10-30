using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadRogue.Primitives;

namespace SadConsole.Tests
{
    /// <summary>
    /// Custom assert methods that can be of use for dealing with cases often encountered with SadConsole types.
    /// </summary>
    public static class AssertExtensions
    {
        /// <summary>
        /// Checks that the two IMatchable implementations given match according to the match function.
        /// </summary>
        /// <param name="m1"/>
        /// <param name="m2"/>
        /// <typeparam name="T">True if the given IMatchable implementations match, false otherwise.</typeparam>
        public static void Matches<T>(T m1, T m2)
            where T : IMatchable<T>
        {
            Assert.IsTrue(m1.Matches(m2));
        }
    }
}
