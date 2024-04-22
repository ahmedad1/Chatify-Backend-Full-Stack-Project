using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternUOW.Core.DTOs
{
    public record SignUpDto(
        string FirstName,
        string LastName,
        string UserName,
        string Email,
        string Password
        );
   
}
