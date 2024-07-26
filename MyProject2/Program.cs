using MyLibrary2;
//namespace MyProject2;
namespace AnotherProject;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        // 建立商品物件
        Book book = new Book { Name = "C#入門", Price = 300, Author = "張三", Pages = 300 };
        Electronic phone = new Electronic { Name = "iPhone 14", Price = 30000, Brand = "Apple", Model = "iPhone14" };

        // 建立訂單物件並添加商品
        Order order = new Order();
        order.AddProduct(book);
        order.AddProduct(phone);

        // 顯示訂單明細
        order.ShowOrder();
    }
}
