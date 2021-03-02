using System.Security.Claims;
using System.Threading.Tasks;
using animalFinder.DTO.API;
using animalFinder.Exception;
using animalFinder.Service.Interface;
using animalFinder.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = animalFinder.Service.Interface.IAuthorizationService;

namespace animalFinder.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthorizationService authorization;
        private readonly IUserService userService;

        public AuthController(IAuthorizationService authorization, IUserService userService)
        {
            this.authorization = authorization;
            this.userService = userService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult UserInfo()
        {
            string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
            return Ok(new
            {
                userId = userId
            });
        }

        [HttpPost("register")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] LoginPassword registerForm)
        {
            try
            {
                await authorization.Register(registerForm.Login, registerForm.Password);
            }
            catch (ApiException exp)
            {
                return StatusCode(
                    exp.GetStatusCode(),
                    Error.CreateBuilder().SetErrorMessage(exp.GetErrorMessage()).Build()
                    );
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] LoginPassword loginForm)
        {
            DTO.Service.Token token;
            string userId;
            try
            {
                token = await authorization.Login(loginForm.Login, loginForm.Password);
                userId = userService.GetId(loginForm.Login);
            }
            catch (ApiException exp)
            {
                return StatusCode(
                    exp.GetStatusCode(),
                    Error.CreateBuilder().SetErrorMessage(exp.GetErrorMessage()).Build()
                    );
            }
            catch
            {
                return BadRequest();
            }

            return Ok(new
            {
                tokens = Token.CreateBuilder()
                .SetAccessToken(token.AccessToken)
                .SetRefreshToken(token.RefreshToken)
                .Build(),
                userId
            })
                ;
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var token);

            try
            {
                await authorization.Logout(userId, token);
            }
            catch (ApiException exp)
            {
                return StatusCode(
                    exp.GetStatusCode(),
                    Error.CreateBuilder().SetErrorMessage(exp.GetErrorMessage()).Build()
                    );
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("release-refresh-token")]
        public async Task<IActionResult> ReleaseRefreshToken()
        {
            string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;
            HttpContext.Request.Headers.TryGetValue("Authorization", out var refreshToken);

            DTO.Service.Token token;
            try
            {
                token = await authorization.ReleaseRefreshToken(userId, refreshToken);
            }
            catch (ApiException exp)
            {
                return StatusCode(
                    exp.GetStatusCode(),
                    Error.CreateBuilder().SetErrorMessage(exp.GetErrorMessage()).Build()
                    );
            }
            catch
            {
                return BadRequest();
            }

            return Ok(
                Token.CreateBuilder()
                .SetAccessToken(token.AccessToken)
                .SetRefreshToken(token.RefreshToken)
                .Build());
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string redirectTo)
        {
            try
            {
                await authorization.ConfirmEmail(userId, code);
                return Ok("The email confirmation successfully");
            }
            catch (ApiException exp)
            {
                return StatusCode(
                    exp.GetStatusCode(),
                    Error.CreateBuilder().SetErrorMessage(exp.GetErrorMessage()).Build()
                    );
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("release-reset-password-code")]
        public async Task<IActionResult> ReleaseResetPasswordCode([FromBody] ForgotPassword registerForm)
        {
            try
            {
                await authorization.ReleaseResetPasswordCode(registerForm.Login, "auth");
                return Ok();
            }
            catch (ApiException exp)
            {
                return StatusCode(
                    exp.GetStatusCode(),
                    Error.CreateBuilder().SetErrorMessage(exp.GetErrorMessage()).Build()
                    );
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string userId, string code)
        {
            try
            {
                await authorization.ResetPassword(userId, code);
                return Ok("Password reset successfully");
            }
            catch (ApiException exp)
            {
                return StatusCode(
                    exp.GetStatusCode(),
                    Error.CreateBuilder().SetErrorMessage(exp.GetErrorMessage()).Build()
                    );
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}