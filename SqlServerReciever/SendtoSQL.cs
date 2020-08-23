using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Data.Sql;
using Newtonsoft.Json.Linq;
using System.Data;

namespace RecieveFromTopic
{
    public static class SendtoSQL
    {
        
        [FunctionName("SendtoSQL")]
        public static void Run([ServiceBusTrigger("practicetopic", "sub1", Connection = "MyServiceBus")] string mySbMsg,
            [CosmosDB(
            databaseName:"Company",
            collectionName:"Employee",
            ConnectionStringSetting = "myCosmosDB")]out dynamic document, ILogger log)
        {
            SqlCommand cmd;
            SqlConnection con;
            SqlDataAdapter da;

            var myJson = mySbMsg;
            var Jobj = JObject.Parse(myJson);
            var id = Jobj["Id"].ToString();
            var name = Jobj["Name"].ToString();
            var code = Jobj["Code"].ToString();
            var comp = Jobj["Competency"].ToString();


            document = new { id=id,Name=name,Code=code,Competency=comp };
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");


            con = new SqlConnection(@"Data Source=DESKTOP-N2E41F3;Initial Catalog=Company;Integrated Security=True;MultipleActiveResultSets=True");
            con.Open();
            Console.WriteLine("Connection Open ! ");
            cmd = new SqlCommand("INSERT INTO Employee(Id,Name,Code,Competency) VALUES (@Id, @Name,@Code,@Competency)", con);
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value=id;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name ;
            cmd.Parameters.Add("@Code", SqlDbType.VarChar).Value = code;
            cmd.Parameters.Add("@Competency", SqlDbType.VarChar).Value = comp;
            cmd.ExecuteNonQuery();

        }
    }
}
