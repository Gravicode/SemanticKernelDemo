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
using CustomSemanticKernel;
using SemanticKernelDemo.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Reliability;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.ResponseModels;

namespace SemanticKernelDemo.Services
{
    public class ImageEditService
    {
        public string Username { get; set; } = "TestUser";
        IOpenAIService openAiService { set; get; }
        public bool IsProcessing { get; set; } = false;
        /// <summary>
        /// Create a new instance of OpenAI image generation service
        /// </summary>
        /// <param name="apiKey">OpenAI API key, see https://platform.openai.com/account/api-keys</param>
        /// <param name="organization">OpenAI organization id. This is usually optional unless your account belongs to multiple organizations.</param>
        /// <param name="handlerFactory">Retry handler</param>
        /// <param name="log">Logger</param>
        public ImageEditService(IOpenAIService service)
        {
            this.openAiService = service;
        }

      
        public async Task<List<ImageModel>> GenerateImageEditAsync(byte[] ImageData, string ImageFileName, string Prompt, byte[] MaskData, string MaskFileName, int NumberImages, string ImageSize, CancellationToken cancellationToken = default)
        {
            if (IsProcessing) return default;
            try
            {
                IsProcessing = true;
                var results = new List<ImageModel>();
                var ErrorMsg = string.Empty;
                var imageResult = await openAiService.Image.CreateImageEdit(new ImageEditCreateRequest
                {
                    Image = ImageData,
                    ImageName = ImageFileName,
                    Prompt = Prompt,
                    Mask = MaskData,
                    MaskName = MaskFileName,
                    N = NumberImages,
                    Size = ImageSize,
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    User = Username
                });


                if (imageResult.Successful)
                {

                    Console.WriteLine(string.Join("\n", imageResult.Results.Select(r => r.Url)));
                    var count = 1;
                    results.AddRange(imageResult.Results.Select(x => new ImageModel() { Url = x.Url, Title = $"Image-{count++}" }));
                }
                else
                {
                    var res = imageResult.Error.Message;
                    Console.WriteLine(res);
                    ErrorMsg = res;
                    //results.Add(res);
                }
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Snackbar.Add($"failed to generate image: {ex}", Severity.Error);
            }
            finally
            {
                IsProcessing = false;
            }
            return default;
        }

    }
}
