using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class NotificationsResponseDTO
{
    public int totalNotifications { get; set; }
    public List<NotificationItemDTO> notifications { get; set; } = new();
}
