using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Math.EC.Rfc7748;
using RepositoryPattern.Core.DTOs;
using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.OptionPattern;
using RepositoryPattern.Core.ResultModels;
using RepositoryPatternUOW.Core.DTOs;
using RepositoryPatternUOW.Core.Interfaces;
using RepositoryPatternUOW.Core.Models;
using RepositoryPatternUOW.EFcore.Mapperly;


namespace RepositoryPatternUOW.EFcore.Repositories
{
    public class UserRepository(AppDbContext context,MapToModel mapToModel,ISenderService senderService,IGenerateTokens generateTokens,IOptions<RefreshTokenOptions>refreshTokenOptions) : IUserRepository
    {
        public async Task<LoginResult> LoginAsync(LoginDto loginDto)
        {
            context.ChangeTracker.LazyLoadingEnabled = false;
            var user=await context.Users.AsNoTracking().FirstOrDefaultAsync(x=>x.UserName==loginDto.UserName);
            if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(loginDto.Password, user.Password)) 
                return new();
            if(!user.EmailConfirmed)
                return new(true);
            var jwt = generateTokens.GenerateToken(user);
            var refreshToken = generateTokens.GenerateToken();
            context.ChangeTracker.LazyLoadingEnabled = true;
            context.Attach(user);
            user.RefreshTokens!.Add(new()
            {
                ExpiresAt = DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter),
                Token=refreshToken
            });
            await context.SaveChangesAsync();
            return new(true,true,jwt,refreshToken,user.Email,user.FirstName,user.LastName,user.Id);
           
        }

        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            context.ChangeTracker.LazyLoadingEnabled = false;
            var user = await context.Users.AsNoTracking().Include(x => x.VerificationCode).FirstOrDefaultAsync(x => x.Email == email);
            if (user is { EmailConfirmed: true }or null)
                return false;

            if (user!.VerificationCode is { IsActive: true })
                return true;
            else if (user!.VerificationCode is { IsActive: false })
                context.Remove(user.VerificationCode);
            int code = Random.Shared.Next(100000, int.MaxValue);
            string bodyOfMessage =
                @$"Dear {email} ,
                                   you have signed up on our Chatify web application, 
                                   and we have sent to you a verification code which is : <b>{code}</b> ";
            string subject =  "Email Confirmation";

            context.ChangeTracker.LazyLoadingEnabled = true;
            var expirationOfCode= DateTime.Now.AddMinutes(1.5);
            context.Attach(user);
            user.VerificationCode = new() { Code = code.ToString(), ExpiresAt = expirationOfCode  };
            try
            {
                Task t1 = context.SaveChangesAsync();
                Task t2 = senderService.SendAsync(email, subject, bodyOfMessage);
                await Task.WhenAll(t1, t2);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;

            }
            
        }

        public async Task<bool> SignOutAsync(string refToken)
        {
            var result = await context.RefreshTokens.Where(x=>x.Token==refToken).ExecuteDeleteAsync();
            return result > 0;
        }

        public async Task<SignUpResult> SignUpAsync(SignUpDto signUpDto)
        {
           if(await context.Users.AnyAsync(x => x.Email == signUpDto.Email))
                return new(false, true, false);
           if(await context.Users.AnyAsync(x => x.UserName == signUpDto.UserName))
                return new(false, false, true);
            User user =mapToModel.MapSignUpDto(signUpDto);
            user.Password=BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return new(true, false, false);  
        }
        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            context.ChangeTracker.LazyLoadingEnabled = false;
            var user = await context.Users.Include(x=>x.VerificationCode).AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
            if (user is null) return false;
            Func<bool,Task> cleanAndConfirmCode = async (bool confirm) => {
                context.VerifcationCodes.Remove(user.VerificationCode!);
                if (confirm)
                {
                    user.EmailConfirmed = true;
                    context.Update(user);
                }
                await context.SaveChangesAsync();
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

    }
}
