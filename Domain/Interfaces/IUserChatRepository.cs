using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserChatRepository
    {
        Task<bool> AddAsync(UserChat userChatId);
        Task<bool> DeleteByChatIdAsync(long chatId);
        Task<UserChat?> GetByChatId(long chatId);
    }
}
