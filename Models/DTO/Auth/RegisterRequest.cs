using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Validation;

namespace Models.DTO.Auth;

public class RegisterRequest
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 chars, include uppercase, lowercase, number, and special character.")]
    public string Password { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[a-zA-Z]{2,50}$",
        ErrorMessage = "First name must be letters only, 2-50 characters.")]
    public string FirstName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[a-zA-Z]{2,50}$",
        ErrorMessage = "Last name must be letters only, 2-50 characters.")]
    public string LastName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._]{3,20}$",
        ErrorMessage = "Username must be 3-20 characters, letters, numbers, dots, or underscores.")]
    public string UserName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[A-Z]{2}$",
    ErrorMessage = "Country code must be 2 uppercase letters (e.g. TN, FR, US).")]
    public string CountryCode { get; set; } = null!;

    [Required]
    [MinimumAge(7)]
    public DateTime DateOfBirth { get; set; }
}