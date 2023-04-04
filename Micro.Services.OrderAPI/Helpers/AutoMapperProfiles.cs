using AutoMapper;
using Micro.Services.OrderAPI.Messages;
using Micro.Services.OrderAPI.Models;

namespace Micro.Services.OrderAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CheckoutHeaderDto, OrderHeader>();
        }
    }
}
