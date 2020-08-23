using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SendToTopic
{
    public static class SendToTopic
    {


        const string ServiceBusConnectionString = "Endpoint=sb://servicebuspractice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=44Xtdc5DTF2dnGjGiRYnYAVfyPVCW3ZVM6boz8Ty/gE=";
        const string TopicName = "practicetopic";
        static ITopicClient topicClient;

        [FunctionName("SendToTopic")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");



            string messageBody = await new StreamReader(req.Body).ReadToEndAsync();



            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await topicClient.SendAsync(message);


            await topicClient.CloseAsync();


            return new OkObjectResult("Done");
        }
    }
}
