using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.DTOs
{
    public class ResponseFriendRequestDto
    {
        [StringLength(100)]
        public string UserName { get; set; }
        public bool IsAccepted { get; set; }
    }
}
