using AutoMapper;

namespace Lagoo.BusinessLogic.Common.Mappings;

/// <summary>
///  Mapping from <see cref="TSource"/> to corresponding class
/// </summary>
/// <typeparam name="TSource">Type from which mapping's happening</typeparam>
public interface IMapFrom<TSource>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(TSource), GetType());
}