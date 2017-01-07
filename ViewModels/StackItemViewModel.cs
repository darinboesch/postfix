using System.ComponentModel.DataAnnotations;

namespace postfix.ViewModels
{
    public class StackItemViewModel
    {
        [Required]
        public dynamic Item { get; set; }

        public string Type { get; set; }
    }
}