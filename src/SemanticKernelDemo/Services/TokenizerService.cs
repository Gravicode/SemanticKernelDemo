using OpenAI.Interfaces;
using OpenAI.Tokenizer.GPT3;

namespace SemanticKernelDemo.Services
{
    public class TokenizerService
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
        public TokenizerService(IOpenAIService service)
        {
            this.openAiService = service;
        }


        public async Task<(IEnumerable<int> Tokens, int Count)> GetTokens(string Content, CancellationToken cancellationToken = default)
        {
            if (IsProcessing) return default;
            try
            {
                IsProcessing = true;

                var encodedList = TokenizerGpt3.Encode(Content);
                var tokenCount = TokenizerGpt3.TokenCount(Content);
                Console.WriteLine($"tokenz: {encodedList}, count: {tokenCount}");
                return (encodedList, tokenCount);

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
