using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Validations
{
    public class PasswordLowerCaseValidationAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                string val = value.ToString();
                Regex reg = new Regex("/^[a-zÀ-ÖØ-öø-ÿ]+$/");
                return reg.IsMatch(val) ? null : new ValidationResult($"{validationContext.MemberName} must contain at least one lower case char");
            }

            return new ValidationResult($"{validationContext.MemberName} must contain at least one lower case char");
        }
    }
}
