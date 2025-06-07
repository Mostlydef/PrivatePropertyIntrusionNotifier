using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramBot.Interfaces;


namespace TelegramBot.Services
{
    internal class ReceiverService : IReceiverService
    {

        private readonly ITelegramBotClient _botClient;
        private readonly UpdateHandler _updateHandler;
        private readonly ILogger _logger;

        public ReceiverService(ITelegramBotClient botClient, UpdateHandler updateHandler, ILogger<ReceiverService> logger)
        {
            _botClient = botClient;
            _updateHandler = updateHandler;
            _logger = logger;
        }

        public async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            var receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>(),
                DropPendingUpdates = true,
            };

            var me = await _botClient.GetMe(cancellationToken);
            _logger.LogInformation("Начало получения обновлений с {BotName}", me.Username ?? "Мой бот");

            await _botClient.ReceiveAsync(updateHandler: _updateHandler, receiverOptions: receiverOptions, cancellationToken: cancellationToken);
        }
    }
}

