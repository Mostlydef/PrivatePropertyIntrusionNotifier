using Infrastructure.Persistence;
using Microsoft.Extensions.FileProviders;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;
using Application.Interfaces;
using Application.Services;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.WithOrigins("http://192.168.1.195")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader();
                    });
            });
            builder.Services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient) =>
            {
                var configSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
                var botConfig = configSection.Get<BotConfiguration>();

                if (botConfig is null || string.IsNullOrEmpty(botConfig.TelegramBotToken))
                    throw new InvalidOperationException("Telegram bot token is missing in configuration");

                var options = new TelegramBotClientOptions(botConfig.TelegramBotToken);
                return new TelegramBotClient(options, httpClient);
            });
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var configSection = builder.Configuration.GetSection(DataBaseConfiguration.Configuration);
                var databaseConfig = configSection.Get<DataBaseConfiguration>();

                if (databaseConfig is null || string.IsNullOrEmpty(databaseConfig.DefaultConnection))
                    throw new InvalidOperationException("Database missing in configuration");

                options.UseNpgsql(databaseConfig.DefaultConnection);
            });
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddScoped<IUserChatRepository, UserChatRepository>();
            builder.Services.AddScoped<IUserDevicesRepository, UserDeviceRepository>();
            builder.Services.AddScoped<IUserChatService, UserChatServices>();
            builder.Services.AddScoped<IUserDevicesService, UserDevicesServices>();
            builder.Services.AddLogging();
            var app = builder.Build();
            app.UseCors("AllowAll");

            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllers();

            app.Run();

        }

        public class BotConfiguration
        {
            public static readonly string Configuration = "Telegram";
            public string TelegramBotToken { get; set; } = String.Empty;
        }

        public class DataBaseConfiguration
        {
            public static readonly string Configuration = "PostrgeSQL";
            public string DefaultConnection { get; set; } = String.Empty;
        }
    }
}
