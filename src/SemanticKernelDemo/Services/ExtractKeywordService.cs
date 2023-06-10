using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.SemanticFunctions;
using SemanticKernelDemo.Data;
using Microsoft.SemanticKernel.Orchestration;
using SemanticKernelDemo.Helpers;

namespace SemanticKernelDemo.Services
{
    public class ExtractKeywordService
    {
        public string SkillName { get; set; } = "ExtractKeywordSkill";
        public string FunctionName { set; get; } = "ExtractKeyword";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public ExtractKeywordService()
        {
            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel = new KernelBuilder()
                .WithOpenAITextCompletionService(modelId: model, apiKey: apiKey, orgId: orgId, serviceId: "davinci")
                .Build();

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.5, double FrequencyPenalty = 0.8f,
    double PresencePenalty = 0.0f)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;


            string skPrompt = """
Extract keywords from this text:

{{$input}}
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        PresencePenalty = PresencePenalty, FrequencyPenalty = FrequencyPenalty
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var ExtractKeywordFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, ExtractKeywordFunction);
        }

        public async Task<string> ExtractKeywords(string input)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, input);
                IsProcessing = true;
                var ExtractKeyword = await kernel.RunAsync(input, ListFunctions[FunctionName]);

                Console.WriteLine(ExtractKeyword);
                Result = ExtractKeyword.Result.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ex.ToString();
            }
            finally
            {
                IsProcessing = false;
            }
            return Result;
        }

    }
}
