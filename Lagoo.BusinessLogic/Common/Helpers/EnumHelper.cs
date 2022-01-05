namespace Lagoo.BusinessLogic.Common.Helpers;

/// <summary>
///   A helper for working with <see cref="Enum"/>
/// </summary>
public static class EnumHelper
{
    public static List<TEnum> GetAllMemberValues<TEnum>()
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException("Specified type is not an enum", nameof(TEnum));
        }
        
        var enumType = typeof(TEnum);
        
        return Enum.GetValues(enumType).Cast<TEnum>().ToList();
    }
    
    public static List<TEnum> GetAllMemberNames<TEnum>()
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException("Specified type is not an enum", nameof(TEnum));
        }
        
        var enumType = typeof(TEnum);
        
        return Enum.GetNames(enumType).Cast<TEnum>().ToList();
    }
}