using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserDevicesRepository
    {
        Task<UserDevices?> GetByMacAdress(string macAdress);
        Task<bool> DeleteByMacAdress(string macAdress);
        Task<bool> AddAsync(UserDevices userDevices);
        Task<List<UserDevices>?> GetAllByChatIdAsync(long chatId);
    }
}
