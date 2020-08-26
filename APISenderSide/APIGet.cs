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
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace APISenderSide
{
    public static class APIGet
    {
        
        const string ServiceBusConnectionString = "Endpoint=sb://servicebuspractice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=44Xtdc5DTF2dnGjGiRYnYAVfyPVCW3ZVM6boz8Ty/gE=";
        const string TopicName = "practicetopic";
        static ITopicClient topicClient;

        [FunctionName("APIGet")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ExecutionContext context,
            ILogger log)
        {
            TelemetryClient telemetry = new TelemetryClient();
            telemetry.InstrumentationKey = System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
            string requestname = req.ToString();
            var time = DateTime.Now;
            var sw = Stopwatch.StartNew();
            telemetry.Context.Operation.Id = Guid.NewGuid().ToString();



            //TelemetryConfiguration.Active.ConfigurationKey = "66f3f1f9-ad96-458f-8de8-378deffd1c38";
            log.LogInformation("C# HTTP trigger function processed a request.");

            string messageBody = await new StreamReader(req.Body).ReadToEndAsync();
            var message = new Message(Encoding.UTF8.GetBytes(messageBody))
            {

                CorrelationId = "1"
            };

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);
            await topicClient.SendAsync(message);
            await topicClient.CloseAsync();


            telemetry.TrackRequest(requestname, time, sw.Elapsed, "200", true);
            return new OkObjectResult("Done");
        }
    }
}
