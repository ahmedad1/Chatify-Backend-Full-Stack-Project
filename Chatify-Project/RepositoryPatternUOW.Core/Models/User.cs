using RepositoryPattern.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternUOW.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }=null!;
        public string LastName { get; set; }=null!;
        public string UserName { get; set; }=null!;
        public string Email { get; set; }=null!;
        public string Password { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public virtual ICollection<UserConnection>? UserConnections { get; set; }
        public virtual ICollection<Group>? Groups { get; set; }
        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }
        public virtual VerificationCode? VerificationCode { get; set; }

    }
}
