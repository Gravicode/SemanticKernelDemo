using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;

namespace SemanticKernelDemo.Services
{
    public class AudioTranslationService
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
        public AudioTranslationService(IOpenAIService service)
        {
            this.openAiService = service;
        }


        public async Task<string> TranslateAudio(byte[] AudioData, string Filename, CancellationToken cancellationToken = default)
        {
            if (IsProcessing) return default;
            try
            {
                IsProcessing = true;

                var audioResult = await openAiService.Audio.CreateTranslation(new  AudioCreateTranscriptionRequest
                {
                    FileName = Filename,
                    File = AudioData,
                    Model = "whisper-1",
                    ResponseFormat = StaticValues.AudioStatics.ResponseFormat.VerboseJson
                });
                var results = string.Empty;
                if (audioResult.Successful)
                {
                    results = string.Join("\n", audioResult.Text);
                    Console.WriteLine(results);

                }
                else
                {
                    if (audioResult.Error == null)
                    {
                        throw new Exception("Unknown Error");
                    }
                    results = $"{audioResult.Error.Code}: {audioResult.Error.Message}";
                    Console.WriteLine($"{audioResult.Error.Code}: {audioResult.Error.Message}");
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
