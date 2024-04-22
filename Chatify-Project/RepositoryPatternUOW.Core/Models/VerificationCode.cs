using RepositoryPatternUOW.Core.Models;


namespace RepositoryPattern.Core.Models
{
    public class VerificationCode
    {
        public string Code { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive => ExpiresAt > DateTime.Now;
        public virtual User User { get; set; } = null!;
    }

}
