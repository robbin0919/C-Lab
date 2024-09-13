using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;
namespace TXT2DBV8_MariaDB10;
class Program
{
    static void Main(string[] args)
    {
         string filePath = "your_file_path.txt";
        string connectionString = "Server=your_mariadb_server;Database=your_database;Uid=your_user;Pwd=your_password;";

        List<Ticket> tickets = ParseTicketsFromText(filePath);

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new   
 MySqlCommand())
                {
                    command.Connection = connection;
                    command.Comma   
ndText = "INSERT INTO your_table (TicketNumber, Column2, CreateTime, ...) VALUES (@TicketNumber, @Column2, @CreateTime, ...)"; // 請根據您的表格結構調整欄位名稱和資料型態
                    command.Parameters.AddWithValue("@TicketNumber", MySqlDbType.VarChar);
                    command.Parameters.AddWithValue("@Column2", MySqlDbType.VarChar);
                    command.Parameters.AddWithValue("@CreateTime", MySqlDbType.DateTime);
                    // ... 其他參數

                    foreach (var ticket in tickets)
                    {
                        command.Parameters["@TicketNumber"].Value = ticket.TicketNumber;
                        command.Parameters["@Column2"].Value = ticket.Column2;
                        command.Parameters["@CreateTime"].Value = ticket.CreateTime;
                        // ... 其他參數
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("資料庫操作發生錯誤：" + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("發生未知錯誤：" + ex.Message);
        }
    }
        static List<Ticket> ParseTicketsFromText(string filePath)
        {
            List<Ticket> tickets = new List<Ticket>();
            string ticketNumber = "";
            string ticketStatus = "";
            List<string> ticketData = new List<string>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#####TICKET"))
                    {
                        if (!string.IsNullOrEmpty(ticketNumber))
                        {
                            tickets.Add(new Ticket
                            {
                                TicketNumber = ticketNumber,
                                Status = ticketStatus,
                                Data = ticketData
                            });
                        }
                        ticketNumber = line.Substring(7);
                        ticketStatus = "";
                        ticketData.Clear();
                    }
                    else
                    {
                        ticketData.Add(line);
                    }
                }

                // 处理最后一张票券
                if (!string.IsNullOrEmpty(ticketNumber))
                {
                    tickets.Add(new Ticket
                    {
                        TicketNumber = ticketNumber,
                        Status = ticketStatus,
                        Data = ticketData
                    });
                }
            }
            return tickets;
        } 

        class Ticket
        {
            public string TicketNumber { get; set; }
            public string Status { get; set; }
            public List<string> Data { get; set; }
        }

}
