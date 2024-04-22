using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternUOW.Core.Models
{
    public class RefreshToken
    {
        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsActive => ExpiresAt > DateTime.Now;
        public virtual User User { get; set; } = null!;

    }
}
