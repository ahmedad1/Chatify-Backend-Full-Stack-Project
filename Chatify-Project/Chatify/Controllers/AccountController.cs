using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RepositoryPattern.Core.DTOs;
using RepositoryPattern.Core.OptionPattern;
using RepositoryPatternUOW.Core.DTOs;
using RepositoryPatternUOW.Core.Interfaces;

namespace Chatify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserRepository userRepository,JwtOptions jwtOptions,IOptions<RefreshTokenOptions> refreshTokenOptions) : ControllerBase
    {
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto)
        {
           var result= await userRepository.SignUpAsync(signUpDto);
           return result.Success? Ok(result) : BadRequest(result);
        }
        [HttpPost("code/send")]
        public async Task<IActionResult> SendCode([FromBody]string email)
        {
            var result = await userRepository.SendVerificationCodeAsync(email);
            return result? Ok():NotFound();
        }
        [HttpPost("code/verify")]
        public async Task<IActionResult> VerifyCode(VerifyCodeDto verifyCodeDto)
        {
            var result = await userRepository.VerifyCodeAsync(verifyCodeDto.Email, verifyCodeDto.Code);
            return result? Ok():NotFound();
        }
        [HttpPost("login")]
        public async Task<IActionResult>Login(LoginDto loginDto)
        {
            var result=await userRepository.LoginAsync(loginDto);
            if(result is { Success:true ,EmailConfirmed:true})
            {
                SetCookie("jwt", result.Jwt!,DateTime.Now.AddMinutes(jwtOptions.ExpiresAfter),true);
                SetCookie("refreshToken", result.RefreshToken!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter),true);
                SetCookie("userName", loginDto.UserName!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
                SetCookie("email", result.Email!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
                SetCookie("firstName", result.FirstName!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
                SetCookie("lastName", result.LastName!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
                SetCookie("id", result.Id.ToString()!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
               
                return Ok(new {Success=true,EmailConfirmed=true});
            }else if(result is { Success: true })
            {
                return Ok(new { Success = true, EmailConfirmed = false });
            } 

            return NotFound();
        }
        private void SetCookie(string key, string value ,DateTime expirationDate,bool hasHttpOnlyFlag=false, bool hasSecureFlag = true)
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires= expirationDate;
            cookieOptions.Secure = hasSecureFlag;
            cookieOptions.HttpOnly = hasHttpOnlyFlag;
            cookieOptions.SameSite = SameSiteMode.None;
            Response.Cookies.Append(key,value,cookieOptions);
            
        }

    }
}
