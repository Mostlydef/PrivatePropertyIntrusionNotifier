using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserChatRepository : IUserChatRepository
    {
        private readonly AppDbContext _context;

        public UserChatRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<bool> AddAsync(UserChat userChatId)
        {
            await _context.AddAsync(userChatId);
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<bool> DeleteByChatIdAsync(long chatId)
        {
            var chat = await _context.UserChats.FirstOrDefaultAsync(u => u.ChatId == chatId);
            if(chat != null)
            {
                _context.UserChats.Remove(chat);
                var result = _context.SaveChanges();
                return result > 0;
            }
            return false;
        }

        public async Task<UserChat?> GetByChatId(long chatId)
        {
            return await _context.UserChats.FirstOrDefaultAsync(u => u.ChatId == chatId);
        }
    }
}
