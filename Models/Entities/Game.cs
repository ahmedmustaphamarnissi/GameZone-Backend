using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Game
{
    public int GameId { get; set; }

    public string GameName { get; set; } = null!;

    public string GameDescription { get; set; } = null!;

    public decimal InitialPrice { get; set; }

    public int Discount { get; set; }

    public DateTime AdditionDate { get; set; }

    public int AppropriateAge { get; set; }

    public double Size { get; set; }

    public DateTime LastUpdate { get; set; }

    public int StatusId { get; set; }

    public int CompanyId { get; set; }

    public string GameVersion { get; set; } = null!;

    public int TypeId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<EventGame> EventGames { get; set; } = new List<EventGame>();

    public virtual ICollection<GameDevice> GameDevices { get; set; } = new List<GameDevice>();

    public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();

    public virtual ICollection<GameLanguage> GameLanguages { get; set; } = new List<GameLanguage>();

    public virtual ICollection<GamesFeature> GamesFeatures { get; set; } = new List<GamesFeature>();

    public virtual ICollection<GamesVidsAndPicture> GamesVidsAndPictures { get; set; } = new List<GamesVidsAndPicture>();

    public virtual ICollection<InstalledGame> InstalledGames { get; set; } = new List<InstalledGame>();

    public virtual ICollection<PurchasedGame> PurchasedGames { get; set; } = new List<PurchasedGame>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual GamesStatus Status { get; set; } = null!;

    public virtual ICollection<SystemRequirement> SystemRequirements { get; set; } = new List<SystemRequirement>();

    public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
}
