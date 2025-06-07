namespace TelegramBot.Services
{
    internal class PollingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Начало опроса сервиса.");

            await DoWork(cancellationToken);
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var receiver = scope.ServiceProvider.GetRequiredService<ReceiverService>();

                    await receiver.ReceiveAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Опрос прерван с ошибкой: {Exception}", ex);

                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
            }
        }
    }
}
