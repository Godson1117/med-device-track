using AutoMapper;
using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Domain.Entities;

namespace MedicalDeviceTracking.API.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Gateway mappings
        CreateMap<Gateway, GatewayDto>()
            .ForMember(dest => dest.Advertisements, opt => opt.MapFrom(src => src.Advertisements));

        CreateMap<CreateGatewayDto, Gateway>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Advertisements, opt => opt.Ignore());

        // SensorAdvertisement mappings
        CreateMap<SensorAdvertisement, SensorAdvertisementDto>();

        CreateMap<CreateSensorAdvertisementDto, SensorAdvertisement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GatewayId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Gateway, opt => opt.Ignore());
    }
}
