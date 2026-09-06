using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class UserProfileSettingsDTO
{
    public string firstName { get; set; } = null!;

    public string lastName { get; set; } = null!;

    public DateTime dateOfBirth { get; set; }

    public string countryName { get; set; } = null!;
    public string countryCode { get; set; } = null!;
    public string email { get; set; } = null!;

    public string? phoneNumber { get; set; }

    public string? gender { get; set; }

    public string userName { get; set; } = null!;

    public string? profilePicture { get; set; }

    public string? bio { get; set; }
}
