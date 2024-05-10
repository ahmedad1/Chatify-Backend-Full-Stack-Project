using Azure.Core;
using System.IdentityModel.Tokens.Jwt;

namespace Chatify
{
    public class JwtHandler
    {
        public static JwtPayload ExtractPayload(HttpRequest request)
        {

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(request.Cookies["jwt"]);
            return jwt.Payload;
        }
    }
}
