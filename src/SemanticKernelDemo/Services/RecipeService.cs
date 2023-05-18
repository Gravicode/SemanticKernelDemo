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
    public class RecipeService
    {
        public string SkillName { get; set; } = "RecipeSkill";
        public string FunctionName { set; get; } = "Recipe";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public RecipeService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.3, double FrequencyPenalty = 0.0f, double
    PresencePenalty= 0.0f, double TopP = 1)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
RECIPE FOR: GRILLED BASIL CHICKEN

INGREDIENTS:

3/4 cup balsamic vinegar

1/4 cup tightly packed fresh basil leaves, orange quote icon gently rub produce under cold running water.

2 tbsp olive oil

garlic clove, minced

1/2 tsp salt

4 plum tomatoes, orange quote icon scrubbed with clean vegetable brush under running water.

4 boneless skinless chicken breast halves (4 ounces each)

DIRECTIONS:

1. Wash hands with soap and water.

2. After washing basil and tomatoes, blot them dry with clean paper towel.

3. Using a clean cutting board, cut tomatoes into quarters.

4. For marinade, place first six ingredients in a blender. Cover and process until well blended.

5. Place chicken breasts in a shallow dish; orange quote icon do not rinse raw poultry. Cover with marinade. Cover dish. Refrigerate about 1 hour, turning occasionally. orange quote icon Wash dish after touching raw poultry.

6. orange quote icon Wash hands with soap and water after handling uncooked chicken.

7. Place chicken on an oiled grill rack over medium heat. orange quote icon Do not reuse marinades used on raw foods. Grill chicken 4-6 minutes per side. orange quote icon Cook until internal temperature reaches 165 °F as measured with a food thermometer.

RECIPE FOR: {{$input}}
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP, FrequencyPenalty=FrequencyPenalty, PresencePenalty=PresencePenalty
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var RecipeFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, RecipeFunction);
        }

        public async Task<string> GenerateRecipe(string FoodName)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                IsProcessing = true;
                var Recipe = await kernel.RunAsync(FoodName, ListFunctions[FunctionName]);

                Console.WriteLine(Recipe);
                Result = Recipe.Result.Trim();
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
