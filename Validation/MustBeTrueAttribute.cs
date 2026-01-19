using System.ComponentModel.DataAnnotations;

namespace EventEaseApp.Validation
{
    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is bool b && b;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"The {name} field must be accepted.";
        }
    }
}