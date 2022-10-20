using System.Reflection;
using AutoMapper;

namespace Lagoo.BusinessLogic.Common.Mappings;

/// <summary>
///   Mappings creation via reflection
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
                (i.GetGenericTypeDefinition() == typeof(IMapFrom<>)
                 || i.GetGenericTypeDefinition() == typeof(IMapTo<>))));

        const string methodNameOfMappingFrom = nameof(IMapFrom<int>.Mapping);
        var interfaceNameOfMappingFrom = typeof(IMapFrom<>).Name;
        
        const string methodNameOfMappingTo = nameof(IMapFrom<int>.Mapping);
        var interfaceNameOfMappingTo = typeof(IMapFrom<>).Name;
        
        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            
            var methodInfo = type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                ? GetMethodInfo(type, methodNameOfMappingFrom, interfaceNameOfMappingFrom)
                : GetMethodInfo(type, methodNameOfMappingTo, interfaceNameOfMappingTo);

            methodInfo?.Invoke(instance, new object?[] { this });
        }
    }
    
    private MethodInfo? GetMethodInfo(Type type, string methodName, string interfaceName)
    {
        return type.GetMethod(methodName) ?? type.GetInterface(interfaceName)?.GetMethod(methodName);
    }
}