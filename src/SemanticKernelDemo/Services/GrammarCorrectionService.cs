using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.SemanticFunctions;
using SemanticKernelDemo.Data;
using Microsoft.SemanticKernel.Orchestration;

namespace SemanticKernelDemo.Services
{
    public class GrammarCorrectionService
    {
        public string SkillName { get; set; } = "GrammarCorrectionSkill";
        public string FunctionName { set; get; } = "GrammarCorrection";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public GrammarCorrectionService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.0, double TopP = 1, double FrequencyPenalty = 0.0f, double PresencePenalty = 0.0f)
        {


            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt =
                 """
    Correct this to standard {{$lang}}:

    {{$input}}

    """;

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP,
        FrequencyPenalty = FrequencyPenalty, PresencePenalty = PresencePenalty
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var GrammarCorrectionFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, GrammarCorrectionFunction);
        }

        public async Task<string> CorrectGrammar(string Language, string Content)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                IsProcessing = true;
                var context = new ContextVariables();
                context.Set("lang", Language);
                context.Set("input", Content);
                var GrammarCorrection = await kernel.RunAsync(context, ListFunctions[FunctionName]);

                Console.WriteLine(GrammarCorrection);
                Result = GrammarCorrection.Result.Trim();
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
