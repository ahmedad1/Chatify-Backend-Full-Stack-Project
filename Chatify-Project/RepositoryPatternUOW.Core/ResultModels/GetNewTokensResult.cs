using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.ResultModels
{
    public class GetNewTokensResult
    {
        public bool Success { get; set; }
        public string? RefreshToken { get; set; }
        public string? Jwt { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Id { get; set; }
        public string? Email { get; set; }
    }
}
