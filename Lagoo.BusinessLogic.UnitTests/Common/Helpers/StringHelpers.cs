using System;
using System.Linq;

namespace Lagoo.BusinessLogic.UnitTests.Common.Helpers;

/// <summary>
///   Helper methods for working with strings
/// </summary>
public static class StringHelpers
{
    private static readonly Random Random = new();
    
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}