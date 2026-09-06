using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Data.enums;

namespace Models.DTO;

using System.ComponentModel.DataAnnotations;

public class PurchaseGameDTO : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "A valid game ID is required.")]
    public int gameId { get; set; }


    [Required(ErrorMessage = "Card holder name is required.")]
    [MinLength(4, ErrorMessage = "Card holder name must contain at least 4 characters.")]
    [MaxLength(100, ErrorMessage = "Card holder name cannot exceed 100 characters.")]
    [RegularExpression(
        @"^[a-zA-ZÀ-ÿ\s.'-]+$",
        ErrorMessage = "Card holder name contains invalid characters."
    )]
    public string cardHolder { get; set; } = null!;


    [Required(ErrorMessage = "Card number is required.")]
    [RegularExpression(
        @"^\d{16}$",
        ErrorMessage = "Card number must contain exactly 16 digits."
    )]
    public string cardNumber { get; set; } = null!;


    [Required(ErrorMessage = "Expiry date is required.")]
    public DateTime expiryDate { get; set; }

    [EnumDataType(typeof(enCardType), ErrorMessage = "Invalid card type.")]
    public enCardType cardType { get; set; }


    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        // Card is valid until the end of its expiry month
        var currentMonth = new DateTime(
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Month,
            1
        );

        var expiryMonth = new DateTime(
            expiryDate.Year,
            expiryDate.Month,
            1
        );

        if (expiryMonth < currentMonth)
        {
            yield return new ValidationResult(
                "The card has expired.",
                new[] { nameof(expiryDate) }
            );
        }

        // Optional: prevent unreasonable future dates
        if (expiryDate.Year > DateTime.UtcNow.Year + 30)
        {
            yield return new ValidationResult(
                "The expiry date is invalid.",
                new[] { nameof(expiryDate) }
            );
        }
    }
}
