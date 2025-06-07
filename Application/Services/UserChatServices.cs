using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
namespace Application.Services
{
    public class UserChatServices : IUserChatService
    {
        private readonly IUserChatRepository _repository;
        private readonly IMapper _mapper;

        public UserChatServices(IUserChatRepository userChatRepository, IMapper mapper)
        {
            _repository = userChatRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(UserChatDTO dto)
        {
            return await _repository.AddAsync(_mapper.Map<UserChat>(dto));
        }

        public async Task<bool> DeleteChatAsync(long chatId)
        {
            return await _repository.DeleteByChatIdAsync(chatId);
        }

        public async Task<UserChatDTO?> GetChatAsync(long chatId)
        {
            var userChat = await _repository.GetByChatId(chatId);
            if (userChat != null)
            {
                return _mapper.Map<UserChatDTO>(userChat);
            }
            return null;
        }
    }
}
