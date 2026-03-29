using System.ComponentModel.DataAnnotations;
using static MonitorLab.Data.Common.ErrorMessages.Monitor;
namespace MonitorLab.Data.Attributes
{
    public class ContrastRatioAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string[] bothNumber = value?.ToString()?
                .Split(':', StringSplitOptions.RemoveEmptyEntries) ??
                                  Array.Empty<string>();

            if (bothNumber.Length == 0 || bothNumber.Length != 2)
            {
                return new ValidationResult(InvalidContrastRatio);
            }

            bool tryNum1 = double.TryParse(bothNumber[0], out double num1);
            bool tryNum2 = double.TryParse(bothNumber[1], out double num2);

            if ((!tryNum1 || !tryNum2) || (num1 < num2))
            {
                return new ValidationResult(InvalidContrastRatio);
            }

            return ValidationResult.Success;

        }

    }
}
