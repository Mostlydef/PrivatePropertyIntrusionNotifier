using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class UserDevicesServices : IUserDevicesService
    {
        private readonly IUserDevicesRepository _repository;
        private readonly IMapper _mapper;

        public UserDevicesServices(IUserDevicesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;   
        }

        public async Task<bool> AddAsync(UserDevicesDTO dto)
        {
            return await _repository.AddAsync(_mapper.Map<UserDevices>(dto));
        }

        public async Task<bool> DeleteAsync(string macAdress)
        {
            return await _repository.DeleteByMacAdress(macAdress);
        }

        public async Task<UserDevicesDTO?> GetAsync(string macAdress)
        {
            var userDevices = await _repository.GetByMacAdress(macAdress);
            if (userDevices != null)
            {
                return _mapper.Map<UserDevicesDTO>(userDevices);
            }
            return null;
        }

        public async Task<List<UserDevicesDTO>?> GetAllAsync(long chatId)
        {
            var devices = await _repository.GetAllByChatIdAsync(chatId);
            if(devices != null)
            {
                return _mapper.Map<List<UserDevicesDTO>>(devices);
            }
            return null;
        }
    }
}
