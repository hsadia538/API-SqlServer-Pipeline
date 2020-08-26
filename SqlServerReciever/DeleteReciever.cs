using System;
using System.Net.Http;
using Commons;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SqlServerReciever
{
    public static class DeleteReciever
    {
        [FunctionName("DeleteReciever")]
        public static async void Run([ServiceBusTrigger("practicetopic", "sub5", Connection = "MyServiceBus")]string mySbMsg, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            var client = new HttpClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("https://jsonplaceholder.typicode.com/posts" + mySbMsg)

            };


           int id = Int32.Parse(mySbMsg);

            // Make an object and call the delete val method
            var sqlOb = new Sql();
            sqlOb.DeleteVal(id);
            sqlOb.ConClose();

            Console.WriteLine("done");


        }


    }
}