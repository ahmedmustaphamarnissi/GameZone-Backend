using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO;

public class ChangePasswordDTO
{
    [Required]
    public string oldPassword { get; set; } = null!;
    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 chars, include uppercase, lowercase, number, and special character.")]
    public string newPassword { get; set; } = null!;
}
