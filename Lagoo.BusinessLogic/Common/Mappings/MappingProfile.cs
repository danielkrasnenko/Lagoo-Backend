using System.Reflection;
using AutoMapper;

namespace Lagoo.BusinessLogic.Common.Mappings;

/// <summary>
///  Mappings creation via reflection
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly
            .GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IMapFrom<>)));

        const string methodName = nameof(IMapFrom<int>.Mapping);
        var interfaceName = typeof(IMapFrom<>).Name;
        
        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod(methodName) ?? type.GetInterface(interfaceName)?.GetMethod(methodName);

            methodInfo?.Invoke(instance, new object?[] { this });
        }
    }
}