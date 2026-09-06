using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class UpdateUserDTO
{
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string firstName { get; set; } = null!;

    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string lastName { get; set; } = null!;

    [Required]
    public DateTime dateOfBirth { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(10)]
    public string countryCode { get; set; } = null!;

    [Phone]
    public string? phoneNumber { get; set; }

    public string? gender { get; set; }

    [MaxLength(500)]
    public string? profilePicture { get; set; }

    [MaxLength(500)]
    public string? bio { get; set; }
}
