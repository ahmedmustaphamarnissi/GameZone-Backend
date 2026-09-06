using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class EventDTO
{
    public int eventId { get; set; }
    public string eventName { get; set; } = null!;
    public string? eventDescription { get; set; }
    public DateTime endDate { get; set; }
    public string? imagePath { get; set; }
    public List<GameDTO>? games { get; set; }
}
