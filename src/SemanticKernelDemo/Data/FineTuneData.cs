using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelDemo.Data
{
    public class FineTuneData
    {
        public string Prompt { get; set; }
        public string Completion { get; set; }
    }
    public enum FineTuneCases
    {
        ClassificationSentiment,
        ClassificationYesNo,
        ClassificationNumericalCategory,
        GenerationWriteAds,
        GenerationEntityExtraction,
        GenerationCustomerSupport,
        GenerationProductDesc
    }
}
