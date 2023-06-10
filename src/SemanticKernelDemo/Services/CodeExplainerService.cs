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
    public class CodeExplainerService
    {
        public string SkillName { get; set; } = "CodeExplainerSkill";
        public string FunctionName { set; get; } = "CodeExplainer";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public CodeExplainerService()
        {

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel = new KernelBuilder()
     .WithOpenAITextCompletionService(modelId: model, apiKey: apiKey, orgId: orgId, serviceId: "davinci")
     .Build();

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.0, double FrequencyPenalty = 0, double PresencePenalty = 0, double TopP = 0.5)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
{{$input}}

Here's what the above class is doing, explained in a concise way:

1. 
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

            var CodeExplainerFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, CodeExplainerFunction);
        }

        public async Task<string> Explain(string code)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, code);
                IsProcessing = true;
                var CodeExplainer = await kernel.RunAsync(code, ListFunctions[FunctionName]);
                var Res = CodeExplainer.Result;
                if (Res != null)
                {
                    Res = $"1. {Res}";
                }
                Console.WriteLine(Res);
                Result = Res;
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
