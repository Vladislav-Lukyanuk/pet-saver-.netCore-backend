using DAL.Entity;
using DAL.Provider.Interface;
using System;

namespace DAL.Provider
{
    public class UserDataProvider : DataProvider<User>, IUserDataProvider
    {
        public UserDataProvider(Context context) : base(context)
        {
        }

        public string GetId(string email)
        {
            return Get(u => u.Email.Equals(email)).Id;
        }

        public User Get(string userId)
        {
            return Get(u => u.Id.Equals(userId));
        }
    }
}
