using System;
using System.Collections.Generic;
using System.IO;
using Oracle.ManagedDataAccess.Client;

namespace TXT2DB;
class Program
{
    static void Main(string[] args)
        {
            string filePath = "TEST.TXT";
            string connectionString = "Your Oracle connection string"; // 請替換為您的 Oracle 連接字串
            string logFilePath = "insert_log.txt";

            try
            {
                // ... (讀取檔案並解析資料的程式碼)

                // 插入資料到資料庫
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        // ... (其他程式碼)

                        foreach (var ticket in tickets)
                        {
                            // 輸出到 console 和 log 檔案
                            Console.WriteLine($"TicketNumber: {ticket.Key}");
                            File.AppendAllText(logFilePath, $"TicketNumber: {ticket.Key}\n");

                            foreach (var column2Value in ticket.Value)
                            {
                                Console.WriteLine($"Column2: {column2Value}");
                                File.AppendAllText(logFilePath, $"Column2: {column2Value}\n");

                                // 插入資料庫
                                command.Parameters[0].Value = ticket.Key;
                                command.Parameters[1].Value = column2Value;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }

                Console.WriteLine("資料插入成功！");
                Console.WriteLine($"詳細資訊請參考: {logFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
                File.AppendAllText(logFilePath, $"Error: {ex.Message}\n");
            }
        }

}
