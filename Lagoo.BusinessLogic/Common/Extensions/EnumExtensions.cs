using System.ComponentModel;

namespace Lagoo.BusinessLogic.Common.Extensions;

/// <summary>
///   Extension method for <see cref="Enum"/>
/// </summary>
public static class EnumExtensions
{
    public static string GetEnumDescription<TEnum>(this TEnum @enum) where TEnum : Enum
    {
        var descriptionAttribute = @enum.GetAttributeFromEnum<TEnum, DescriptionAttribute>();
        
        return descriptionAttribute?.Description ?? @enum.ToString();
    }

    private static TAttribute? GetAttributeFromEnum<TEnum, TAttribute>(this TEnum @enum) where TAttribute : Attribute
    {
        if (@enum is null)
        {
            return null;
        }

        var enumType = typeof(TEnum);
        var enumName = enumType.GetEnumName(@enum);

        if (enumName is null)
        {
            return null;
        }
        
        var memberInfo = enumType.GetMember(enumName);
        var attribute = memberInfo[0].GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;

        return attribute;
    }
}