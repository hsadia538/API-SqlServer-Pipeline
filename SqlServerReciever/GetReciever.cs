using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using Commons;

namespace RecieveFromTopic
{
    public static class GetReciever
    {

        [FunctionName("GetReciever")]
        public static async void Run([ServiceBusTrigger("practicetopic", "sub1", Connection = "MyServiceBus")] string mySbMsg, ILogger log)
        {

            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            var client = new HttpClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://jsonplaceholder.typicode.com/" + mySbMsg),

            };

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            string responseBody = await response.Content.ReadAsStringAsync();
            log.LogInformation(responseBody);

            dynamic JsonItem = JsonConvert.DeserializeObject(responseBody);

            //loop through the JsonArray  
            foreach (var s in JsonItem)
            {
                
                int Uid = s.userId;
                int id = s.id;
                string title = s.title.ToString();
               string body = s.body.ToString();

                //Make and sql object for insertion
                var sqlOb = new Sql();
                sqlOb.InsertToTable(Uid, id, title, body);
                sqlOb.ConClose();
            }
            Console.WriteLine("done");
        }
    }
}