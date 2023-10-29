using System.Text.RegularExpressions;

namespace SadConsole;

/// <summary>
/// Validates a string
/// </summary>
public static partial class StringValidation
{
    /// <summary>
    /// The object produced when validating a string.
    /// </summary>
    public readonly struct Result
    {
        /// <summary>
        /// Indicates whether or not this result is valid.
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// The error message when <see cref="IsValid"/> is <see langword="false"/>. May be an empty string when there isn't a message.
        /// </summary>
        public readonly string ErrorMessage;

        /// <summary>
        /// Creates a new string validation result.
        /// </summary>
        /// <param name="isValid">A value to indicate that the result is valid or not.</param>
        /// <param name="errorMsg">A message associated with the result when <paramref name="isValid"/> is <see langword="false"/>.</param>
        public Result(bool isValid, string? errorMsg = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMsg ?? string.Empty;
        }

        /// <summary>
        /// A successful result.
        /// </summary>
        /// <returns>A successful result.</returns>
        public static Result Success() =>
            new(true, string.Empty);
    }

    /// <summary>
    /// Validates a string value.
    /// </summary>
    /// <param name="input">The string to validate.</param>
    /// <returns>A result that indicates whether or not the string value has been validated.</returns>
    public delegate Result Validator(string input);

    /// <summary>
    /// Always validates to true.
    /// </summary>
    /// <param name="input">The string to validate.</param>
    /// <returns>A positive validation result.</returns>
    public static Result None(string input) =>
        Result.Success();

    /// <summary>
    /// Returns a <see cref="Validator"/> delegate that validates whether or not a string can be parsed by the <see cref="long.TryParse(string?, out long)"/>.
    /// </summary>
    /// <param name="errorMessage">A message to provide to the result if the validation fails.</param>
    /// <returns>The validation result.</returns>
    public static Validator Integer(string errorMessage) =>
        (input) => new Result(long.TryParse(input, out _), errorMessage);

    /// <summary>
    /// Returns a <see cref="Validator"/> delegate that validates whether or not a string can be parsed by the <see cref="double.TryParse(string?, out double)"/>.
    /// </summary>
    /// <param name="errorMessage">A message to provide to the result if the validation fails.</param>
    /// <returns>The validation result.</returns>
    public static Validator Decimal(string errorMessage) =>
        (input) => new Result(double.TryParse(input, out _), errorMessage);

    /// <summary>
    /// Returns a <see cref="Validator"/> delegate that validates whether or not a length of a string falls within the specified range.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="errorMessage">A message to provide to the result if the validation fails.</param>
    /// <returns>The validation result.</returns>
    public static Validator LengthRange(int min, int max, string errorMessage) =>
        (input) => new Result(input.Length >= min && input.Length <= max, errorMessage);

    /// <summary>
    /// Uses multiple <see cref="Validator"/> delegates to validate a string. All validators must pass.
    /// </summary>
    /// <param name="validators">Each <see cref="Validator"/> to use.</param>
    /// <returns>When all validators pass, a successful result; otherwise, the failed result.</returns>
    public static Validator All(params Validator[] validators) =>
        (input) =>
        {
            foreach (Validator v in validators)
            {
                Result result = v(input);

                if (!result.IsValid) return result;
            }

            return Result.Success();
        };

#if NET7_0_OR_GREATER
    /// <summary>
    /// Returns a <see cref="Validator"/> delegate that validates whether or not a string contains only letters.
    /// </summary>
    /// <param name="errorMessage">A message to provide to the result if the validation fails.</param>
    /// <returns>The validation result.</returns>
    public static Validator Letters(string errorMessage) =>
        (input) => new Result(LettersOnly().IsMatch(input), errorMessage);

    [GeneratedRegex("[a-zA-Z]")]
    private static partial Regex LettersOnly();
#else
    /// <summary>
    /// Returns a <see cref="Validator"/> delegate that validates whether or not a string contains only letters.
    /// </summary>
    /// <param name="errorMessage">A message to provide to the result if the validation fails.</param>
    /// <returns>The validation result.</returns>
    public static Validator Letters(string errorMessage) =>
        (input) => new Result(new Regex("[a-zA-Z]").IsMatch(input), errorMessage);
#endif
}
