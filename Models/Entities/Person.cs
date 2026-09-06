using System;
using System.Collections.Generic;

namespace GameZoneBack.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public int CountryId { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Gender { get; set; }

    public int GradeId { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual Grade Grade { get; set; } = null!;

    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

    public virtual User? User { get; set; }
}
