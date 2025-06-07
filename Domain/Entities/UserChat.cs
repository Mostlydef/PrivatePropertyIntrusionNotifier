using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserChat
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }

        public ICollection<UserDevices> UserDevices { get; set; } = new List<UserDevices>();
    }
}
