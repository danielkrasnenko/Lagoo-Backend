using AutoMapper;

namespace Lagoo.BusinessLogic.Common.Mappings;

/// <summary>
///   Mapping to <see cref="TDestination"/> to corresponding class
/// </summary>
/// <typeparam name="TDestination">Type to which mapping's happening</typeparam>
public interface IMapTo<TDestination>
{
    void Mapping(Profile profile) => profile.CreateMap(GetType(), typeof(TDestination));
}