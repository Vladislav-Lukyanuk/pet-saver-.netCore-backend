using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace animalFinder.Selector
{
    public class ErrorSelector
    {
        public static bool Get(IdentityResult result, out string[] errors)
        {
            if (!result.Succeeded)
            {
                errors = result.Errors.Select(e => e.Code).ToArray();
                return true;
            }

            errors = null;
            return false;
        }
    }
}
