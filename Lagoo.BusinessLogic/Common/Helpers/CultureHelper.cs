using System.Globalization;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.Domain.Enums;

namespace Lagoo.BusinessLogic.Common.Helpers;

public static class CultureHelper
{
    public static CultureInfo[] GetSupportedCulturesInfo()
    {
        var enumValues = Enum.GetValues(typeof(Culture)).Cast<Culture>();

        return enumValues.Select(@enum => new CultureInfo(@enum.GetEnumDescription())).ToArray();
    }

    public static List<Culture> GetSupportedCultures => EnumHelper.GetAllMemberValues<Culture>();
}