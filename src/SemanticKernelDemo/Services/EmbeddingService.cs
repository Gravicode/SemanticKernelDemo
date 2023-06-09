﻿using OpenAI.Interfaces;
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

        public double GetCosineSimilarity(List<double> V1, List<double> V2)
        {
            int N = 0;
            N = ((V2.Count < V1.Count) ? V2.Count : V1.Count);
            double dot = 0.0d;
            double mag1 = 0.0d;
            double mag2 = 0.0d;
            for (int n = 0; n < N; n++)
            {
                dot += V1[n] * V2[n];
                mag1 += Math.Pow(V1[n], 2);
                mag2 += Math.Pow(V2[n], 2);
            }

            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
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
