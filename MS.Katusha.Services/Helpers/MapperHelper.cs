using AutoMapper;

namespace MS.Katusha.Services.Helpers
{
    public static class MapperHelper
    {
        public static void HandleMappings()
        {
            Mapper.CreateMap<MS.Katusha.Domain.Entities.Conversation, MS.Katusha.Domain.Raven.Entities.Conversation>()
                .ForMember(dest => dest.FromName, opt => opt.MapFrom(src => src.From.Name))
                .ForMember(dest => dest.ToName, opt => opt.MapFrom(src => src.To.Name))
                .ForMember(dest => dest.FromGuid, opt => opt.MapFrom(src => src.From.Guid))
                .ForMember(dest => dest.ToGuid, opt => opt.MapFrom(src => src.To.Guid))
                .ForMember(dest => dest.FromPhotoGuid, opt => opt.MapFrom(src => src.From.ProfilePhotoGuid))
                .ForMember(dest => dest.ToPhotoGuid, opt => opt.MapFrom(src => src.To.ProfilePhotoGuid))
                ;
            Mapper.CreateMap<MS.Katusha.Domain.Raven.Entities.Conversation, MS.Katusha.Domain.Entities.Conversation>();
        }
    }
}