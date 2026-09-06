using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class NotificationItemDTO
    {
        public string title { get; set; } = null!;
        public string body { get; set; } = null!;
        public DateTime createdDate { get; set; }
        public int notificationTypeId { get; set; }
        public bool isRead { get; set; }
    }
}
