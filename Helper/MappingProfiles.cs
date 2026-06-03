using alposim.DTO;
using alposim.Models;
using AutoMapper;

namespace alposim.Helper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ProductRequestDto, Product>();
    }
}  