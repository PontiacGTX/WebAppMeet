using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Validations
{
    public class PasswordNumberValidationAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                string val = value.ToString();
                Regex reg = new Regex(".*[0-9].*");
                return reg.IsMatch(val) ? null : new ValidationResult($"{validationContext.MemberName} must contain at least one number");
            }

            return new ValidationResult($"{validationContext.MemberName} must contain at least one number");
        }
    }
}
