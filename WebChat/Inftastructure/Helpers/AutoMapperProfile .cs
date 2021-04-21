using AutoMapper;
using WebChat.Models;
using WebChat.Models.Autorization;

namespace WebChat.Inftastructure.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserModel>();
            CreateMap<RegisterModel, ApplicationUser>();
            CreateMap<UpdateModel, ApplicationUser>();
        }
    }
}
