using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using System.Net.Http;
using System.Text;

namespace APISenderSide
{
    public static class APIGetSpecific
    {
        const string ServiceBusConnectionString = "Endpoint=sb://servicebuspractice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=44Xtdc5DTF2dnGjGiRYnYAVfyPVCW3ZVM6boz8Ty/gE=";
        const string TopicName = "practicetopic";
        static ITopicClient topicClient;

        [FunctionName("APIGetSpecific")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {


            log.LogInformation("C# HTTP trigger function processed a request.");

            string messageBody = await new StreamReader(req.Body).ReadToEndAsync();
            var message = new Message(Encoding.UTF8.GetBytes(messageBody)) {
                
                CorrelationId="2"
            };
            
            

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);
            await topicClient.SendAsync(message);
            await topicClient.CloseAsync();


            return new OkObjectResult("Done");
        }
    }
}

