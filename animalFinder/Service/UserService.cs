using animalFinder.Constant;
using animalFinder.Enum;
using animalFinder.Exception;
using animalFinder.Object;
using animalFinder.Service.Interface;
using DAL;
using DAL.Entity;
using DAL.Provider.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Threading.Tasks;

namespace animalFinder.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserDataProvider _userDataProvider;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfirmationCodeService _confirmationCodeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            IConfirmationCodeService confirmationCodeService,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService
            )
        {
            _unitOfWork = unitOfWork;
            _userDataProvider = _unitOfWork.Get<IUserDataProvider>();
            _userManager = userManager;
            _confirmationCodeService = confirmationCodeService;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public string GetId(string email)
        {
            return _userDataProvider.GetId(email);
        }

        public async Task GenerateChangeEmailMail(string userId, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            var uri = await _confirmationCodeService.GenerateChangeEmailToken(user, newEmail, _httpContextAccessor.HttpContext);

            await _emailService.Generate(user.Email, "ConfirmationMail", new EmailObject { Email = user.Email, Uri = uri }, MailConstant.CONFIRMING_EMAIL);
        }

        public async Task<string> ChangeEmail(string userId, string newEmail, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            if (!result.Succeeded) 
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            return userId;
        }

        public async Task<string> ChangePersonalData(string userId, string name, string phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            var nameResult = await _userManager.SetUserNameAsync(user, name);

            var phoneNumberResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);

            if (!nameResult.Succeeded || !phoneNumberResult.Succeeded)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.AccessForbidden);
            }

            return userId;
        }
    }
}
