﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IUserChatService
    {
        Task<bool> AddAsync(UserChatDTO dto);
        Task<bool> DeleteChatAsync(long chatId);
        Task<UserChatDTO?> GetChatAsync(long chatId);
    }
}
