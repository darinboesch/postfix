using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace postfix.ViewModels
{
    public class ExecStackViewModel
    {
        public Dictionary<string, string> Options { get; set; }

        [Required]
        public List<StackItemViewModel> Stack { get; set; }
    }
}