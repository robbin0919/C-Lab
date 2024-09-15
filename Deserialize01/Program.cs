namespace Deserialize01;

using System.Text.Json;
using System.Collections.Generic;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

class Program
{
    static void Main(string[] args   
)
    {
        // 建立多筆產品資料
        List<Product> products = new List<Product>
        {
            new Product { Id = 1, Name = "Apple", Price = 1.99m },
            new Product { Id = 2, Name = "Banana", Price = 0.99m },
            new Product { Id = 3, Name = "Orange", Price = 1.50m }
        };

        // 序列化為 JSON 字串，並設定安全選項
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // 格式化輸出
            IgnoreNullValues = true, // 忽略 null 值
            PropertyNameCaseInsensitive = false // 不允許屬性名稱大小寫不敏感
        };
        string jsonString = JsonSerializer.Serialize(products, options);
        Console.WriteLine(jsonString);

        // 反序列化，並設定安全選項
        List<Product> deserializedProducts = JsonSerializer.Deserialize<List<Product>>(jsonString, options);

        // 輸出反序列化結果
        foreach (var product in deserializedProducts)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
        }
    }
}