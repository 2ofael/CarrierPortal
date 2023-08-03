using System;
using System.ComponentModel.DataAnnotations;
namespace CarrierPortal.CoustomValidations
{
  

    public class DateOfBirthAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                if (dateOfBirth >= DateTime.Today)
                {
                    return new ValidationResult("Date of Birth must be earlier than the current date.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
