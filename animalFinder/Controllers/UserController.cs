using animalFinder.Exception;
using animalFinder.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using animalFinder.DTO.API;
using System.Security.Claims;

namespace animalFinder.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePersonalData(string name, string phoneNumber)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;

                var returnedUserId = await _userService.ChangePersonalData(userId, name, phoneNumber);

                Ok(returnedUserId);
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

        [HttpPost("generate-change-email-mail")]
        [Authorize]
        public async Task<IActionResult> GenerateChangeEmailMail([FromBody] string newEmail)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;

                await _userService.GenerateChangeEmailMail(userId, newEmail);

                Ok();
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

        [HttpGet("change-email")]
        public async Task<IActionResult> ChangeEmail([FromQuery] string email, [FromQuery] string token)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.PrimarySid).Value;

                await _userService.ChangeEmail(userId, email, token);

                Ok("The email address is changed");
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
    }
}
