using animalFinder.DTO.API;

namespace animalFinder.Builder.API
{
    public class ErrorBuilder
    {
        private Error error;
        public ErrorBuilder()
        {
            error = new Error();
        }
        public ErrorBuilder SetErrorMessage(string message)
        {
            error.Message = message;
            return this;
        }
        public Error Build() => error;
    }
}
