//這個是用來代表一筆支出的 Entity
//這個 Entity 會對應到資料庫中的一個 Table
//這個 Entity 會被 Entity Framework core 用來建立資料庫的 Table
//這個 Entity 會有幾個欄位，分別是 Id, Date, Category, Amount, Remark , description
//id 是一個唯一的識別碼，是這筆支出的編號
//Date 是支出的日期
//Category 是支出的類別
//Amount 是支出的金額
//Remark 是支出的備註
//description 是支出的描述


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseAPI.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public string Description { get; set; } 
    }
}