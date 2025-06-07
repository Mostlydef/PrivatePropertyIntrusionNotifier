using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.DTOs
{
    internal class DeleteDeviceRequest
    {
        public string MacAdress { get; set; } = string.Empty;
    }
}
