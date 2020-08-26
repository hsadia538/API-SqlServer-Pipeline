using System;
using System.Net.Http;
using Commons;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SqlServerReciever
{
    public static class PostReciever
    {
        [FunctionName("PostReciever")]
        public static async void Run([ServiceBusTrigger("practicetopic", "sub3", Connection = "MyServiceBus")]string mySbMsg, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            var client = new HttpClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://jsonplaceholder.typicode.com/posts"),
                Content = new StringContent(mySbMsg)

            };
           

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            string responseBody = await response.Content.ReadAsStringAsync();
            log.LogInformation(responseBody);

            //Getting the Id from recieved response
            dynamic JsonItem = JsonConvert.DeserializeObject(responseBody);
            var id = JsonItem.id;

            //Inserting a new entry in the table
            var sqlOb = new Sql();
            sqlOb.InsertToTable(id);
            sqlOb.ConClose() ;

            Console.WriteLine("done");

        }
    }
}
