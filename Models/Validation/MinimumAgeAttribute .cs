using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Validation;

public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minAge;

    public MinimumAgeAttribute(int minAge)
    {
        _minAge = minAge;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is DateTime dob)
        {
            var age = DateTime.Today.Year - dob.Year;
            if (dob.Date > DateTime.Today.AddYears(-age)) age--;

            if (age >= _minAge)
                return ValidationResult.Success;

            return new ValidationResult($"You must be at least {_minAge} years old.");
        }
        return new ValidationResult("Invalid date of birth.");
    }
}
