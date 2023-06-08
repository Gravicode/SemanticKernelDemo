using SemanticKernelDemo.Data;
using System;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace SemanticKernelDemo.Data
{

    public static class JSONLHelper
    {
        public static string ToJSON(this object obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions() { WriteIndented = true });
        }
        
        public static string ToJSON(this List<FineTuneData> obj)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in obj)
                {
                    sb.AppendLine("{ \"prompt\": \"" + item.Prompt + "\", \"completion\": \"" + item.Completion + "\"}");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return string.Empty;
        }


    }


}

