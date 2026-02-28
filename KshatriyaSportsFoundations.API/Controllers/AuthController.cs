using KshatriyaSportsFoundations.API.Models.Dtos.Auth;
using KshatriyaSportsFoundations.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KshatriyaSportsFoundations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;
        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        //api/auth/register
        [HttpPost]
        [Route("RegisterSecret")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                IdentityUser identityUser = new IdentityUser
                {
                    UserName = registerUserDto.Username,
                    Email = registerUserDto.Username
                };
                var response = await _userManager.CreateAsync(identityUser, registerUserDto.Password);

                if (response.Succeeded)
                {
                    response = await _userManager.AddToRolesAsync(identityUser, registerUserDto.Roles);

                    if (response.Succeeded)
                        return Ok("Successfully user registered, please login");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "something went wrong,please try again");
            }
            return BadRequest("something went wrong,please try again");
        }

        //post: api/auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);
            LoginResponseDto loginResponse = new();

            try
            {
                if (user != null)
                {
                    var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                    if (isValidPassword)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles != null)
                        {
                            //create token
                            var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

                            loginResponse.Status = true;
                            loginResponse.Message = "Token successfully generated";
                            loginResponse.JwtToken = jwtToken;
                            return Ok(loginResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loginResponse.Status = false;
                loginResponse.Message = ex.Message;
                return StatusCode(500, loginResponse);
            }
            loginResponse.Status = false;
            loginResponse.Message = "Invalid email or password";

            return BadRequest(loginResponse);
        }
    }
}
