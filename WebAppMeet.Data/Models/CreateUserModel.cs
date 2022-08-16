using SharedProject.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data.Validations;

namespace WebAppMeet.Data.Models
{
    public class CreateUserModel
    {
        [Required(ErrorMessage ="Email is required, please write a valid email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required, please write a valid passsword")]
        [PasswordSpecialCharValidation]
        [PasswordUpperCaseValidation]
        [PasswordLowerCaseValidation]
        [PasswordNumberValidation]
        public string Password { get; set; }
    }
}
