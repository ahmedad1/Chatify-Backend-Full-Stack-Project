using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepositoryPattern.Core.ResultModels
{
    public class SearchResult
    {
           [JsonIgnore]
           public int Id { get; set; }
           public string UserName { get; set; }
           public string FirstName { get; set; }
           public string LastName { get; set; }
           public bool GotRequest { get; set; } = false; 
}
}
