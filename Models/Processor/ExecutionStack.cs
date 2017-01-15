using System.Collections.Generic;

namespace postfix.Models.Processor
{
    public class ExecutionStack
    {
        public Dictionary<string, string> Options { get; set; }
        public List<StackItem> Stack { get; set; }
    }
}