
using System;
using System.Data;
using MySql.Data.MySqlClient;
namespace MariaDB_ADO_NET_01;
 
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "server=your_server;database=your_database;uid=your_user;pwd=your_password;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT * FROM your_table", connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine(reader["column1"] + " " + reader["column2"]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
 
