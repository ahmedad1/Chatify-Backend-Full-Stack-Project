using Chatify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RepositoryPattern.Core.DTOs;
using RepositoryPattern.Core.OptionPattern;
using RepositoryPattern.Core.RecaptchaResponseModel;
using RepositoryPatternUOW.Core.DTOs;


namespace Chatify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAuthenticationService authenticationService,IOptions<RecaptchaSecret>RecaptchaSecretOptions,JwtOptions jwtOptions,IOptions<RefreshTokenOptions> refreshTokenOptions) : ControllerBase
    {
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto)
        {
            var validatedRecaptcha = await ValidateRecaptchaAsync(signUpDto.RecaptchaToken, RecaptchaSecretOptions.Value.SecretKey, "signup");
            if (!validatedRecaptcha) 
                return BadRequest();
           var result= await authenticationService.SignUpAsync(signUpDto);
           return result.Success? Ok(result) : BadRequest(result);
        }
        [HttpPost("code/send")]
        public async Task<IActionResult> SendCode(EmailDto email)
        {
            var result = await authenticationService.SendVerificationCodeAsync(email.email);
            if (result.Success)
            {
                SetCookie("identityToken", result.Token!, DateTime.Now.AddMinutes(25), true);
                return Ok();
            }
            else
                return NotFound();
        }
        [HttpPost("code/verify")]
        public async Task<IActionResult> VerifyCode(VerifyCodeDto verifyCodeDto)
        {
            if (!Request.Cookies.TryGetValue("identityToken", out string? token))
                return NotFound();
            var result = await authenticationService.VerifyCodeAsync(verifyCodeDto.Email, verifyCodeDto.Code,token);
            return result? Ok():NotFound();
        }
        [HttpPost("login")]
        public async Task<IActionResult>Login(LoginDto loginDto)
        {
            var resultOfVerifingRecaptcha = await ValidateRecaptchaAsync(loginDto.recaptchaToken,RecaptchaSecretOptions.Value.SecretKey,"login");
            if (!resultOfVerifingRecaptcha)
                return NotFound();
            var result=await authenticationService.LoginAsync(loginDto);
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
                return Ok(new { Success = true, EmailConfirmed = false,result.Email });
            } 

            return NotFound();
        }
        private void SetCookie(string key, string value ,DateTime expirationDate,bool hasHttpOnlyFlag=false, bool hasSecureFlag = true)
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires= expirationDate;
            cookieOptions.Secure = hasSecureFlag;
            cookieOptions.HttpOnly = hasHttpOnlyFlag;
            //cookieOptions.SameSite = SameSiteMode.None;
            cookieOptions.SameSite = SameSiteMode.Strict;
            Response.Cookies.Append(key,value,cookieOptions);
            
        }
        [Authorize]
        [HttpDelete("sign-out")]
        public async Task<IActionResult> SignOut()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out string? val))
                return NotFound();
            var result = await authenticationService.SignOutAsync(val);
            if (result) {
                SetCookie("jwt", "", DateTime.Now.AddMinutes(-20), true);
                SetCookie("refreshToken", "", DateTime.Now.AddHours(-20), true);
                SetCookie("userName", "", DateTime.Now.AddHours(-20));
                SetCookie("email", "", DateTime.Now.AddHours(-20));
                SetCookie("firstName","", DateTime.Now.AddHours(-20));
                SetCookie("lastName","", DateTime.Now.AddHours(-20));
                SetCookie("id","", DateTime.Now.AddHours(-20));
                return Ok(); 
            }
            return NotFound();

        }
        [HttpPost("tokens")]
        public async Task<IActionResult> GetNewTokens()
        {
            if(!Request.Cookies.TryGetValue("refreshToken",out string? refToken)||!Request.Cookies.TryGetValue("email",out string? email))
                return Unauthorized();
            var result = await authenticationService.GetNewTokens(email, refToken);
            if (result.Success)
            {
                //SetCookie("jwt", result.Jwt!,DateTime.Now.AddMinutes(jwtOptions.ExpiresAfter),true,true);
                //SetCookie("refreshToken", result.RefreshToken!,DateTime.Now.AddMinutes(jwtOptions.ExpiresAfter),true,true);
                SetCookie("jwt", result.Jwt!, DateTime.Now.AddMinutes(jwtOptions.ExpiresAfter), true);
                SetCookie("refreshToken", result.RefreshToken!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter), true);
                SetCookie("userName", result.UserName!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
                SetCookie("email", result.Email!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
                SetCookie("firstName", result.FirstName!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
                SetCookie("lastName", result.LastName!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
                SetCookie("id", result.Id.ToString()!, DateTime.Now.AddHours(refreshTokenOptions.Value.ExpiresAfter));
                return Ok();
            }
            return Unauthorized();
            
        }
        private async Task<bool> ValidateRecaptchaAsync(string recaptchaToken, string secretKey,string actionName)
        {
            var formUrlEncoded = new FormUrlEncodedContent([KeyValuePair.Create("secret",secretKey),KeyValuePair.Create("response",recaptchaToken)]);
          
            using var fetcher = new HttpClient();
            var response=await fetcher.PostAsync($"https://www.google.com/recaptcha/api/siteverify",formUrlEncoded);
            if (!response.IsSuccessStatusCode)
                return false;
            var resultData =await response.Content.ReadFromJsonAsync<RecaptchaResponse>();

            return resultData!.success&&resultData.score>=.5 &&resultData.action==actionName;

        }
    }
}
