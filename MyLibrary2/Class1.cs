namespace MyLibrary2;

// 基底類別：商品
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public virtual void DisplayInfo()
    {
        //Console.WriteLine($"商品名稱：{Name}, 價格：{Price}");
        //warning CS8618: 退出建構函式時，不可為 Null 的 屬性 'Name' 必須包含非 Null 值。請考慮將 屬性 宣告為可為 Null。
        Console.WriteLine($"商品名稱:{Name?.ToString() ?? "未命名"},價格:{Price}");
    }
}

// 子類別：書籍
public class Book : Product
{
    public string Author { get; set; }
    public int Pages { get; set; }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"作者：{Author}, 頁數：{Pages}");
    }
}

// 子類別：電子產品
public class Electronic : Product
{
    public string Brand { get; set; }
    public string Model { get; set; }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"品牌：{Brand}, 型號：{Model}");
    }
}

// 訂單類別
public class Order
{
    public List<Product> Products { get; set; } = new List<Product>();

    public void AddProduct(Product product)
    {
        Products.Add(product);
    }

    public void ShowOrder()
    {
        Console.WriteLine("訂單明細：");
        foreach (var product in Products)
        {
            product.DisplayInfo();
        }
    }
}
