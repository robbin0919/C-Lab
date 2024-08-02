using System;
using System.Collections.Generic;
using System.IO;
using Oracle.ManagedDataAccess.Client;
// using System.Linq;
namespace TXT2DBV8;
public class Ticket
{
    public string TicketNumber { get; set; }
    public List<string> Column2Values { get; set; } = new List<string>();
}

class Program
{
    static void Main(string[] args)
    {
        string filePath = "TEST.TXT";
        string connectionString = "Your Oracle connection string"; // 請替換為您的 Oracle 連接字串
        string logFilePath = "insert_log.txt";

        List<Ticket> tickets = new List<Ticket>();
        Ticket currentTicket = null;

        try
        {
            // 讀取檔案並解析資料
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#####"))
                    {
                        currentTicket = new Ticket { TicketNumber = line.Substring(5) };
                        tickets.Add(currentTicket);
                    }
                    else if (currentTicket != null)
                    {
                        currentTicket.Column2Values.Add(line);
                    }
                }
            }

            // 輸出所有 ticket 資訊到 console 和 log 檔案，方便檢查
            Console.WriteLine("以下為要插入的資料：");
            File.AppendAllText(logFilePath, "以下為要插入的資料：\n");
            foreach (var ticket in tickets)
            {
                Console.WriteLine($"TicketNumber: {ticket.TicketNumber}");
                File.AppendAllText(logFilePath, $"TicketNumber: {ticket.TicketNumber}\n");
                foreach (var column2Value in ticket.Column2Values)
                {
                    Console.WriteLine($"Column2: {column2Value}");
                    File.AppendAllText(logFilePath, $"Column2: {column2Value}\n");
                }
                Console.WriteLine();
                File.AppendAllText(logFilePath, "\n");
            }

            // 確認是否繼續插入
            Console.Write("是否繼續插入資料到資料庫？(Y/N): ");
            if (Console.ReadLine().ToUpper() != "Y")
            {
                Console.WriteLine("取消插入。");
                return;
            }

            // 插入資料到資料庫
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO ITGIT (TicketNumber, Column2) VALUES (:TicketNumber, :Column2)";
                    command.Parameters.Add(":TicketNumber", OracleDbType.Varchar2);
                    command.Parameters.Add(":Column2", OracleDbType.Varchar2);

                    foreach (var ticket in tickets)
                    {
                        foreach (var column2Value in ticket.Column2Values)
                        {
                            command.Parameters[0].Value = ticket.TicketNumber;
                            command.Parameters[1].Value = column2Value;
                            command.ExecuteNonQuery();
                        }
                    }
                }

                Console.WriteLine("資料插入成功！");
                Console.WriteLine($"詳細資訊請參考: {logFilePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"發生錯誤: {ex.Message}");
            File.AppendAllText(logFilePath, $"Error: {ex.Message}\n");
        }
    }
}
