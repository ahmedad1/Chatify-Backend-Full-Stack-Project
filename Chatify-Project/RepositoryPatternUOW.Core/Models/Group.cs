using RepositoryPattern.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternUOW.Core.Models
{
    public class Group
    {
        public string Id { get; set; } = null!;
        public virtual ICollection<User> Users { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; }=null!;

    }
}
