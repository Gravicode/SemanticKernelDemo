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
    public class SummaryService
    {
        public string SkillName { get; set; } = "SummarySkill";
        public string FunctionName { set; get; } = "Summary";
        int MaxTokens { set; get; } 
        double Temperature { set; get; } 
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public SummaryService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens=2000, double Temperature=0.2, double TopP=0.5)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
{{$input}}

Summarize the content above.
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP,
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var summaryFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, summaryFunction);
        }

        public async Task<string> Summarize(string input)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;
                    
            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, input);
                IsProcessing = true;
                var summary = await kernel.RunAsync(input, ListFunctions[FunctionName]);

                Console.WriteLine(summary);
                Result =  summary.Result;
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
