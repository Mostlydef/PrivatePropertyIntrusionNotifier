using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserDevicesDTO
    {
        public Guid Id { get; set; }
        public Guid UserChatId { get; set; }
        public long ChatId { get; set; }
        public string? MacAdress { get; set; }
        public string? Location { get; set; }
    }
}
