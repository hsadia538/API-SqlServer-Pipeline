using System;
using System.Net.Http;
using System.Text;
using Commons;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace SqlServerReciever
{
    public static class GetSpecificReciever
    {

        
    
        [FunctionName("GetSpecificReciever")]
        public static void Run([ServiceBusTrigger("practicetopic", "sub2", Connection = "MyServiceBus")] Message message, ILogger log, MessageReceiver messageReceiver)
        {
            try
            {

                string mySbMsg = Encoding.UTF8.GetString(message.Body);
               
              
               

                log.LogInformation($"C# ServiceBus topic trigger function processed message: {message}");


                var client = new HttpClient();
                var httpRequestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://jsonplaceholder.typicode.com/" + mySbMsg),

                };

                HttpResponseMessage response = client.SendAsync(httpRequestMessage).Result;

                string responseBody = response.Content.ReadAsStringAsync().Result;
                log.LogInformation(responseBody);

                dynamic JsonItem = JsonConvert.DeserializeObject(responseBody);

                //Getting all the attributes to be inserted in the sql table
                int Uid = JsonItem.userId;
                int id = JsonItem.id;
                string title = JsonItem.title.ToString();
                string body = JsonItem.body.ToString();

                //Making object and calling the method for inserting the row in the table
                var sqlOb = new Sql();
                sqlOb.InsertToTable(Uid, id, title, body);
                sqlOb.ConClose();
                messageReceiver.DeadLetterAsync(message.SystemProperties.LockToken);
               

                Console.WriteLine("done");
            }
            catch(Exception e)
            {
                throw;
            }

        }



    }
}