using postfix.Shared.Identity;

namespace postfix.Models.User
{
    public class PostfixUser : IdentityUser
    {
        // custom properties
        public string Notes { get; set; }
    }
}