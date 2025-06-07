using TelegramBot.DTOs;

namespace TelegramBot.Interfaces
{
    internal interface IHttpSendlerService
    {
        Task<string> SendAddDeviceAsync(long chatId, string macAdress, string location);
        Task<List<UserDevicesDTO>?> SendGetAllRequest(long chatId);
        Task<string> SendDeleteChatRequest(long chatId);
        Task<string> SendDeleteDeviceRequest(string macAdress);
    }
}
