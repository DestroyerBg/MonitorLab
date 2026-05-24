using AutoMapper;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Data.Models;
using MonitorLab.Web.Models.CompareViewModels;
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
            CreateMap<Monitor, MonitorDetailsDTO>()
                    .ForMember(d => d.Ports, opt => opt.Ignore());
            CreateMap<MonitorDetailsDTO, MonitorDetailsViewModel>();
            CreateMap<MonitorPortDetailsDTO, MonitorPortDetailsViewModel>();
            CreateMap<Port, MonitorPortDetailsDTO>()
                .ForMember(dest => dest.Count, opt => opt.Ignore());
            CreateMap<Monitor, MonitorComparisonCardDTO>()
                .ForMember(dest => dest.Ports, opt => opt.Ignore())
                .ForMember(dest => dest.GamingScore, opt => opt.Ignore())
                .ForMember(dest => dest.OfficeScore, opt => opt.Ignore())
                .ForMember(dest => dest.MultimediaScore, opt => opt.Ignore())
                .ForMember(dest => dest.DesignScore, opt => opt.Ignore());
            CreateMap<MonitorComparisonCardDTO, MonitorComparisonCardViewModel>();
            CreateMap<ComparisonRecommendationDTO, ComparisonRecommendationViewModel>();
            CreateMap<CompareDTO, CompareViewModel>();
            CreateMap<MonitorCreateDTO, MonitorCreateViewModel>()
                .ForMember(src => src.Resolutions, opt => opt.Ignore())
                .ForMember(src => src.PanelTypes, opt => opt.Ignore())
                .ForMember(src => src.ImageFile, opt => opt.Ignore());
            CreateMap<MonitorCreateViewModel, MonitorCreateDTO>();
            CreateMap<MonitorCreateDTO, Monitor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MonitorPorts, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
             
        }
    }
}
