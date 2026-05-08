using AutoMapper;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Data.Models;
using MonitorLab.Web.Models.MonitorViewModels;
using Monitor = MonitorLab.Data.Models.Monitor;

namespace MonitorLab.Web.MapperProfiles
{
    public class MonitorProfiles : Profile
    {
        public MonitorProfiles()
        {
            CreateMap<Monitor, MonitorCardDto>();
            CreateMap<MonitorCardDto, MonitorCardViewModel>();
            CreateMap<MonitorCatalogDTO, MonitorCatalogViewModel>();
        }
    }
}
