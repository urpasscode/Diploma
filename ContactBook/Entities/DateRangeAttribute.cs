using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebApplication1.CustomAttributes
{
    public class DateRangeAttribute : ValidationAttribute
    {
        public DateRangeAttribute(string minDate, string maxDate)
        {
            MinDate = DateOnly.Parse(minDate);
            MaxDate = DateOnly.Parse(maxDate);
        }

        public DateOnly MinDate { get; set; }
        public DateOnly MaxDate { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow null values if needed
            }

            if (value is string dateString)
            {
                if (DateOnly.TryParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
                {
                    if (dateValue >= MinDate && dateValue <= MaxDate)
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            else if (value is DateOnly dateValue)
            {
                if (dateValue >= MinDate && dateValue <= MaxDate)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(string.Format(ErrorMessage, MinDate.ToString("dd.MM.yyyy"), MaxDate.ToString("dd.MM.yyyy")));
        }
    }

    /*public class DateRangeAttribute : ValidationAttribute
    {
        public DateRangeAttribute(string minDate, string maxDate)
        {
            MinDate = DateOnly.Parse(minDate);
            MaxDate = DateOnly.Parse(maxDate);
        }
        public DateOnly MinDate { get; set; }
        public DateOnly MaxDate { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow null values if needed
            }

            DateOnly dateValue = (DateOnly)value;
            if (dateValue >= MinDate && dateValue <= MaxDate)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(string.Format(ErrorMessage, MinDate.ToString("dd.MM.yyyy"), MaxDate.ToString("dd.MM.yyyy")));
            }
        }
    }*/

    public class DateOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly dateOnly)
            {
                // Проверяем, что дата находится в диапазоне от 1950-01-01 до 2023-01-01
                if (dateOnly >= DateOnly.FromDateTime(DateTime.Parse("1950-01-01")) &&
                    dateOnly <= DateOnly.FromDateTime(DateTime.Parse("2023-01-01")))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult("Значение должно быть датой в формате yyyy-MM-dd");
        }
    }
}
