using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.ResultModels
{
    public record LoginResult
    (
        bool Success=false,
        bool EmailConfirmed=false,
        string? Jwt=null,
        string? RefreshToken = null,
        string? Email = null,
        string? FirstName = null,
        string? LastName=null,
        int? Id=null

        );
}
