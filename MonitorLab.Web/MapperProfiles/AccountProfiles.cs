using AutoMapper;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Web.Models.AccountViewModels;

namespace MonitorLab.Web.MapperProfiles
{
    public class AccountProfiles : Profile
    {
        public AccountProfiles()
        {
            CreateMap<LoginViewModel, LoginDTO>();
            CreateMap<LoginDTO, LoginViewModel>();
        }
    }
}
