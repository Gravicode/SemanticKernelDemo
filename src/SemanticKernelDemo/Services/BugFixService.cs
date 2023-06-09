using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using SemanticKernelDemo.Data;
using Microsoft.SemanticKernel.Orchestration;
using SemanticKernelDemo.Helpers;

namespace SemanticKernelDemo.Services
{
    public class BugFixService
    {
        public string SkillName { get; set; } = "BugFixSkill";
        public string FunctionName { set; get; } = "BugFix";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public BugFixService()
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
/*
//Fix bugs in the code below

//Buggy Code:

{{$input}}
*/

// {{$lang}}
// Fixed code. Does not include the prompt. Apply standard naming convention.
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP, FrequencyPenalty=FrequencyPenalty, PresencePenalty=PresencePenalty, StopSequences = {"/*","*/"}
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var BugFixFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, BugFixFunction);
        }

        public async Task<string> FixBug(string Lang, string Code)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, Code);
                IsProcessing = true; 
                var context = new ContextVariables();
                context.Set("lang", Lang);
                context.Set("input", Code);
                var BugFix = await kernel.RunAsync(context, ListFunctions[FunctionName]);

                Console.WriteLine(BugFix);
                Result = BugFix.Result.Trim();
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
