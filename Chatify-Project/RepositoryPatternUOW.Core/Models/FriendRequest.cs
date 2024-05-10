using RepositoryPatternUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.Models
{
    public class FriendRequest
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public virtual User Sender { get; set; } = null!;
        public virtual User Recipient { get; set; }=null!;
    }
}
