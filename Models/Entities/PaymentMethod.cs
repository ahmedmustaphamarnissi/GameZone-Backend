using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public int PersonId { get; set; }

    public string CardHolder { get; set; } = null!;

    public string CardNumber { get; set; } = null!;

    public DateOnly ExpiryDate { get; set; }

    public bool IsDefault { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CardTypeId { get; set; }

    public virtual CardType? CardType { get; set; }

    public virtual Person Person { get; set; } = null!;

    public virtual ICollection<PurchasedGame> PurchasedGames { get; set; } = new List<PurchasedGame>();
}
