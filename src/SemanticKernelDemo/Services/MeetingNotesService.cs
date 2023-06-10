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
    public class MeetingNotesService
    {
        public string SkillName { get; set; } = "MeetingNotesSkill";
        public string[] FunctionNames { set; get; } = new string[] { "NotesSummary", "KeyDecisions", "ActionItems" };
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public MeetingNotesService()
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
            try
            {


                this.MaxTokens = MaxTokens;
                this.Temperature = Temperature;
                this.TopP = TopP;

                string skPrompt = """
{{$input}}

Summarize the meeting notes above.
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

                var MeetingNotesFunction = kernel.RegisterSemanticFunction(SkillName, FunctionNames[0], functionConfig);
                ListFunctions.Add(FunctionNames[0], MeetingNotesFunction);

                skPrompt = """
{{$input}}

identify key decisions in the transcript above.
""";
                var promptTemplate2 = new PromptTemplate(
        skPrompt,                        // Prompt template defined in natural language
        promptConfig,                    // Prompt configuration
        kernel                           // SK instance
    );


                var functionConfig2 = new SemanticFunctionConfig(promptConfig, promptTemplate2);
                var MeetingNotesFunction2 = kernel.RegisterSemanticFunction(SkillName, FunctionNames[1], functionConfig2);
                ListFunctions.Add(FunctionNames[1], MeetingNotesFunction2);

                skPrompt = """
{{$input}}

What are the key action items in the transcript above.
""";
                var promptTemplate3 = new PromptTemplate(
        skPrompt,                        // Prompt template defined in natural language
        promptConfig,                    // Prompt configuration
        kernel                           // SK instance
    );


                var functionConfig3 = new SemanticFunctionConfig(promptConfig, promptTemplate3);
                var MeetingNotesFunction3 = kernel.RegisterSemanticFunction(SkillName, FunctionNames[2], functionConfig3);
                ListFunctions.Add(FunctionNames[2], MeetingNotesFunction3);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<string> GetNotes(string input)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, input);
                IsProcessing = true;

                foreach (var FunctionName in FunctionNames)
                {
                    var MeetingNotes = await kernel.RunAsync(input, ListFunctions[FunctionName]);

                    Console.WriteLine(MeetingNotes);
                    Result += $"[{FunctionName}]\n" + MeetingNotes.Result.Trim() + Environment.NewLine;
                }
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
