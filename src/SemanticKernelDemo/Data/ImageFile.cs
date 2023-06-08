using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelDemo.Data {
    public class ImageFile {
        public string FileName { get; set; }
        public string Ext { get; set; }
        public byte[] Content { get; set; }
    }
}
