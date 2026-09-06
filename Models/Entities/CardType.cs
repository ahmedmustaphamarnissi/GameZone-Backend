using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class CardType
{
    public int CardTypeId { get; set; }

    public string CardTypeName { get; set; } = null!;

    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
}
