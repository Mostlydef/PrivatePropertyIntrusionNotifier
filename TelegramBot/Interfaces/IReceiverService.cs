
namespace TelegramBot.Interfaces
{
    internal interface IReceiverService
    {
        Task ReceiveAsync(CancellationToken cancellationToken);
    }
}
