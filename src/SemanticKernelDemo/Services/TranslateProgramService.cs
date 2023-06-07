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
    public class TranslateProgramService
    {
        public string SkillName { get; set; } = "TranslateProgramSkill";
        public string FunctionName { set; get; } = "TranslateProgram";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public TranslateProgramService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.0, double TopP = 1, double FrequencyPenalty= 0.0f,double PresencePenalty= 0.0f)
        {
            var StopSequences = new List<string>() { "###" };
            
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt =
                 """
    ##### Translate this code from {{$from}} into {{$to}}
    ### {{$from}}
        
        {{$input}}
        
    ### {{$to}}
    """;

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP,
        FrequencyPenalty = FrequencyPenalty, PresencePenalty = PresencePenalty, StopSequences = StopSequences
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var TranslateProgramFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, TranslateProgramFunction);
        }

        public async Task<string> Translate(string FromLang, string ToLang, string Code)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, Code);
                IsProcessing = true;
                var context = new ContextVariables();
                context.Set("from", FromLang);
                context.Set("to", ToLang);
                context.Set("input", Code);
                var TranslateProgram = await kernel.RunAsync(context, ListFunctions[FunctionName]);

                Console.WriteLine(TranslateProgram);
                Result = TranslateProgram.Result.Trim();
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
