using animalFinder.Constant;
using animalFinder.Enum;
using animalFinder.Exception;
using animalFinder.Object;
using animalFinder.Selector;
using animalFinder.Service.Interface;
using animalFinder.SettingsObject;
using DAL;
using DAL.Entity;
using DAL.Provider.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace animalFinder.Service
{
    public class AuthorizationService: IAuthorizationService
    {
        private readonly UserManager<User> userManager;
        private readonly IJWTService jwt;
        private readonly IEmailService email;
        private readonly IConfirmationCodeService generateConfirmationCode;
        private readonly IPasswordService resetPasswordCode;
        private readonly LinkGenerator linkGenerator;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly JWTSettings jwtSettings;
        private readonly IUnitOfWork unitOfWork;
        private readonly ITokenDataProvider tokenDataProvider;

        public AuthorizationService(UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IJWTService jwt,
            IEmailService email,
            IConfirmationCodeService generateConfirmationCode,
            IPasswordService resetPasswordCode,
            LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor,
            IOptions<JWTSettings> jwtSettings)
        {
            this.userManager = userManager;
            this.jwt = jwt;
            this.email = email;
            this.generateConfirmationCode = generateConfirmationCode;
            this.linkGenerator = linkGenerator;
            this.resetPasswordCode = resetPasswordCode;
            this.httpContextAccessor = httpContextAccessor;
            this.jwtSettings = jwtSettings.Value;
            this.unitOfWork = unitOfWork;
            this.tokenDataProvider = unitOfWork.Get<ITokenDataProvider>();
        }
        public async Task<DTO.Service.Token> Login(string identification, string password)
        {
            User user = await userManager.FindByNameAsync(identification);

            if (user is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.IsNotConfirmedUser);
            }

            if (!await userManager.CheckPasswordAsync(user, password))
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.EmailOrPasswordIncorrect);
            }

            string accessToken = jwt.Generate
                (
                    user.Id,
                    jwtSettings.Issuer,
                    jwtSettings.Audience,
                    jwtSettings.Bytes,
                    jwtSettings.TokenExpireTime
                );

            string refreshToken = jwt.Generate
                (
                    user.Id,
                    jwtSettings.Issuer,
                    jwtSettings.Audience,
                    jwtSettings.Bytes,
                    60 * 24 * 7
                );

            tokenDataProvider.Add(accessToken, refreshToken, user);
            unitOfWork.Commit();

            return new DTO.Service.Token()
            {
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<DTO.Service.Token> ReleaseRefreshToken(string userId, string refreshToken)
        {
            User user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            Token token = tokenDataProvider.GetUserTokenByRefreshToken(
                refreshToken.Split(" ")[1],
                userId
                );

            if (token is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            string newAccessToken = jwt.Generate
                (
                    user.Id,
                    jwtSettings.Issuer,
                    jwtSettings.Audience,
                    jwtSettings.Bytes,
                    jwtSettings.TokenExpireTime
                );

            string newRefreshToken = jwt.Generate
               (
                   user.Id,
                   jwtSettings.Issuer,
                   jwtSettings.Audience,
                   jwtSettings.Bytes,
                   60 * 24 * 7
               );

            token.AccessToken = newAccessToken;
            token.RefreshToken = newRefreshToken;

            tokenDataProvider.UpdateToken(token);
            unitOfWork.Commit();

            return new DTO.Service.Token()
            {
                UserId = user.Id,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task Logout(string userId, string accessToken)
        {
            User user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            Token token = tokenDataProvider.GetUserTokenByAccessToken(
                accessToken.Split(" ")[1],
                userId
                );

            if (token is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            tokenDataProvider.Remove(token);
            unitOfWork.Commit();
        }

        public async Task Register(string identification, string password)
        {
            User user = new User()
            {
                UserName = identification
            };

            user.Email = identification;

            IdentityResult result = await userManager.CreateAsync(user, password);

            if (ErrorSelector.Get(result, out var errors))
            {
                throw new ApiException(HttpStatusCode.BadRequest, errors[0]);
            }
            
            var callbackUri = await generateConfirmationCode.GenerateEmailConfirmationToken(
                 user,
                 httpContextAccessor.HttpContext);

            await email.Generate(user.Email, "ConfirmationMail", new EmailObject { Email = user.Email, Uri = callbackUri }, MailConstant.CONFIRMING_EMAIL);
        }

        public async Task ConfirmEmail(string userId, string code)
        {
            if (userId is null || code is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.IncorrectData);
            }

            User user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            IdentityResult result = await userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new ApiException(HttpStatusCode.NotAcceptable, ApiError.ConfirmationCodeIsWrong);
            }
        }

        public async Task ResetPassword(string userId, string code)
        {
            if (userId is null || code is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.IncorrectData);
            }

            User user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            string newPassword = resetPasswordCode.GeneratePassword();
            IdentityResult result = await userManager.ResetPasswordAsync(user, code, newPassword);
            if (!result.Succeeded)
            {
                throw new ApiException(HttpStatusCode.NotAcceptable, ApiError.ResetPasswordCodeIsWrong);
            }

            await email.Generate(user.Email, "ResetPasswordSuccessful", new EmailObject { Email = user.Email, Password = newPassword }, MailConstant.RESET_PASSWORD_SUCCESSFUL_EMAIL);
        }

        public async Task ReleaseResetPasswordCode(string login, string controllerName)
        {
            if (login is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.IncorrectData);
            }

            User user = await userManager.FindByNameAsync(login);
            if (user is null)
            {
                throw new ApiException(HttpStatusCode.Forbidden, ApiError.NotFound);
            }

            string callbackUri = await resetPasswordCode.GenerateResetCode(user, httpContextAccessor.HttpContext, controllerName);

            await email.Generate(user.Email, "ResetPasswordMail", new EmailObject { Email = user.Email, Uri = callbackUri }, MailConstant.RESET_PASSWORD_EMAIL);
        }

    }
}
