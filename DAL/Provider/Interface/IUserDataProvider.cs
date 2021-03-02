using DAL.Entity;

namespace DAL.Provider.Interface
{
    public interface IUserDataProvider
    {
        string GetId(string email);
        User Get(string userId);
    }
}
