using RepositoryPattern.Core.DTOs;
using RepositoryPattern.Core.ResultModels;
using RepositoryPatternUOW.Core.DTOs;

namespace Chatify.Services
{
    public interface IAuthenticationService
    {
        public Task<SignUpResult> SignUpAsync(SignUpDto signUpDto);
        public Task<SendCodeResult> SendVerificationCodeAsync(string email);
        public Task<bool> VerifyCodeAsync(string email, string code, string identityToken);
        public Task<LoginResult> LoginAsync(LoginDto loginDto);
        public Task<bool> SignOutAsync(string refToken);
        public Task<GetNewTokensResult> GetNewTokens(string email,string refToken);
    }

}
