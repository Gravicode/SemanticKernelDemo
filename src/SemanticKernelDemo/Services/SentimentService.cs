using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.SemanticFunctions;
using SemanticKernelDemo.Data;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.Maui.Controls.Xaml;

namespace SemanticKernelDemo.Services
{
    public class SentimentService
    {
        public string SkillName { get; set; } = "SentimentSkill";
        public string FunctionName { set; get; } = "Sentiment";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();
        IKernel kernel { set; get; }

        public SentimentService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 120, double Temperature = 0f, double TopP = 1, double FrequencyPenalty = 0.0,double PresencePenalty = 0.0f)
        {
           
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
Classify the sentiment in these sentences:

{{$input}}

Sentence sentiment ratings:
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP, FrequencyPenalty = FrequencyPenalty, PresencePenalty = PresencePenalty
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var SentimentFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, SentimentFunction);
        }

        public async Task<string[]> GetSentiments(string[] inputs)
        {
            var Result = new string[inputs.Length];
            if (IsProcessing) return Result;

            try
            {
                IsProcessing = true;
                var count = 1;
                var prompt = string.Empty;
                foreach(var input in inputs)
                {
                    prompt += $"{count++}. \"{input}\"\n";
                }
                var Sentiment = await kernel.RunAsync(prompt, ListFunctions[FunctionName]);
                var splitted = Sentiment.Result.Split('\n');
                count = 0;
                foreach(var split in splitted)
                {
                    if (split.Length > 0 && !string.IsNullOrEmpty(split))
                        Result[count++] = split; 
                }
                Console.WriteLine(Sentiment);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                IsProcessing = false;
            }
            return Result;
        }

    }
}
