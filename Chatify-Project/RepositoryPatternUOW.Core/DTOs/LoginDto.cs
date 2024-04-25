

using System.ComponentModel.DataAnnotations;

namespace RepositoryPattern.Core.DTOs
{
    public class LoginDto {
        [StringLength(100)]
        public string UserName { get; set; }
        [StringLength(100)]
        public string Password { get; set; }

        public LoginDto( string userName, string password)
        {
            Password = password;
            UserName = userName;
        }
    }
   
}
