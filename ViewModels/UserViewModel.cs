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

    public class UserClaimViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }
    }
}