using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class PurchasedGame
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public decimal PurchasedPrice { get; set; }

    public DateTime Date { get; set; }

    public int? PaymentMethodId { get; set; }

    public bool IsFavoriteGame { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual User User { get; set; } = null!;
}
