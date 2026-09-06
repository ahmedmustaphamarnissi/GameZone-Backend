using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Country
{
    public int CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public string PhoneCode { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
