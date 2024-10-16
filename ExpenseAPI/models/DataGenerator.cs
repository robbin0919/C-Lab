//使用ExpenseContext 產生預設資料

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExpenseAPI.Models;

namespace ExpenseAPI.Models
{
    public static class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ExpenseContext(
                serviceProvider.GetRequiredService<DbContextOptions<ExpenseContext>>()))
            {
                // Look for any expenses.
                if (context.Expenses.Any())
                {
                    return;   // Data was already seeded
                }
                //要包含 category與Remark，最後要有 context.SaveChanges();
                context.Expenses.AddRange(
                    new Expense
                    {
                        Date = DateTime.Parse("2021-01-01"),
                        Category = "早餐",
                        Amount = 100,
                        Remark = "好吃",
                        Description = "早餐吃的香蕉"
                    },
                    new Expense
                    {
                        Date = DateTime.Parse("2021-01-01"),
                        Category = "午餐",
                        Amount = 150,
                        Remark = "好吃",
                        Description = "午餐吃的蘋果"
                    },
                    new Expense
                    {
                        Date = DateTime.Parse("2021-01-01"),
                        Category = "晚餐",
                        Amount = 200,
                        Remark = "好吃",
                        Description = "晚餐吃的橘子"
                    },
                    new Expense
                    {
                        Date = DateTime.Parse("2021-01-01"),
                        Category = "宵夜",
                        Amount = 50,
                        Remark = "好吃",
                        Description = "宵夜吃的葡萄"
                    }
                );
                context.SaveChanges();
                
    
            }
        }
    }
}