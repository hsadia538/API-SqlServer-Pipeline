using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
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
            SqlCommand cmd;
            SqlConnection con;


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
            //HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            //string responseBody = await response.Content.ReadAsStringAsync();
            //log.LogInformation(responseBody);



            // Get the connection made with SQL Server
            con = new SqlConnection(@"Data Source=DESKTOP-N2E41F3;Initial Catalog=Company;Integrated Security=True;MultipleActiveResultSets=True");
            con.Open();

            // Insert the data in SQL tables called Res
            cmd = new SqlCommand("Update Res SET body=@body WHERE id=@id", con);

            //Binding Parameters
            cmd.Parameters.AddWithValue("@id", SqlDbType.Int).Value = id;
            cmd.Parameters.AddWithValue("@body", SqlDbType.NVarChar).Value = body;
            cmd.ExecuteNonQuery();
            Console.WriteLine("done");


        }


    }
}