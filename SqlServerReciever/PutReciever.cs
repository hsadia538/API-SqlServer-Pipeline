using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using Commons;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SqlServerReciever
{
    public static class PutReciever
    {
        [FunctionName("PutReciever")]
        public static async void Run([ServiceBusTrigger("practicetopic", "sub4", Connection = "MyServiceBus")]string mySbMsg, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");


            dynamic JsonItem = JsonConvert.DeserializeObject(mySbMsg);
            int id = JsonItem.id;
            string body = JsonItem.body.ToString();

            var client = new HttpClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri("https://jsonplaceholder.typicode.com/posts" + id)

            };


            //we dont need the dummy response so we skip it here as it is not causing any change

            //create an sql object and call Update table method
            var sqlOb = new Sql();
            sqlOb.UpdateTable(id, body);
            sqlOb.ConClose();

            Console.WriteLine("done");


        }


    }
}