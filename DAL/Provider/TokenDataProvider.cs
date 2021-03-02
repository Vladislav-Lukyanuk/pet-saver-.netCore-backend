using DAL.Entity;
using DAL.Provider.Interface;

namespace DAL.Provider
{
    public class TokenDataProvider : DataProvider<Token>, ITokenDataProvider
    {
        public TokenDataProvider(Context context) : base(context)
        {
        }

        public void Add(string token, string refreshToken, User user)
        {
            Add(new Token() { AccessToken = token, RefreshToken = refreshToken, UserId = user.Id });
        }

        public Token GetUserTokenByAccessToken(string token, string userId)
        {
            return Get(t => t.AccessToken.Equals(token) && t.UserId.Equals(userId));
        }
        public Token GetUserTokenByRefreshToken(string token, string userId)
        {
            return Get(t => t.RefreshToken.Equals(token) && t.UserId.Equals(userId));
        }

        public void UpdateToken(Token token)
        {
            Update(token);
        }
    }
}
