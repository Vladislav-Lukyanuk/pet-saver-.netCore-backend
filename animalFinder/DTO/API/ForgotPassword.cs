using System.ComponentModel.DataAnnotations;

namespace animalFinder.DTO.API
{
    public class ForgotPassword
    {
        [Required]
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
