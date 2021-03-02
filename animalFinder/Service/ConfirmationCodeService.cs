using animalFinder.Service.Interface;
using DAL.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace animalFinder.Service
{
    public class ConfirmationCodeService : IConfirmationCodeService
    {
        private readonly UserManager<User> userManager;
        private readonly LinkGenerator linkGenerator;

        public ConfirmationCodeService(UserManager<User> userManager, LinkGenerator linkGenerator)
        {
            this.userManager = userManager;
            this.linkGenerator = linkGenerator;
        }

        public async Task<string> GenerateEmailConfirmationToken(User user, HttpContext context)
        {
            string code = await userManager.GenerateEmailConfirmationTokenAsync(user);

            return linkGenerator.GetUriByAction(context,
            "confirmemail",
            "auth",
            new { userId = user.Id, code }
            );
        }

        public async Task<string> GenerateChangeEmailToken(User user, string newEmail, HttpContext context)
        {
            string token = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            return linkGenerator.GetUriByAction(context,
            "changeemail",
            "user",
            new { email = newEmail, token }
            );
        }
    }
}
