using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.SemanticFunctions;
using SemanticKernelDemo.Data;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.AI.ImageGeneration;
using SemanticKernelDemo.Helpers;

namespace SemanticKernelDemo.Services
{
    public class ArtisticImageService
    {
        public string SkillName { get; set; } = "ArtisticImageSkill";
        public string FunctionName { set; get; } = "ArtisticImageGenerator";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;

        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }

        public ArtisticImageService()
        {

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel = new KernelBuilder()
    .WithOpenAIImageGenerationService(apiKey: apiKey, orgId: orgId, serviceId: "dallE")
    .WithOpenAITextCompletionService(modelId: "text-davinci-003", apiKey: apiKey, orgId: orgId, serviceId: "davinci")
    .Build();

            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 300, double Temperature = 1, double TopP = 0.5)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;


            // Create a semantic function that generate a random image description.
            var skPrompt = "Describe the artistic image with one detailed sentence about {{$input}}. ";


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

            var ArtisticImageFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, ArtisticImageFunction);
        }

        public async Task<string> GenerateArtisticImage(string input)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                // Get AI service instance used to generate images
                var dallE = kernel.GetService<IImageGeneration>();


                IsProcessing = true;
                //var ArtisticImageDesc = await kernel.RunAsync(input, ListFunctions[FunctionName]);


                // Use DALL-E 2 to generate an image. OpenAI in this case returns a URL (though you can ask to return a base64 image)
                var imageUrl = await dallE.GenerateImageAsync(input, 512, 512);

                Console.WriteLine(imageUrl);
                Result = imageUrl;
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
        public async Task<string> GeneratePoem(string input)
        {
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, input);
                IsProcessing = true;
                var ArtisticImageDesc = await kernel.RunAsync(input, ListFunctions[FunctionName]);

                Console.WriteLine(ArtisticImageDesc);
                Result = ArtisticImageDesc.Result;
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
