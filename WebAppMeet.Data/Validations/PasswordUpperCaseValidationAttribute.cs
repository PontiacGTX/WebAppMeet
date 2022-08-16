using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebAppMeet.Data.Validations
{
    internal class PasswordUpperCaseValidationAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            if (value is not null)
            {
                string val = value.ToString();
                Regex reg = new Regex("/^[A-ZÀ-ÖØ-öø-ÿ]+$/");
                return reg.IsMatch(val) ? null : new ValidationResult($"{validationContext.MemberName} must contain at least one upper case char");
            }

            return new ValidationResult($"{validationContext.MemberName} must contain at least one upper case char");
        }
    }
}
