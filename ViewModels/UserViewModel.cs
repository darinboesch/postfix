using System.ComponentModel.DataAnnotations;

namespace postfix.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}