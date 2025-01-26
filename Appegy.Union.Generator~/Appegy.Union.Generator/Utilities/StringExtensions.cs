namespace Appegy.Union.Generator.Utilities;

public static class StringExtensions
{
    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    /// <param name="input">The input string to convert.</param>
    /// <returns>The string in camelCase format.</returns>
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Ensure the first character is lowercase.
        var firstChar = char.ToLower(input[0]);

        // If the string has only one character, return it as lowercase.
        if (input.Length == 1)
        {
            return firstChar.ToString();
        }

        // Combine the lowercase first character with the rest of the string.
        return firstChar + input.Substring(1);
    }
}