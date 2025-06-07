using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Application.Interfaces;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<ImageController> _logger;
        private readonly IUserDevicesService _userDevicesService;
        private const string _messageText = "\n❗ Проникновение!\n\n🏠 Объект: {0}\n🕒 Время: {1}";

        public ImageController(ITelegramBotClient botClient, ILogger<ImageController> logger, IUserDevicesService userDevices)
        {
            _botClient = botClient;
            _logger = logger;
            _userDevicesService = userDevices;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, [FromForm] string macAddress)
        {
            var device = await _userDevicesService.GetAsync(macAddress);
            if(image == null || image.Length == 0 || String.IsNullOrEmpty(macAddress))
            {
                return BadRequest("No file uploaded.");
            } else if(device == null || device.ChatId == 0)
            {
                return Forbid("Не зарегистрированная камера.");
            }
            await using var stream = image.OpenReadStream();
            await _botClient.SendPhoto(
            chatId: device.ChatId,
            photo: new InputFileStream(stream),
            caption: String.Format(_messageText, device.Location, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            return Ok(new { message = "Отправлено!" });
        }
    }
}
