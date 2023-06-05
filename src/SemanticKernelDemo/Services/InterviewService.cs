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
    public class InterviewService
    {
        public string SkillName { get; set; } = "InterviewSkill";
        public string FunctionName { set; get; } = "Interview";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public InterviewService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.5, double FrequencyPenalty = 0.0f,
    double PresencePenalty= 0.0f, double TopP = 1)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
Create a list of {{$number}} questions for my interview with a {{$input}}:
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP, PresencePenalty=PresencePenalty, FrequencyPenalty = FrequencyPenalty
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var InterviewFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, InterviewFunction);
        }

        public async Task<string> GenerateQuestion(int Number,string Profession)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                IsProcessing = true;
                var context = new ContextVariables();
                context.Set("number", Number.ToString());
                context.Set("input", Profession);
                var Interview = await kernel.RunAsync(context, ListFunctions[FunctionName]);

                Console.WriteLine(Interview);
                Result = Interview.Result.Trim();
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
