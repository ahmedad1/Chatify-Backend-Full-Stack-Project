using Microsoft.Extensions.Options;
using RepositoryPattern.Core.DTOs;
using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.OptionPattern;
using RepositoryPattern.Core.ResultModels;
using RepositoryPatternUOW.Core.DTOs;
using RepositoryPatternUOW.Core.Models;
using RepositoryPatternUOW.EFcore.Mapperly;

namespace Chatify.Services
{
    public class AuthenticationService(IUnitOfWork unitOfWork, MapToModel mapToModel, IGenerateTokens generateTokens, IOptions<RefreshTokenOptions> refreshTokenOptions, ISenderService senderService) :IAuthenticationService
    {
        public async Task<LoginResult> LoginAsync(LoginDto loginDto)
        {

            var user=await unitOfWork.UserRepository.GetOneByAsync(x=>x.UserName==loginDto.UserName,false);
            if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(loginDto.Password, user.Password))
                return new();
            if (!user.EmailConfirmed)
                return new(true, false, null, null, user.Email);
            var jwt = generateTokens.GenerateToken(user);
            var refreshToken = generateTokens.GenerateToken();
            unitOfWork.SetLazyLoading(true);
            unitOfWork.UserRepository.Attach(user);
            if (user.RefreshTokens!.Count() > 5)
               await unitOfWork.RefreshTokenRepository.ExecuteDeleteAsync(x => x.UserId == user.Id);
            user.RefreshTokens!.Add(new()
            {
                ExpiresAt = DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter),
                Token = refreshToken
            });
            await unitOfWork.SaveChangesAsync();
            return new(true, true, jwt, refreshToken, user.Email, user.FirstName, user.LastName, user.Id);

        }

        public async Task<SendCodeResult> SendVerificationCodeAsync(string email)
        {
            var user = await unitOfWork.UserRepository.GetOneByAsync(x => x.Email == email, false, ["IdentityTokenVerification", "VerificationCode"]);
            if (user is { EmailConfirmed: true } or null)
                return new(false);

            if (user!.VerificationCode is { IsActive: true })
                return new(true, user.IdentityTokenVerification.Token, user.IdentityTokenVerification.ExpiresAt);
            else if (user!.VerificationCode is { IsActive: false })
                unitOfWork.VerificationCodeRepository.Remove(user.VerificationCode);
            int code = Random.Shared.Next(100000, int.MaxValue);
            string bodyOfMessage =
                @$"Dear {email} ,
                                   you have signed up on our Chatify web application, 
                                   and we have sent to you a verification code which is : <b>{code}</b> ";
            string subject = "Email Confirmation";


            var expirationOfCode = DateTime.Now.AddMinutes(1.5);
            unitOfWork.UserRepository.Attach(user);
            user.VerificationCode = new() { Code = code.ToString(), ExpiresAt = expirationOfCode };
            if (user.IdentityTokenVerification is not null)
                unitOfWork.IdentityTokenVerificationRepository.Remove(user.IdentityTokenVerification);
            var token = generateTokens.GenerateToken();
            var expDate = DateTime.Now.AddMinutes(25);
            user.IdentityTokenVerification = new()
            {
                Token = token,
                ExpiresAt = expDate,

            };

            try
            {

                Task t1 = unitOfWork.SaveChangesAsync();
                Task t2 = senderService.SendAsync(email, subject, bodyOfMessage);
                await Task.WhenAll(t1, t2);
                return new(true, token, expDate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new(false);

            }

        }

        public async Task<bool> SignOutAsync(string refToken)
        {
            var result = await unitOfWork.RefreshTokenRepository.ExecuteDeleteAsync(x => x.Token == refToken);
            return result > 0;
        }

        public async Task<SignUpResult> SignUpAsync(SignUpDto signUpDto)
        {
            if (await unitOfWork.UserRepository.ExistsAsync(x => x.Email == signUpDto.Email))
                return new(false, true, false);
            if (await unitOfWork.UserRepository.ExistsAsync(x => x.UserName == signUpDto.UserName))
                return new(false, false, true);
            User user = mapToModel.MapSignUpDto(signUpDto);
            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
            await unitOfWork.UserRepository.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
            return new(true, false, false);
        }
        public async Task<bool> VerifyCodeAsync(string email, string code, string identityToken)
        {
           
            unitOfWork.SetLazyLoading(false);
            var user = await unitOfWork.UserRepository.GetOneByAsync(x => x.Email == email, false, ["VerificationCode", "IdentityTokenVerification"]);
            if (user is null || user.IdentityTokenVerification is null || !user.IdentityTokenVerification.IsActive) return false;
            Func<bool, Task> cleanAndConfirmCode = async (bool confirm) => {
                unitOfWork.VerificationCodeRepository.Remove(user.VerificationCode!);
                if (confirm)
                {
                    unitOfWork.IdentityTokenVerificationRepository.Remove(user.IdentityTokenVerification);
                    user.EmailConfirmed = true;
                
                }
                await unitOfWork.SaveChangesAsync();
            };
            if (user.VerificationCode is { IsActive: false })
            {
                await cleanAndConfirmCode(false);
                return false;
            }
            else if (user.VerificationCode is { IsActive: true } && user.VerificationCode.Code == code)
            {
                await cleanAndConfirmCode(true);
                return true;

            }
            return false;
        }
        public async Task<GetNewTokensResult> GetNewTokens(string email,string refToken)
        {
            var refreshToken = await unitOfWork.RefreshTokenRepository.GetOneByAsync(x => x.Token == refToken && x.User.Email == email, false, ["User"]) ;
            
            if (refreshToken is null) return new() { Success=false};
            if(refreshToken is not { IsActive: true })
            {
               await unitOfWork.RefreshTokenRepository.ExecuteDeleteAsync(x => x.Token == refToken);
               return new() { Success=false};
            }
            var newRefToken = generateTokens.GenerateToken();
            var newJwt = generateTokens.GenerateToken(refreshToken.User);
            var result = new GetNewTokensResult() { Success = true, Jwt = newJwt, RefreshToken = newRefToken, UserName = refreshToken.User.UserName, Email = refreshToken.User.Email, FirstName = refreshToken.User.FirstName, LastName = refreshToken.User.LastName, Id = refreshToken.UserId };
            unitOfWork.RefreshTokenRepository.Remove(refreshToken);
            refreshToken.Token = newRefToken;
            refreshToken.ExpiresAt = DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter);
            await unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await unitOfWork.SaveChangesAsync();
            return result ;
        }

    }
}
