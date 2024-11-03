//這個是用來代表一筆支出的 Entity
//這個 Entity 會對應到資料庫中的一個 Table
//這個 Entity 也會被 Entity Framework core 用來建立資料庫的 Table
// Entity Framework core 會根據這個 Entity 來建立 Table
// 這個 Entity 也會被 Entity Framework core 用來查詢、新增、修改、刪除資料
// 這個 Entity 有一些欄位，分別是 Id, Date, Category, Amount, Remark
// Id 是一個 int，代表這筆支出的 Id，這個 Id 是唯一的
// Date 是一個 DateTime，代表這筆支出的日期
// Category 是一個 string，代表這筆支出的類別
// Amount 是一個 decimal，代表這筆支出的金額
// Remark 是一個 string，代表這筆支出的備註


using System;
using System.ComponentModel.DataAnnotations;

namespace MyBudget.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Display(Name = "日期")]
        [Required(ErrorMessage = "請輸入日期")]
        public DateTime Date { get; set; }

        [Display(Name = "類別")]
        [Required(ErrorMessage = "請輸入類別")]
        public string? Category { get; set; }

        [Display(Name = "金額")]
        [Required(ErrorMessage = "請輸入金額")]
        [Range(1, 1000000, ErrorMessage = "金額必須介於 1 到 1,000,000 之間")]
        public decimal Amount { get; set; }

        [Display(Name = "備註")]
        public string?  Remark  { get; set; }
    }
}

 