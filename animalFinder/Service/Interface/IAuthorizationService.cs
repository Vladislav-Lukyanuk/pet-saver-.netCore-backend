using animalFinder.DTO.Service;
using System.Threading.Tasks;

namespace animalFinder.Service.Interface
{
    public interface IAuthorizationService
    {
        Task<Token> Login(string identification, string password);
        Task ReleaseResetPasswordCode(string login, string controllerName);
        Task ResetPassword(string userId, string code);
        Task<Token> ReleaseRefreshToken(string userId, string refreshToken);
        Task Register(string identification, string password);
        Task ConfirmEmail(string userId, string code);
        Task Logout(string userId, string token);
    }
}
