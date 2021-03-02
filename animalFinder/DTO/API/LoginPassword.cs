using System.ComponentModel.DataAnnotations;

namespace animalFinder.DTO.API
{
    public class LoginPassword
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
