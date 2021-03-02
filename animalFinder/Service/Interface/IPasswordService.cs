using DAL.Entity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace animalFinder.Service.Interface
{
    public interface IPasswordService
    {
        Task<string> GenerateResetCode(User user, HttpContext context, string controllerName);
        string GeneratePassword();
    }
}
