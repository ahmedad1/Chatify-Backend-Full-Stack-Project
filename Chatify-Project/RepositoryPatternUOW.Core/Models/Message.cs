using RepositoryPatternUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string GroupId { get; set; }
        public string MessageText { get; set; }=null!;
        public bool IsRead { get; set; }
        public virtual User Sender  { get; set; }=null!;
        public virtual User Receiver { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;
    }
}
