using System.Threading.Tasks;

namespace animalFinder.Service.Interface
{
    public interface IUserService
    {
        string GetId(string email);
        Task GenerateChangeEmailMail(string userId, string newEmail);
        Task<string> ChangeEmail(string userId, string newEmail, string token);
        Task<string> ChangePersonalData(string userId, string name, string phoneNumber);

    }
}
