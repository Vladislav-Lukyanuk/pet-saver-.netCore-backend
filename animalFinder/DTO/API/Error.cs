using animalFinder.Builder.API;

namespace animalFinder.DTO.API
{
    public class Error
    {
        public string Message { get; set; }
        public static ErrorBuilder CreateBuilder() => new ErrorBuilder();
    }
}
