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
    public class TabularizeService
    {
        public string SkillName { get; set; } = "TabularizeSkill";
        public string FunctionName { set; get; } = "Tabularize";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public TabularizeService()
        {
            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel = new KernelBuilder()
    .WithOpenAITextCompletionService(modelId: model, apiKey: apiKey, orgId: orgId, serviceId: "davinci")
    .Build();

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.0, double FrequencyPenalty = 0.0f,
    double PresencePenalty = 0.0f, double TopP = 1)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
A csv table summarizing content below:
{{$input}}

""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP, PresencePenalty = PresencePenalty, FrequencyPenalty = FrequencyPenalty
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var TabularizeFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, TabularizeFunction);
        }

        public async Task<string> Tabularize(string content)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, content);
                IsProcessing = true;
                var Tabularize = await kernel.RunAsync(content, ListFunctions[FunctionName]);

                Console.WriteLine(Tabularize);
                Result = Tabularize.Result.Trim();
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
