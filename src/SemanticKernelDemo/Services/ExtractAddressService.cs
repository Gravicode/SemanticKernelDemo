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
    public class ExtractAddressService
    {
        public string SkillName { get; set; } = "ExtractAddressSkill";
        public string FunctionName { set; get; } = "ExtractAddress";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public ExtractAddressService()
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
Extract adddress entities from this text as JSON:

Text: "130 W BOSE ST STE 100, PARK RIDGE, IL, 60068, USA" JSON Output: { "BUILDING_NO": "130", "STREET_NAME" : "W BOSE ST", "CITY" : "PARK RIDGE", "STATE" : "IL", "ZIP_CODE" : "60068", "COUNTRY" : "USA" }

Text: "8311 MCDONALD RD, HOUSTON, TX, 77053-4821, USA" JSON Output: { "BUILDING_NO": "8311", "STREET_NAME" : "MCDONALD RD", "CITY" : "HOUSTON", "STATE" : "TX", "ZIP_CODE" : "77053-4821", "COUNTRY" : "USA" }

Text: "{{$input}}" JSON Output: 
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP, FrequencyPenalty =   FrequencyPenalty, PresencePenalty = PresencePenalty
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var ExtractAddressFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, ExtractAddressFunction);
        }

        public async Task<string> ExtractAddress(string input)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, input);
                IsProcessing = true;
                var ExtractAddress = await kernel.RunAsync(input, ListFunctions[FunctionName]);

                Console.WriteLine(ExtractAddress);
                Result = ExtractAddress.Result;
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
