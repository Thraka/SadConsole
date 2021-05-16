using System.Text;

namespace SadConsole
{
    /// <summary>
    /// Extensions for the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions2
    {
        /// <summary>
        /// Converts a string into codepage 437.
        /// </summary>
        /// <param name="text">The string to convert</param>
        /// <param name="codepage">Optional codepage to provide.</param>
        /// <returns>A transformed string.</returns>
        public static string ToAscii(this string text, int codepage = 437)
        {
            // Converts characters such as ░▒▓│┤╡ ☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼■²ⁿ√·
            byte[] stringBytes = CodePagesEncodingProvider.Instance.GetEncoding(437).GetBytes(text);
            char[] stringChars = new char[stringBytes.Length];

            for (int i = 0; i < stringBytes.Length; i++)
                stringChars[i] = (char)stringBytes[i];

            return new string(stringChars);
        }
    }
}
