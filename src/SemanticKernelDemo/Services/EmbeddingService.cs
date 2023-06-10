using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;

namespace SemanticKernelDemo.Services
{
    public class EmbeddingService
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
        public EmbeddingService(IOpenAIService service)
        {
            this.openAiService = service;
        }


        public async Task<List<double>> GenerateEmbeddingAsync(string Content, CancellationToken cancellationToken = default)
        {
            if (IsProcessing) return default;
            try
            {
                IsProcessing = true;
              
                var embeddingResult = await openAiService.Embeddings.CreateEmbedding(new EmbeddingCreateRequest()
                {
                    InputAsList = new List<string> { Content },
                    Model = "text-embedding-ada-002"
                });
                var Result = new List<double>(); 
                if (embeddingResult.Successful)
                {
                    Result = embeddingResult.Data.FirstOrDefault()?.Embedding;
                    Console.WriteLine(embeddingResult.Data.FirstOrDefault());
                }
                else
                {
                    if (embeddingResult.Error == null)
                    {
                        throw new Exception("Unknown Error");
                    }
                    
                    Console.WriteLine($"{embeddingResult.Error.Code}: {embeddingResult.Error.Message}");
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
