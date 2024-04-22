

namespace RepositoryPattern.Core.OptionPattern
{
    public class JwtOptions
    {

        
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public string IssuerSigningKey { get; set; }
            public int ExpiresAfter { get; set; }
      

    }
}
