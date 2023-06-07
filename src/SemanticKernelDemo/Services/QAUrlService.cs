using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using SemanticKernelDemo.Data;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Memory;
using HtmlAgilityPack;
using CommunityToolkit.Maui.Core;
using System.Threading;
using CommunityToolkit.Maui.Alerts;

using SemanticKernelDemo.Helpers;

namespace SemanticKernelDemo.Services
{
    public class QAUrlService
    {
        public string SkillName { get; set; } = "QAUrlSkill";
        public string FunctionName { set; get; } = "QAUrl";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;
        const string COLLECTION = "PageCollection";
        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();
        HashSet<string> Ids = new HashSet<string>();
        IKernel kernel { set; get; }
        public int ContentCount { get; internal set; } = 0;
        public QAUrlService()
        {
            kernel = KernelBuilder.Create();

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();
            kernel.Config.AddOpenAITextEmbeddingGenerationService("embedding", "text-embedding-ada-002", apiKey, orgId);
            kernel.Config.AddOpenAITextCompletionService("davinci", model, apiKey, orgId);
           

            kernel.UseMemory(new VolatileMemoryStore());
            SetupSkill();
        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.2, double TopP = 0.5)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
Use the following pieces of context to answer the question at the end. If you don't know the answer, don't try to make up an answer and answer with 'I don't know'. Answer in the langauge that used for the question.

{{$context}}

Question: {{$input}}
Answer:
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

            var QAUrlFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, QAUrlFunction);
        }
        public async Task<string> Answer(string question)
        {
            if (ContentCount <= 0) throw new Exception("Please add content at least 1 page url");
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, question);
                IsProcessing = true;

                var results = await kernel.Memory.SearchAsync(COLLECTION, question, limit: 2).ToListAsync();
               
                var context = new ContextVariables();
                var ctx = results.Any()
                        ? string.Join("\n", results.Select(r => r.Metadata.Text))
                        : "No context found for this question.";
                context.Set("context", ctx);
                context.Set("input", question);

                var result = await kernel.RunAsync(context, ListFunctions[FunctionName]);
                if (result.ErrorOccurred) throw new Exception(result.LastErrorDescription);
                Result = result.Result;
                
            }
            catch (Exception ex)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
               
                string text = ex.ToString();
                ToastDuration duration = ToastDuration.Short;
                double fontSize = 14;
                var toast = Toast.Make(text, duration, fontSize);

                await toast.Show(cancellationTokenSource.Token);
                Console.WriteLine(ex);
                return ex.ToString();
            }
            finally
            {
                IsProcessing = false;
            }
            return Result;

        }

        

        public async Task AddPageUrl(HttpClient client, string url, string contentSelector)
        {
            var content = await client.GetStringAsync(url);
            var title = string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var mainElement = doc.DocumentNode.SelectSingleNode(contentSelector);
            title = mainElement.SelectSingleNode("//h1").InnerText;
            content = mainElement.InnerText;

            await kernel.Memory.SaveInformationAsync(COLLECTION, content, url, title);
            Ids.Add(url);
            ContentCount++;
        }

        public async Task AddContent(string title, string content)
        {
            ContentCount++;
            var url = $"doc-{title}-page-{ContentCount}";
           
            await kernel.Memory.SaveInformationAsync(COLLECTION, content, url, title);
            
        }

        public async Task Reset()
        {
            if (ContentCount <= 0) return;
            foreach(var url in Ids)
            {
                await kernel.Memory.RemoveAsync(COLLECTION, url);
            }
            ContentCount = 0;
        }
    }
}
