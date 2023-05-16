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
    public class CSharpCommentService
    {
        public string SkillName { get; set; } = "CSharpCommentSkill";
        public string FunctionName { set; get; } = "CSharpComment";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public CSharpCommentService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.0, double FrequencyPenalty = 0.0f,
   double PresencePenalty= 0.0f, double TopP = 1)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
# C#
{{$input}}

# An elaborate, high quality XML comment for the method above:
///
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP, PresencePenalty = PresencePenalty, FrequencyPenalty=FrequencyPenalty
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var CSharpCommentFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, CSharpCommentFunction);
        }

        public async Task<string> GenerateComment(string Code)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                IsProcessing = true;
                var CSharpComment = await kernel.RunAsync(Code, ListFunctions[FunctionName]);

                Console.WriteLine(CSharpComment);
                Result = CSharpComment.Result;
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
