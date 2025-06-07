using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class UserDevicesProfile : Profile
    {
        public UserDevicesProfile()
        {
            CreateMap<UserDevices, UserDevicesDTO>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.UserChat.ChatId))
            .ForMember(dest => dest.UserChatId, opt => opt.MapFrom(src => src.UserChatId));

            CreateMap<UserDevicesDTO, UserDevices>()
                .ForMember(dest => dest.UserChatId, opt => opt.MapFrom(src => src.UserChatId))
                .ForMember(dest => dest.UserChat, opt => opt.Ignore()); 
        }
    }
}
