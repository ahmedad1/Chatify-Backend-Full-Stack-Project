using RepositoryPattern.Core.DTOs;
using RepositoryPattern.Core.ResultModels;
using RepositoryPatternUOW.Core.DTOs;


namespace RepositoryPatternUOW.Core.Interfaces
{
    public interface IUserRepository
    {
        public Task<SignUpResult> SignUpAsync(SignUpDto signUpDto);
        public Task<bool> SendVerificationCodeAsync(string email);
        public Task<bool>VerifyCodeAsync(string email, string code);
        public Task<LoginResult>LoginAsync(LoginDto loginDto);
       
    }
}
