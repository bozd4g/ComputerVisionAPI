using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ComputerVisionAPI.Provider
{
    public class ServiceManager
    {
        // Constructor
        public ServiceManager()
        {
        }

        public async Task<string> Request(Stream stream)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress =
                        new Uri("https://westus.api.cognitive.microsoft.com/vision/v1.0/describe?maxCandidates=1");
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{ApiKeyWillBeHere}");
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/octet-stream"));

                    HttpContent content = new StreamContent(stream);
                    content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

                    var response = await client.PostAsync(
                        "https://westus.api.cognitive.microsoft.com/vision/v1.0/describe?maxCandidates=1", content);
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return "Task failed.\nDetails: " + ex.Message;
            }
        }
    }
}