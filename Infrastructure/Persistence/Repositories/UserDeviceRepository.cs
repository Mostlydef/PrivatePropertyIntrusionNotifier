using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class UserDeviceRepository : IUserDevicesRepository
    {
        private readonly AppDbContext _context;

        public UserDeviceRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<bool> AddAsync(UserDevices userDevices)
        {
            await _context.AddAsync(userDevices);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteByMacAdress(string macAdress)
        {
            var device = await _context.UserDevices.FirstOrDefaultAsync(u => u.MacAdress == macAdress);
            if(device != null)
            {
                _context.UserDevices.Remove(device);
                var result = _context.SaveChanges();
                return result>0;
            }
            return false;
        }

        public async Task<UserDevices?> GetByMacAdress(string macAdress)
        {
            return await _context.UserDevices.Include(u => u.UserChat).FirstOrDefaultAsync(u => u.MacAdress == macAdress);
        }

        public async Task<List<UserDevices>?> GetAllByChatIdAsync(long chatId)
        {
            return await _context.UserDevices.Include(u => u.UserChat).Where(u => u.UserChat.ChatId == chatId).ToListAsync();
        }
    }
}
