using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TelegramBot.DTOs;
using TelegramBot.Interfaces;
using static TelegramBot.Program;

namespace TelegramBot.Services
{
    internal class HttpSendlerService : IHttpSendlerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpSendlerService> _logger;
        private readonly string _apiUrl;

        public HttpSendlerService(HttpClient httpClient, ILogger<HttpSendlerService> logger, IOptions<ApiSettings> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiUrl = options.Value.Url;

            _logger.LogInformation("Loaded ApiUrl from config: {ApiUrl}", _apiUrl);
        }

        public async Task<string> SendAddDeviceAsync(long chatId, string macAdress, string location)
        {
            var deviceRequest = new AddDeviceRequest
            {
                ChatId = chatId,
                MacAdress = macAdress,
                Location = location
            };
            var json = JsonSerializer.Serialize(deviceRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"{_apiUrl}/add", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SendDeleteChatRequest(long chatId)
        {
            var deleteChatRequest = new DeleteChatRequest
            {
                ChatId = chatId
            };
            var json = JsonSerializer.Serialize(deleteChatRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_apiUrl}/deletechat"),
                Content = content
            };

            HttpResponseMessage respons = await _httpClient.SendAsync(request);
            return await respons.Content.ReadAsStringAsync();
        }

        public async Task<string> SendDeleteDeviceRequest(string macAdress)
        {
            var deleteDeviceRequest = new DeleteDeviceRequest
            {
                MacAdress = macAdress
            };
            var json = JsonSerializer.Serialize(deleteDeviceRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_apiUrl}/deletedevice"),
                Content = content
            };

            HttpResponseMessage respons = await _httpClient.SendAsync(request);
            return await respons.Content.ReadAsStringAsync();
        }

        public async Task<List<UserDevicesDTO>?> SendGetAllRequest(long chatId)
        {
            string url = $"{_apiUrl}/getall?chatId={chatId}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string responsBody = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                return JsonSerializer.Deserialize<List<UserDevicesDTO>>(responsBody, options);
            }
            else
            {
                return null;
            }
        }
    }
}
