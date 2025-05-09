using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Bookline.Shared.Validations
{
    public class ValidDateFormatAttribute : ValidationAttribute
    {
        private readonly string _format;

        public ValidDateFormatAttribute(string format = "yyyy-MM-dd")
        {
            _format = format;
            ErrorMessage = $"Date value must be follow this format: {_format}.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string dateString = value as string;

            if (string.IsNullOrWhiteSpace(dateString))
            {
                return new ValidationResult("Date is required.");
            }

            bool isValid = DateTime.TryParseExact(dateString, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _);

            return isValid ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}