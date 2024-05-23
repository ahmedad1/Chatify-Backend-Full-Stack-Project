using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternUOW.Core.DTOs
{
    public class SignUpDto
    {
        public SignUpDto(
        string FirstName,
        string LastName,
        string UserName,
        string Email,
        string Password,
        string RecaptchaToken
        )
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.UserName = UserName;
            this.Email = Email;
            this.Password = Password;
            this.RecaptchaToken = RecaptchaToken;
        }
        [StringLength(100)]
        public string FirstName { get; }
        [StringLength(100)]
        public string LastName { get; }
        [StringLength(100)]
        public string UserName { get; }
        [RegularExpression(@"\w+@\w+\.\w+(\.\w+)*", ErrorMessage = "Invalid Email")]
        [StringLength(100)]
        public string Email { get; }
        [StringLength(100,MinimumLength =8)]
        public string Password { get; }
        public string RecaptchaToken { get; set; }
    }
   
}
