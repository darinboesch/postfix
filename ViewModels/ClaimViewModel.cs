using System.ComponentModel.DataAnnotations;

namespace postfix.ViewModels
{
    public class ClaimViewModel
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }
    }
}