using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.SemanticFunctions;
using SemanticKernelDemo.Data;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace SemanticKernelDemo.Services
{
    public class QnAService
    {
        public string SkillName { get; set; } = "QnASkill";
        public string FunctionName { set; get; } = "QnA";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;
        string systemMessage;
        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();

        IKernel kernel { set; get; }
        OpenAIChatHistory chat;
        IChatCompletion chatGPT;
        public bool IsConfigured { get; set; } = false;
        public QnAService()
        {

            // Configure AI backend used by the kernel
            var (model, apiKey, orgId) = AppConstants.GetSettings();

            kernel = new KernelBuilder()
               .WithOpenAIChatCompletionService(modelId: "gpt-3.5-turbo", apiKey: apiKey, orgId: orgId, serviceId: "chat-gpt")
               .Build();
            //SetupSkill();
        }

        public void SetupSkill(string Context = "")
        {
            // Get AI service instance used to manage the user chat
            chatGPT = kernel.GetService<IChatCompletion>();
            systemMessage = string.IsNullOrEmpty(Context) ? "You're chatting with a user. You are an expert of everything. You can answer politely like a professional." : Context;
            chat = (OpenAIChatHistory)chatGPT.CreateNewChat(systemMessage);
            IsConfigured = true;
        }

        public void Reset()
        {
            chat = (OpenAIChatHistory)chatGPT.CreateNewChat(systemMessage);
        }

        public async Task<string> Chat(string userMessage)
        {
            if (!IsConfigured) SetupSkill();

            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                IsProcessing = true;
                //1.Ask the user for a message. The user enters a message.Add the user message into the Chat History object.
                Console.WriteLine($"User: {userMessage}");
                chat.AddUserMessage(userMessage);

                // 2. Send the chat object to AI asking to generate a response. Add the bot message into the Chat History object.
                string assistantReply = await chatGPT.GenerateMessageAsync(chat, new ChatRequestSettings());
                chat.AddAssistantMessage(assistantReply);
                Console.WriteLine(assistantReply);
                Result = assistantReply;
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
}
