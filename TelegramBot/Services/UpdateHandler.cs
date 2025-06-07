using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Interfaces;

namespace TelegramBot.Services
{

    public struct CameraParam
    {
        public string cameraMac;
        public string propertyType;

        public CameraParam()
        {
            cameraMac = string.Empty;
            propertyType = string.Empty;
        }
    }

    internal class UpdateHandler : IUpdateHandler
    {
        private readonly ILogger<UpdateHandler> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IHttpSendlerService _httpSendler;
        private bool _cameraReg;
        private bool _cameraTypePropertyReg;
        private bool _deleteDevice;
        private CameraParam _cameraParam;
        private Dictionary<long, LinkedList<int>> _messageForDelete;
        private const string _textCameraReg = "📷 Регистрация камеры.\nMAC-адрес камеры: {0}\nТип охраняемого объекта: {1}";

        public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IHttpSendlerService httpSendler)
        {
            _client = botClient;
            _logger = logger;
            _httpSendler = httpSendler;
            _cameraReg = false;
            _cameraTypePropertyReg = false;
            _deleteDevice = false;
            _messageForDelete = new Dictionary<long, LinkedList<int>>();
        }


        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        if (update.Message == null)
                            return;
                        await OnMessage(update.Message);
                        break;
                    case UpdateType.CallbackQuery:
                        if (update.CallbackQuery == null)
                            return;
                        await OnCallbackQuery(update.CallbackQuery);
                        break;

                    default:
                        await UnknownUpdateHandlerAsync(update, cancellationToken);
                        break;
                }
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                if ((ex.Message.Contains("Forbidden") || ex.ErrorCode == 403) && update.Message != null)
                {
                    await _httpSendler.SendDeleteChatRequest(update.Message.Chat.Id);
                }
            }

        }

        private async Task OnMessage(Message message)
        {
            if (String.IsNullOrEmpty(message.Text))
                return;
            if (message.Text == "/start")
            {
                _cameraParam.cameraMac = string.Empty;
                _cameraParam.propertyType = string.Empty;
                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Для привязки камеры к чату, требуется ввести MAC-адрес камеры и тип охраняемого объекта!",
                    replyMarkup: ReplyKeyboard(message)
                    );
                var messageEdit = await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: String.Format(_textCameraReg, _cameraParam.cameraMac, _cameraParam.propertyType),
                    replyMarkup: SendInlineKeyboard(message)
                    );
            }
            else if (_cameraReg == true)
            {
                if (Regex.IsMatch(message.Text, @"\b([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}\b"))
                {
                    _cameraParam.cameraMac = message.Text;
                    await _client.SendMessage(
                        chatId: message.Chat.Id,
                        text: String.Format(_textCameraReg, _cameraParam.cameraMac, _cameraParam.propertyType),
                        replyMarkup: SendInlineKeyboard(message)
                    );
                    _messageForDelete[message.Chat.Id].AddLast(message.Id);
                    await _client.DeleteMessages(message.Chat.Id, _messageForDelete[message.Chat.Id]);
                    _cameraReg = false;
                }
                else
                {
                    var lastMessage = await _client.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Введенное выражение не является MAC-адресом!"
                    );
                    _messageForDelete[message.Chat.Id].AddLast(message.Id);
                    _messageForDelete[message.Chat.Id].AddLast(lastMessage.Id);
                    await Task.Delay(1000);
                    await _client.DeleteMessages(message.Chat.Id, _messageForDelete[message.Chat.Id]);
                }
            }
            else if (_cameraTypePropertyReg == true)
            {
                _cameraParam.propertyType = message.Text;
                await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: String.Format(_textCameraReg, _cameraParam.cameraMac, _cameraParam.propertyType),
                    replyMarkup: SendInlineKeyboard(message)
                );
                _messageForDelete[message.Chat.Id].AddLast(message.Id);
                await _client.DeleteMessages(message.Chat.Id, _messageForDelete[message.Chat.Id]);
                _cameraTypePropertyReg = false;
            }
            else if (message.Text == "📷 Добавить камеру")
            {
                _cameraParam.cameraMac = string.Empty;
                _cameraParam.propertyType = string.Empty;
                var messageEdit = await _client.SendMessage(
                    chatId: message.Chat.Id,
                    text: String.Format(_textCameraReg, _cameraParam.cameraMac, _cameraParam.propertyType),
                    replyMarkup: SendInlineKeyboard(message)
                    );
            }
            else if (message.Text == "🎦 Добавленные камеры")
            {
                var devices = _httpSendler.SendGetAllRequest(message.Chat.Id);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Добавленные камеры:");
                if (devices.Result != null)
                {
                    foreach (var device in devices.Result)
                    {
                        builder.AppendLine($"MAC-адресс: {device.MacAdress} Объект: {device.Location}");
                    }
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("❌ Удалить камеру", "deleteDevice")
                        }
                    });
                    await _client.SendMessage(
                        chatId: message.Chat.Id,
                        text: builder.ToString(),
                        replyMarkup: inlineKeyboard);
                }
                else
                {
                    await _client.SendMessage(
                        chatId: message.Chat.Id,
                        text: "У Вас нет добавленных камер.");
                }
            }
            else if (_deleteDevice == true)
            {
                if (Regex.IsMatch(message.Text, @"\b([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}\b"))
                {
                    var response = _httpSendler.SendDeleteDeviceRequest(message.Text);
                    if (response.Result != null)
                    {
                        await _client.SendMessage(
                            chatId: message.Chat.Id,
                            text: response.Result);
                    }
                }
                else
                {
                    await _client.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Введенное выражение не является MAC-адресом!"
                    );
                }
            }
        }

        private InlineKeyboardMarkup SendInlineKeyboard(Message message)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📷 Добавить камеру", "cameraReg"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🏠 Указать объект охраны", "propertyType")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✔️ Готово", "check")
                }
            });
            return inlineKeyboard;
        }

        private async Task OnCallbackQuery(CallbackQuery callbackQuery)
        {
            switch (callbackQuery.Data)
            {
                case "cameraReg":
                    if (callbackQuery.Message != null)
                    {
                        var lastMessage = await _client.SendMessage(
                            chatId: callbackQuery.Message.Chat.Id,
                            text: "Введите MAC-адрес камеры.",
                            cancellationToken: CancellationToken.None);
                        _cameraReg = true;
                        _cameraTypePropertyReg = false;
                        if (_messageForDelete.ContainsKey(callbackQuery.Message.Chat.Id))
                        {
                            _messageForDelete[callbackQuery.Message.Chat.Id].AddLast(lastMessage.Id);
                        }
                        else
                        {
                            _messageForDelete.Add(callbackQuery.Message.Chat.Id, new LinkedList<int>());
                            _messageForDelete[callbackQuery.Message.Chat.Id].AddLast(lastMessage.Id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Регистрация камеры. callbackQuery.Message = null");
                    }
                    break;
                case "propertyType":
                    if (callbackQuery.Message != null)
                    {
                        var lastMessage = await _client.SendMessage(
                            chatId: callbackQuery.Message.Chat.Id,
                            text: "Введите название объекта охраны.",
                            cancellationToken: CancellationToken.None);
                        _cameraReg = false;
                        _cameraTypePropertyReg = true;
                        if (_messageForDelete.ContainsKey(callbackQuery.Message.Chat.Id))
                        {
                            _messageForDelete[callbackQuery.Message.Chat.Id].AddLast(lastMessage.Id);
                        }
                        else
                        {
                            _messageForDelete.Add(callbackQuery.Message.Chat.Id, new LinkedList<int>());
                            _messageForDelete[callbackQuery.Message.Chat.Id].AddLast(lastMessage.Id);
                        }
                    }
                    break;
                case "check":
                    if (callbackQuery.Message != null && !String.IsNullOrEmpty(callbackQuery.Message.Text))
                    {
                        if (String.IsNullOrEmpty(_cameraParam.cameraMac) || String.IsNullOrEmpty(_cameraParam.propertyType))
                        {
                            await _client.SendMessage(
                                    chatId: callbackQuery.Message.Chat.Id,
                                    text: "Введите все поля.");
                            return;
                        }
                        if (callbackQuery.Message.Text.Contains(_cameraParam.cameraMac) && (_cameraParam.cameraMac != "📷 Регистрация камеры.\nMAC-адрес камеры: " ||
                            _cameraParam.cameraMac != "Тип охраняемого объекта: " || _cameraParam.propertyType != "📷 Регистрация камеры.\nMAC-адрес камеры: )" ||
                            _cameraParam.propertyType != "Тип охраняемого объекта: ")
                            && callbackQuery.Message.Text.Contains(_cameraParam.cameraMac))
                        {
                            var split = callbackQuery.Message.Text.Split(separator: new string[]
                               {
                                "📷 Регистрация камеры.\nMAC-адрес камеры: ", "Тип охраняемого объекта: "
                               }, options: StringSplitOptions.None);
                            var result = await _httpSendler.SendAddDeviceAsync(callbackQuery.Message.Chat.Id, split[1].Substring(0, 17), split[2]);
                            await _client.SendMessage(
                                chatId: callbackQuery.Message.Chat.Id,
                                text: result);
                        }
                        else
                        {
                            await _client.SendMessage(
                                    chatId: callbackQuery.Message.Chat.Id,
                                    text: "Введите все поля.");
                        }

                    }
                    break;
                case "deleteDevice":
                    if (callbackQuery.Message != null && !string.IsNullOrEmpty(callbackQuery.Message.Text))
                    {
                        if (Regex.IsMatch(callbackQuery.Message.Text, @"\b([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}\b"))
                        {
                            await _client.SendMessage(
                            chatId: callbackQuery.Message.Chat.Id,
                            text: "Введиите MAC-адресс камеры:");
                            _deleteDevice = true;
                        }
                    }
                    break;

            }

        }

        private ReplyKeyboardMarkup ReplyKeyboard(Message message)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new [] {new KeyboardButton("📷 Добавить камеру"), new KeyboardButton("🎦 Добавленные камеры") }
            })
            { ResizeKeyboard = true };
            return keyboard;
        }

        public async Task SendPhotoFromApiAsync(Stream stream, string fileName, long chatId)
        {
            await _client.SendPhoto(
                chatId: chatId,
                photo: new InputFileStream(stream, fileName),
                caption: "Изображение получено с внешнего API");
        }

        private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }
    }
}
