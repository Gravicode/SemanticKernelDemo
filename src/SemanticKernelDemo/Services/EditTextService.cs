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
    public class EditTextService
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
        public EditTextService(IOpenAIService service)
        {
            this.openAiService = service;
        }


        public async Task<string> GenerateEditTextAsync(string Content,string Instruction, CancellationToken cancellationToken = default)
        {
            if (IsProcessing) return default;
            try
            {
                IsProcessing = true;
                var completionResult = await openAiService.Edit.CreateEdit(new EditCreateRequest()
                {
                    Input =Content,
                    Instruction = Instruction,
                    Model = "text-davinci-edit-001"
                });
                var Result = string.Empty;
                if (completionResult.Successful)
                {
                    Result = completionResult.Choices.FirstOrDefault()?.Text;
                    Console.WriteLine(completionResult.Choices.FirstOrDefault());
                }
                else
                {
                    if (completionResult.Error == null)
                    {
                        throw new Exception("Unknown Error");
                    }
                    Result = $"{completionResult.Error.Code}: {completionResult.Error.Message}";
                    Console.WriteLine(Result);
                }
                
                return Result;
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
