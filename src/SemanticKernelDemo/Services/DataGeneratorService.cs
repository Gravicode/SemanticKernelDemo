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
    public class DataGeneratorService
    {
        public string SkillName { get; set; } = "DataGeneratorSkill";
        public string FunctionName { set; get; } = "DataGenerator";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public DataGeneratorService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.8, double FrequencyPenalty = 0.0f,
    double PresencePenalty= 0.0f, double TopP = 1)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
Generate 3 random data with bootstrap html table from this text:

generate student data with columns student_id, name, class_name, age

result:

<table class="table table-bordered table-hovered">
  <thead>
    <tr>
      <th scope="col">student_id</th>
      <th scope="col">name</th>
      <th scope="col">class_name</th>
      <th scope="col">age</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th scope="row">1234</th>
      <td>Mark</td>
      <td>Class A</td>
      <td>6</td>
    </tr>
    <tr>
      <th scope="row">6576</th>
      <td>Jacob</td>
      <td>Class B</td>
      <td>7</td>
    </tr>
    <tr>
      <th scope="row">7657</th>
      <td>Larry</td>
      <td>Class C</td>
      <td>8</td>
    </tr>
  </tbody>
</table>

Generate {{$number}} random data with bootstrap html table from this text:

{{$input}}

result:

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

            var DataGeneratorFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, DataGeneratorFunction);
        }

        public async Task<string> GenerateData(string input, int number=10)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                IsProcessing = true;
                var context = new ContextVariables();
                context.Set("number", number.ToString());
                context.Set("input", input);
                var DataGenerator = await kernel.RunAsync(context, ListFunctions[FunctionName]);

                Console.WriteLine(DataGenerator);
                Result = DataGenerator.Result.Trim();
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
