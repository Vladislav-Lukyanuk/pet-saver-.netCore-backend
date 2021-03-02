namespace animalFinder.Service.Interface
{
    public interface IJWTService
    {
        public string Generate(string userId, string issuer, string audience, string tokenBytes, int expireTime);
    }
}
