namespace animalFinder.SettingsObject
{
    public class JWTSettings
    {
        public string Bytes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenExpireTime { get; set; }
    }
}
