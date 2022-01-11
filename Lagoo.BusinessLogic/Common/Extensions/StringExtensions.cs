using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Lagoo.BusinessLogic.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///   Checks whether a given string is a valid Email address
    /// </summary>
    /// <param name="source">String to check</param>
    /// <returns>Boolean value representing the result of a check</returns>
    public static bool IsEmail(this string source) => MailAddress.TryCreate(source, out _);

    /// <summary>
    ///   Removes double spaces in a string
    /// </summary>
    /// <param name="source">A string to remove double spaces from</param>
    /// <returns>Returns a string without double spaces</returns>
    public static string RemoveDoubleSpaces(this string source) => Regex.Replace(source, @"(\s)\s+", "$1");
    
    /// <summary>
    ///   Replaces spaces by underscores
    /// </summary>
    /// <param name="source">A string to replace spaces by underscores in</param>
    /// <returns>A new trimmed string with underscores instead of spaces</returns>
    public static string ReplaceSpacesByUnderscores(this string source) =>
        source.RemoveDoubleSpaces().Trim().Replace(' ', '_');
}