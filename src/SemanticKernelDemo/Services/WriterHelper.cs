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
    public class WriterHelperService
    {
        public string SkillName { get; set; } = "WriterHelperSkill";
        public string FunctionName { set; get; } = "WriteOutline";
        public string FunctionName2 { set; get; } = "WriteContent";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public WriterHelperService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 3000, double Temperature = 0.3, double FrequencyPenalty = 0.0f,
    double PresencePenalty= 0.0f, double TopP = 1)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
Create an outline for a book about {{$input}}:
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

            var WriterHelperFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, WriterHelperFunction);

            string skPrompt2 = """
{{$input}}
Write an essay from outline above:

""";

            

            var promptTemplate2 = new PromptTemplate(
    skPrompt2,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig2 = new SemanticFunctionConfig(promptConfig, promptTemplate2);

            var WriterHelperFunction2 = kernel.RegisterSemanticFunction(SkillName, FunctionName2, functionConfig2);
            ListFunctions.Add(FunctionName2, WriterHelperFunction2);
        }

        public async Task<string> CreateOutline(string Topic)
        {
            TokenHelper.CheckMaxToken(this.MaxTokens, Topic);
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                IsProcessing = true;
                var WriterHelper = await kernel.RunAsync(Topic, ListFunctions[FunctionName]);

                Console.WriteLine(WriterHelper);
                Result = WriterHelper.Result.Trim();
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
        public async Task<string> GenerateContent(string Outline)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, Outline);
                IsProcessing = true;
                var WriterHelper = await kernel.RunAsync(Outline, ListFunctions[FunctionName2]);

                Console.WriteLine(WriterHelper);
                Result = WriterHelper.Result.Trim();
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
