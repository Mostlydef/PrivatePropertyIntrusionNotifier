using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserDevicesService
    {
        Task<UserDevicesDTO?> GetAsync(string macAdress);
        Task<bool> AddAsync(UserDevicesDTO dto);
        Task<bool> DeleteAsync(string macAdress);
        Task<List<UserDevicesDTO>?> GetAllAsync(long chatId);
    }
}
