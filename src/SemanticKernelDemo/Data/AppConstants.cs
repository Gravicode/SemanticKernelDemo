using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelDemo.Data
{
    public class AppConstants
    {
        //var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();
        public static string OpenAIApiKey = "";//"-- Open AI Key --";
        public static string OrgID = "";//"-- ORG ID --";
        public static string Model = "text-davinci-003";
        public static string Type = "openai";
        public static (string model, string apiKey, string orgId) GetSettings()
        {
            LoadSettings();
            return (Model,OpenAIApiKey, OrgID);
        }

        public static void SaveSetting()
        {
            Preferences.Set("OpenAIApiKey", OpenAIApiKey);
            Preferences.Set("OrgID", OrgID);
            Preferences.Set("Model", Model);
        }

        public static void LoadSettings()
        {
            Model = Preferences.Get("Model", Model);
            OrgID = Preferences.Get("OrgID", OrgID);
            OpenAIApiKey = Preferences.Get("OpenAIApiKey", OpenAIApiKey);
        }
    }

    
   
}
