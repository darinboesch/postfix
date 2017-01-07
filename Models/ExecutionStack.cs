using System.Collections.Generic;

namespace postfix.Models
{
    public class ExecutionStack
    {
        public Dictionary<string, string> Options { get; set; }
        public List<StackItem> Stack { get; set; }
    }
}