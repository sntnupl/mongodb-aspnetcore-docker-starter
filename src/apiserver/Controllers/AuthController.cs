using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoCore.DbDriver;
using MongoCore.DbDriver.Documents;
using MongoCore.WebApi.Helpers;
using MongoCore.WebApi.Models.Users;

namespace MongoCore.WebApi.Controllers
{
    [Authorize]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IUserManager _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IAppConfig _appConfig;

        public AuthController(ILogger<AuthController> logger, IAppConfig appConfig, IUserManager userManager)
        {
            _logger = logger;
            _appConfig = appConfig;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]AppUserLoginDto loginDto)
        {
            _logger.LogDebug($"[POST] api/auth/login");
            if (null == loginDto) return BadRequest();
            if (!ModelState.IsValid) return new UnprocessableEntityObjectResult(ModelState);
            
            var user = await _userManager.FindByUsernameAsync(loginDto.UserName);
            if (null == user) return BadRequest();
            
            if (!PasswordHasher.Match(loginDto.UserPassword, user.PasswordHash)) return BadRequest();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.TokenConfigs.Key)); 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = AddClaims(user);
            var roles = AddRoles(user);

            var token = new JwtSecurityToken(
                issuer: _appConfig.TokenConfigs.Issuer,
                audience: _appConfig.TokenConfigs.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            ) {Payload = {["roles"] = roles}};

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogDebug("[GET] api/admin/test");
            return Ok();
        }
        
        [HttpGet("testadmin")]
        public IActionResult TestAdmin()
        {
            _logger.LogDebug("[GET] api/admin/testadmin");
            var accessToken = HttpContext.Request.Headers["Authorization"];
            foreach (var token in accessToken) {
                Console.WriteLine($"[TestAdmin] {token}");
            }
            
            var caller = User as ClaimsPrincipal;
            foreach (var claim in caller.Claims) {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }

            var emailClaim = caller.Claims.FirstOrDefault(c => c.Type.Equals("appuser-email"));
            Console.WriteLine($"[TestAdmin] appuser-email: {emailClaim?.Value}");
            
            return Ok();
        }

        private Claim[] AddClaims(UserDocument user)
        {
            //var userClaimns = await _userManager.GetClaimsAsync(user);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("appuser-email", user.Email)
            };

            return claims.ToArray();
        }

        private string[] AddRoles(UserDocument user)
        {
            var userRoles = new List<string>();
            if (user.Admin) userRoles.Add("admin");
            return userRoles.ToArray();
        }

    }
}