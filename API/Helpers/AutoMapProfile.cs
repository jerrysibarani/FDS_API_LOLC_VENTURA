
using API.Data.Entities;
using API.Models.Views;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapProfile : Profile
    {
        public AutoMapProfile()
        {
            CreateMap<DataModels, DOCUMENTS>();
            //CreateMap<CertificateModel, HISTORY_CERTIFICATE>();
            // Change this in your Profile or Mapping configuration
            CreateMap<CertificateModel, HISTORY_CERTIFICATE>().ReverseMap();



            //CreateMap<User, UserDto>()
            // .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
            // .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress));

        }
    }
}
