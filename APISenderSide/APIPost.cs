using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SendToTopic
{
    public static class APIPost
    {


        const string ServiceBusConnectionString = "Endpoint=sb://servicebuspractice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=44Xtdc5DTF2dnGjGiRYnYAVfyPVCW3ZVM6boz8Ty/gE=";
        const string TopicName = "practicetopic";
        static ITopicClient topicClient;

        [FunctionName("APIPost")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {


            var client = new HttpClient();
            log.LogInformation("C# HTTP trigger function processed a request.");



            string messageBody = await new StreamReader(req.Body).ReadToEndAsync();
            //HttpRequestModel httpRequestModel = JsonConvert.DeserializeObject<HttpRequestModel>(req);

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://jsonplaceholder.typicode.com/posts"),
                Content = new StringContent(messageBody)
            };

            string Myjson = JsonConvert.SerializeObject(httpRequestMessage);

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            var message = new Message(Encoding.UTF8.GetBytes(Myjson));

            await topicClient.SendAsync(message);


            await topicClient.CloseAsync();


            return new OkObjectResult("Done");
        }
    }
}
