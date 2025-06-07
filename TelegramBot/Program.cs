using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Interfaces;
using TelegramBot.Services;

namespace TelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<BotConfiguration>(context.Configuration.GetSection(BotConfiguration.Configuration));
                    services.AddHttpClient("telegram_bot_client")
                    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                    {
                        BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                        var botToken = context.Configuration.GetSection(BotConfiguration.Configuration).Get<BotConfiguration>();
                        TelegramBotClientOptions options = new(botConfig.TelegramBotToken);
                        return new TelegramBotClient(options, httpClient);
                    });
                    services.Configure<ApiSettings>(context.Configuration.GetSection(ApiSettings.Configuration));
                    services.AddScoped<UpdateHandler>();
                    services.AddScoped<ReceiverService>();
                    services.AddHttpClient<HttpSendlerService>();
                    services.AddScoped<HttpSendlerService>();
                    services.AddScoped<IHttpSendlerService, HttpSendlerService>();
                    services.AddHostedService<PollingService>();
                    
                    services.AddLogging();
                })
                
                .Build();
            
            host.Run();
        }

        public class BotConfiguration
        {
            public static readonly string Configuration = "Telegram";
            public string TelegramBotToken { get; set; } = string.Empty;

        }

        public class ApiSettings
        {
            public static readonly string Configuration = "ApiSettings";
            public string Url { get; set; } = string.Empty;
        }
    }
}
