using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Web.Models.Entities;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Helpers
{
    public static class MapperHelper
    {
        public static void HandleMappings()
        {
            Mapper.CreateMap<Profile, ProfileModel>();
            Mapper.CreateMap<ProfileModel, Profile>()
                .ForMember(dest => dest.BreastSize, opt => opt.MapFrom(src => (byte?) src.BreastSize))
                .ForMember(dest => dest.DickSize, opt => opt.MapFrom(src => (byte?) src.DickSize))
                .ForMember(dest => dest.DickThickness, opt => opt.MapFrom(src => (byte?) src.DickThickness));
                

            Mapper.CreateMap<ConversationModel, Conversation>();
            Mapper.CreateMap<Conversation, ConversationModel>();

            Mapper.CreateMap<CountriesToVisit, CountriesToVisitModel>();
            Mapper.CreateMap<CountriesToVisitModel, CountriesToVisit>();

            Mapper.CreateMap<LanguagesSpoken, LanguagesSpokenModel>();
            Mapper.CreateMap<LanguagesSpokenModel, LanguagesSpoken>();

            Mapper.CreateMap<Photo, PhotoModel>();
            Mapper.CreateMap<PhotoModel, Photo>();

            Mapper.CreateMap<SearchingFor, SearchingForModel>();
            Mapper.CreateMap<SearchingForModel, SearchingFor>();

            Mapper.CreateMap<State, StateModel>();
            Mapper.CreateMap<StateModel, State>();

            Mapper.CreateMap<Visit, VisitModel>();
            Mapper.CreateMap<VisitModel, Visit>();

            Mapper.CreateMap<Profile, ProfileModel>();
            Mapper.CreateMap<ProfileModel, Profile>();
        }
    }
}