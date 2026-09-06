using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.DTO;

namespace Data_Access_Layer;

public class EventsData : BaseData
{
    public EventsData(IConfiguration config) : base(config)
    {
    }

    public async Task<EventDTO?> GetEventDetailsAsync()
    {
        using var context = CreateDbContext();
        var now = DateTime.UtcNow;
        var eventDetails = await context.Events.AsNoTracking()
            .Where(e => e.IsActive &&
                e.StartDate <= now &&
                e.EndDate > now).Select(res => new
                {
                    EventId = res.EventId,
                    EventName = res.EventName,
                    EventDescription = res.EventDescription,
                    endDate = res.EndDate,
                    imagePath = res.BannerImagePath
                }).FirstOrDefaultAsync();
        if (eventDetails == null)
            return null;
        var eventGames = await context.EventGames.AsNoTracking()
            .Where(eg=> eg.EventId == eventDetails.EventId)
            .Select(g => new GameDTO
            {
                gameId = g.GameId,
                gameName = g.Game.GameName,
                price = g.Game.InitialPrice,
                discount = g.Game.Discount,
                gameCover = g.Game.GamesVidsAndPictures
                        .Where(c => c.IsPrimary == true && c.Type == false)
                        .Select(c => c.Path)
                        .FirstOrDefault(),
                companyName = g.Game.Company.CompanyName,
                companyCover = g.Game.Company.LogoPath,
                additionDate = g.Game.AdditionDate,
                status = g.Game.Status.StatusName,
                categories = g.Game.GameGenres.Select(gg => gg.Genre.TypeName).ToList()
            })
            .ToListAsync();

        return new EventDTO
        {
            eventId = eventDetails.EventId,
            endDate = eventDetails.endDate,
            eventName = eventDetails.EventName,
            eventDescription = eventDetails.EventDescription,
            imagePath = eventDetails.imagePath,
            games = eventGames
        };
    }
}
