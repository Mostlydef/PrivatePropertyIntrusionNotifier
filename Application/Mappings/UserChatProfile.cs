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
    public class UserChatProfile : Profile
    {
        public UserChatProfile()
        {
            CreateMap<UserChat, UserChatDTO>();
            CreateMap<UserChatDTO, UserChat>();
        }

    }
}
