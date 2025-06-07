using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.DTOs
{
    internal class AddDeviceRequest
    {
        public long ChatId { get; set; }
        public string MacAdress { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
