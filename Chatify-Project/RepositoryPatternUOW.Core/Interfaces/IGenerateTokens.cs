using RepositoryPatternUOW.Core.Models;

namespace RepositoryPattern.Core.Interfaces
{
    public interface IGenerateTokens
    {
        /// <summary>
        /// Generates JWT Token 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The Token String</returns>

        public string GenerateToken(User user);

        /// <summary>
        /// Generates The Refresh Token
        /// </summary>
        /// <returns>returns the refresh token string</returns>
        public string GenerateToken();
    }
}
