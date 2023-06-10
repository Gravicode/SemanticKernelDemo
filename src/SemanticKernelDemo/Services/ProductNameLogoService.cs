using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.SemanticFunctions;
using SemanticKernelDemo.Data;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.AI.ImageGeneration;

using SemanticKernelDemo.Helpers;

namespace SemanticKernelDemo.Services
{
    public class ProductNameLogoService
    {
        public string SkillName { get; set; } = "ProductNameLogoSkill";
        public string FunctionName { set; get; } = "ProductNameLogo";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public ProductNameLogoService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel = new KernelBuilder()
   .WithOpenAIImageGenerationService(apiKey: apiKey, orgId: orgId, serviceId: "dallE")
   .WithOpenAITextCompletionService(modelId: model, apiKey: apiKey, orgId: orgId, serviceId: "davinci")
   .Build();


            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.2, double TopP = 0.5)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
Generate 5 product names:

Product description: A home milkshake maker
Seed words: fast, healthy, compact.
Product names: HomeShaker, Fit Shaker, QuickShake, Shake Maker

Generate {{$number}} product names:

Product description: {{$desc}}
Seed words: {{$seed}}
Product names: 
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

            var ProductNameLogoFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, ProductNameLogoFunction);
        }

        public async Task<List<ProductInfo>> GenerateNameAndLogo(string desc, string seedword, int numberOfNames = 5)
        {
            var Result = new List<ProductInfo>();
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, desc);
                IsProcessing = true;
                // Get AI service instance used to generate images
                var dallE = kernel.GetService<IImageGeneration>();
                var context = new ContextVariables();
                context.Set("desc", desc);
                context.Set("seed", seedword);
                context.Set("number", numberOfNames.ToString());
                var ProductNameLogo = await kernel.RunAsync(context, ListFunctions[FunctionName]);

                Console.WriteLine(ProductNameLogo);
                var res = ProductNameLogo.Result;
                var names = res.Split(new char[] { ',' });

                foreach (var name in names)
                {
                    var imageUrl = await dallE.GenerateImageAsync($"A 2d, symmetrical, flat logo for {desc} that is {seedword}. it's product name: {name}", 512, 512);
                    Result.Add(new ProductInfo() { ProductName = name.Trim(), ProductLogoUrl = imageUrl });
                }
                return Result;
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

    public class ProductInfo
    {
        public string ProductName { get; set; }
        public string ProductLogoUrl { get; set; }

    }
}
