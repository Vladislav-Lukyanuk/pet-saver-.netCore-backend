using animalFinder.DTO.API;

namespace animalFinder.Builder.API
{
    public class TokenBuilder
    {
        private Token token;
        public TokenBuilder()
        {
            token = new Token();
        }
        public TokenBuilder SetAccessToken(string accessToken)
        {
            token.AccessToken = accessToken;
            return this;
        }

        public TokenBuilder SetRefreshToken(string refreshToken)
        {
            token.RefreshToken = refreshToken;
            return this;
        }

        public Token Build() => token;
    }
}
