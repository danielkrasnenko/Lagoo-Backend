using System.ComponentModel;

namespace Lagoo.BusinessLogic.Common.Extensions;

/// <summary>
///   Extension method for <see cref="Enum"/>
/// </summary>
public static class EnumExtensions
{
    public static string GetEnumDescription<TEnum>(this TEnum source) where TEnum : Enum
    {
        var descriptionAttribute = source.GetAttributeFromEnum<TEnum, DescriptionAttribute>();
        
        return descriptionAttribute?.Description ?? source.ToString();
    }

    private static TAttribute? GetAttributeFromEnum<TEnum, TAttribute>(this TEnum source) where TAttribute : Attribute
    {
        if (source is null)
        {
            return null;
        }

        var enumType = typeof(TEnum);
        var enumName = enumType.GetEnumName(source);

        if (enumName is null)
        {
            return null;
        }
        
        var memberInfo = enumType.GetMember(enumName);
        var attribute = memberInfo[0].GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;

        return attribute;
    }
}