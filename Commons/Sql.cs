﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace Commons
{
    public class Sql
    {

        SqlCommand cmd;
        SqlConnection con;
       

        public Sql()
        {


            }

        public void InsertToTable(int Uid, int id, string title,string body)
        {

            //Get the connection made with SQL Server
            con = new SqlConnection(@"Data Source=DESKTOP-N2E41F3;Initial Catalog=Company;Integrated Security=True;MultipleActiveResultSets=True");
            con.Open();

            //Insert the data in SQL tables called Res
            cmd = new SqlCommand("INSERT INTO Res(UserId,Id,Title,Body) VALUES ( @UId,@Id,@Title,@Body)", con);

            //Binding Parameters
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@UId", SqlDbType.Int).Value = Uid;
            cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = title;
            cmd.Parameters.Add("@Body", SqlDbType.VarChar).Value = body;
            cmd.ExecuteNonQuery();
        }

    }
}