using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelDemo.Models
{
    public class ChatInteraction
    {
        public DateTime CreatedDate { get; set; }
        public string UserMessage { get; set; }
        public string AssistantMessage { get; set; }
    }
}
