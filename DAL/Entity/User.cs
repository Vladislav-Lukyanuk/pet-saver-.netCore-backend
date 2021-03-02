using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entity
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public override string PhoneNumber { get; set; }
        public ICollection<Token> Tokens { get; set; }
    }
}
