using DAL.Entity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace animalFinder.Service.Interface
{
    public interface IConfirmationCodeService
    {
        public Task<string> GenerateEmailConfirmationToken(User user, HttpContext context);
        public Task<string> GenerateChangeEmailToken(User user, string newEmail, HttpContext context);
    }
}
