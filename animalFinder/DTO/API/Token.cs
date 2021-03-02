using animalFinder.Builder.API;

namespace animalFinder.DTO.API
{
    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public static TokenBuilder CreateBuilder() => new TokenBuilder();
    }
}
