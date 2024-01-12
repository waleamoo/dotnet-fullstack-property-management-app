using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Dtos;
using WebAPI.Errors;
using WebAPI.Extensions;
using WebAPI.Interfaces;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class AccountsController : BaseController
    {
        private readonly IUnitOfWork uow;
        private readonly IConfiguration config;

        public AccountsController(IUnitOfWork uow, IConfiguration config)
        {
            this.uow = uow;
            this.config = config;
        }

        // /api/Accounts/Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginReqDto loginReq)
        {
            var user = await uow.UserRepository.Authenticate(loginReq.UserName, loginReq.Password);
            ApiError apiError = new ApiError();

            if (user == null)
            {
                apiError.ErrorCode = Unauthorized().StatusCode;
                apiError.ErrorMessage = "Invalid user id or password";
                apiError.ErrorDetails = "This error appears when provided user id or password does not exists";
                return Unauthorized(apiError);
            }

            var loginRes = new LoginResDto();
            loginRes.UserName = user.Username;
            loginRes.Token = CreateJWT(user);
            return Ok(loginRes);  
        }

        private string CreateJWT(User user)
        {
            // authentication key 
            // symmetric encryption - one key can be used for both encrypting and decrypting the token 
            // assymeteric encryption - one key for encrypting (public key) and another key for decrypting (private key - only shared by the key initiator)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["AppSettings:Key"]));

            // pieces of information about the user
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // define the secret key and security algorithm
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = signingCredentials
            };
            // token handler responsible for creating JWT 
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginReqDto loginReq)
        {
            ApiError apiError = new ApiError();

            if (loginReq.UserName.IsEmpty() || loginReq.Password.IsEmpty())
            {
                apiError.ErrorCode = BadRequest().StatusCode;
                apiError.ErrorMessage = "User name or password can not be blank";
                return BadRequest(apiError);
            }
                
            if (await uow.UserRepository.UserAlreadyExists(loginReq.UserName))
            {
                apiError.ErrorCode = BadRequest().StatusCode;
                apiError.ErrorMessage = "User already exist, please try something else";
                return BadRequest(apiError);
            }
            uow.UserRepository.Register(loginReq.UserName, loginReq.Password);
            await uow.SaveAsync();
            return StatusCode(201);
        }

    }
}
