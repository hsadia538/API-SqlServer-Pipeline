using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SqlServerReciever
{
    public static class DeleteReciever
    {
        [FunctionName("DeleteReciever")]
        public static async void Run([ServiceBusTrigger("practicetopic", "sub5", Connection = "MyServiceBus")]string mySbMsg, ILogger log)
        {
            SqlCommand cmd;
            SqlConnection con;


            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            var client = new HttpClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("https://jsonplaceholder.typicode.com/posts" + mySbMsg)

            };

            //HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            //string responseBody = await response.Content.ReadAsStringAsync();
            //log.LogInformation(responseBody);

            //dynamic JsonItem = JsonConvert.DeserializeObject(mySbMsg);
            //loop through the JsonArray

           int id = Int32.Parse(mySbMsg);

            // Get the connection made with SQL Server
            con = new SqlConnection(@"Data Source=DESKTOP-N2E41F3;Initial Catalog=Company;Integrated Security=True;MultipleActiveResultSets=True");
            con.Open();

            // Insert the data in SQL tables called Res
            cmd = new SqlCommand("Delete from Res Where Id = @id", con);

            //Binding Parameters
            cmd.Parameters.AddWithValue("@id", SqlDbType.Int).Value = id;
            cmd.ExecuteNonQuery();
            Console.WriteLine("done");


        }


    }
}