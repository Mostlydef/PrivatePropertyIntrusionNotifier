using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Telegram.Bot.Types;
using Application.DTOs.Requests;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : Controller
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly IUserChatService _userChatService;
        private readonly IUserDevicesService _userDevicesService;

        public DeviceController(ILogger<DeviceController> logger, IUserChatService userChatService, IUserDevicesService userDevicesService)
        {
            _logger = logger;
            _userChatService = userChatService;
            _userDevicesService = userDevicesService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddDevice([FromBody] AddDeviceRequest request)
        {
            if (request.ChatId == 0 || String.IsNullOrEmpty(request.MacAdress) || String.IsNullOrEmpty(request.Location))
            {
                return BadRequest($"Что-то пошло не так:\nномер чата - {request.ChatId},\nMAC-адресс - {request.MacAdress},\nобъект - {request.Location}.");
            }
            var isExistChat = await _userChatService.GetChatAsync(request.ChatId);
            var isExistDevice = await _userDevicesService.GetAsync(request.MacAdress);
            if (isExistChat == null && isExistDevice == null)
            {
                var guid = Guid.NewGuid();
                await _userChatService.AddAsync(new UserChatDTO
                {
                    Id = guid,
                    ChatId = request.ChatId
                });

                await _userDevicesService.AddAsync(new UserDevicesDTO
                {
                    Id = Guid.NewGuid(),
                    ChatId = request.ChatId,
                    UserChatId = guid,
                    MacAdress = request.MacAdress,
                    Location = request.Location
                });
            } else if (isExistDevice == null)
            {
                var guid = _userChatService.GetChatAsync(request.ChatId);
                if (guid.Result != null)
                {
                    await _userDevicesService.AddAsync(new UserDevicesDTO
                    {
                        Id = Guid.NewGuid(),
                        ChatId = request.ChatId,
                        MacAdress = request.MacAdress,
                        UserChatId = guid.Result.Id,
                        Location = request.Location
                    });
                }
                else
                {
                    return NotFound($"Что-то пошло не так:\nномер чата - {request.ChatId},\nMAC-адресс - {request.MacAdress},\nобъект - {request.Location}.");
                }

            } else if (isExistChat != null && isExistDevice != null)
            {
                return Conflict("Данная камера уже добавлена.");
            }

            return Ok("Камера успешно добавлена!");
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<UserDevicesDTO>>> GetAllDevices([FromQuery] long chatId)
        {
            if (chatId == 0)
            {
                return BadRequest($"Что-то пошло не так:\nномер чата - {chatId}.");
            }
            var devices = await _userDevicesService.GetAllAsync(chatId);
            if (devices == null || !devices.Any())
            {
                return NotFound("Камеры не найдены.");
            }
            return Ok(devices);
        }

        [HttpDelete("DeleteDevice")]
        public async Task<IActionResult> DeleteDevice([FromBody] DeleteDeviceRequest request)
        {
            if(string.IsNullOrEmpty(request.MacAdress))
            {
                return BadRequest($"Что-то пошло не так:\nMac-адресс - {request.MacAdress}.");
            }
            var isExist = await _userDevicesService.GetAsync(request.MacAdress);
            if (isExist == null)
            {
                return NotFound("Данная камеры не существует или он была удалена");
            }
            await _userDevicesService.DeleteAsync(request.MacAdress);
            return Ok("Камера удалена успешно");
        }

        [HttpDelete("DeleteChat")]
        public async Task<IActionResult> DeleteChat([FromBody] DeleteChatRequest request)
        {
            if (request.ChatId == 0)
            {
                return BadRequest($"Что-то пошло не так:\nномер чата - {request.ChatId}.");
            }
            var isExist = await _userChatService.GetChatAsync(request.ChatId);
            if(isExist == null)
            {
                return NotFound("Данный чат не найден или удален.");
            }
            await _userChatService.DeleteChatAsync(request.ChatId);
            return Ok("Чат успешно удален.");
        }
    }
}
