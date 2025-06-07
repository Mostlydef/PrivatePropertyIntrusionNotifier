using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserDevices
    {
        public Guid Id { get; set; }
        public Guid UserChatId { get; set; }
        public string MacAdress { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public UserChat UserChat { get; set; } = null!;
    }
}
