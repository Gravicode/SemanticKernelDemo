// Copyright (c) Microsoft. All rights reserved.

using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.ImageGeneration;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.CustomClient;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Reliability;
using Microsoft.SemanticKernel.Text;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.ObjectModels;
using SemanticKernelDemo.Data;

namespace CustomSemanticKernel;

public class OpenAIImageCustom : IImageCustom
{
    public string Username { get; set; } = "TestUser";
    IOpenAIService openAiService { set; get; }
   
    /// <summary>
    /// Create a new instance of OpenAI image generation service
    /// </summary>
    /// <param name="apiKey">OpenAI API key, see https://platform.openai.com/account/api-keys</param>
    /// <param name="organization">OpenAI organization id. This is usually optional unless your account belongs to multiple organizations.</param>
    /// <param name="handlerFactory">Retry handler</param>
    /// <param name="log">Logger</param>
    public OpenAIImageCustom(
        string apiKey,
        string? organization = null,
        IDelegatingHandlerFactory? handlerFactory = null,
        ILogger? log = null
    )
    {
        Verify.NotEmpty(apiKey, "The OpenAI API key cannot be empty");
        //this.HTTPClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        this.openAiService = new OpenAIService(new OpenAI.OpenAiOptions() { ApiKey = apiKey, Organization = organization });
        if (!string.IsNullOrEmpty(organization))
        {
            this.openAiService = new OpenAIService(new OpenAI.OpenAiOptions() { ApiKey = apiKey, Organization = organization });
            //this.HTTPClient.DefaultRequestHeaders.Add("OpenAI-Organization", organization);
        }
        else
        {
            this.openAiService = new OpenAIService(new OpenAI.OpenAiOptions() { ApiKey = apiKey });
        }
    }

    public async Task<List<ImageModel>> GenerateImageAsync(string Prompt, int NumImages, string ImageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var results = new List<ImageModel>();
            var ErrorMsg = string.Empty;
            var imageResult = await openAiService.Image.CreateImage(new ImageCreateRequest
            {
                Prompt = Prompt,
                N = NumImages,
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
        return default;
    }

    public async Task<List<ImageModel>> GenerateImageEditAsync(byte[] ImageData, string ImageFileName, string Prompt, byte[] MaskData, string MaskFileName, int NumberImages, string ImageSize, CancellationToken cancellationToken = default)
    {
        try
        {
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
        return default;
    }

    public async Task<List<ImageModel>> GenerateImageVariationsAsync(byte[] ImageData, string ImageFileName, int NumberImages, string ImageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var results = new List<ImageModel>();
            var ErrorMsg = string.Empty;
            var imageResult = await openAiService.Image.CreateImageVariation(new ImageVariationCreateRequest
            {
                Image = ImageData,
                ImageName = ImageFileName,
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
        return default;
    }
}
