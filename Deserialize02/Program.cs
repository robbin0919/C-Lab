namespace Deserialize02;

using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class JsonHelper
{
    public static List<Product> DeserializeAndValidate(string jsonString)
    {
        // 定義正規表達式，用於驗證產品名稱
        string nameRegex = @"^[a-zA-Z\s]+$";

        // 反序列化
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = false
        };
        List<Product> products = JsonSerializer.Deserialize<List<Product>>(jsonString, options);

        // 驗證每個產品的名稱
        for (int i = 0; i < products.Count; i++)
        {
            if (!Regex.IsMatch(products[i].Name, nameRegex))
            {
                // 如果名稱不符合規則，則移除該產品
                products.RemoveAt(i);
                i--; // 因為移除了一個元素，所以索引要減一
            }
        }

        return products;
    }
}

class Program
{
    static void Main(string[] args)
    {        // 建立多筆產品資料
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

        // 反序列化並驗證
        List<Product> validatedProducts = JsonHelper.DeserializeAndValidate(jsonString);

        // 輸出驗證後的結果
        foreach (var product in validatedProducts)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
        }
    }
}
