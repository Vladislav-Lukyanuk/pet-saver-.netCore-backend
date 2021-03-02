using DAL.Entity;

namespace DAL.Provider.Interface
{
    public interface ITokenDataProvider
    {
        public void Add(string token, string refreshToken, User user);
        void Remove(Token token);
        public Token GetUserTokenByAccessToken(string token, string userId);
        public Token GetUserTokenByRefreshToken(string token, string userId);
        public void UpdateToken(Token token);
    }
}
