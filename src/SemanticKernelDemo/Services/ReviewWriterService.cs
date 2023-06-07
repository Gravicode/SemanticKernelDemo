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
    public class ReviewWriterService
    {
        public string SkillName { get; set; } = "ReviewWriterSkill";
        public string FunctionName { set; get; } = "ReviewWriter";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public ReviewWriterService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.5, double TopP = 1, double FrequencyPenalty = 0.0f,
    double PresencePenalty= 0.0f)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
Write a {{$object}} review based on these notes:

Name: {{$name}}
{{$features}}

Review:
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP, PresencePenalty = PresencePenalty, FrequencyPenalty=FrequencyPenalty,
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var ReviewWriterFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, ReviewWriterFunction);
        }

        public async Task<string> CreateReview(string name,string objectName, string features)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, $"{name} {objectName} {features}");
                IsProcessing = true;
                var context = new ContextVariables();
                context.Set("object", objectName);
                context.Set("name", name);
                context.Set("features", features);
                var ReviewWriter = await kernel.RunAsync(context, ListFunctions[FunctionName]);

                Console.WriteLine(ReviewWriter);
                Result = ReviewWriter.Result;
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
