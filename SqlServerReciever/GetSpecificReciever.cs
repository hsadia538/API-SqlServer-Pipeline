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
    public static class GetSpecificReciever
    {
        [FunctionName("GetSpecificReciever")]
        public static async void Run([ServiceBusTrigger("practicetopic", "sub2", Connection = "MyServiceBus")] string mySbMsg, ILogger log)
        {
            SqlCommand cmd;
            SqlConnection con;
           

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


            var Uid = JsonItem.userId;
            var id = JsonItem.id;
            var title = JsonItem.title.ToString();
            var body = JsonItem.body.ToString();

            // Get the connection made with SQL Server
            con = new SqlConnection(@"Data Source=DESKTOP-N2E41F3;Initial Catalog=Company;Integrated Security=True;MultipleActiveResultSets=True");
            con.Open();

            // Insert the data in SQL tables called Res
            cmd = new SqlCommand("INSERT INTO Res(UserId,Id,Title,Body) VALUES ( @UId,@Id,@Title,@Body)", con);

            //Binding Parameters
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@UId", SqlDbType.Int).Value = Uid;
            cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = title;
            cmd.Parameters.Add("@Body", SqlDbType.VarChar).Value = body;
            cmd.ExecuteNonQuery();
            Console.WriteLine("done");


        }
            
        
    }
}