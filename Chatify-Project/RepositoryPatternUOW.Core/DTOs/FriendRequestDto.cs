using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.DTOs
{
    public class FriendRequestDto
    {
        [StringLength(100)]
        public string UserName { get; set; }
    }
}
