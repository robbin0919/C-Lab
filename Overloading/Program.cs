using OverloadingLib;
namespace Overloading;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Circle circle = new Circle { Radius = 5 };
        Rectangle rectangle = new Rectangle { Length = 4, Width = 3 };
        Triangle triangle = new Triangle { Base = 6, Height = 8 };

        Console.WriteLine("圓形面積：" + circle.CalculateArea());   
        Console.WriteLine("矩形面積：" + rectangle.CalculateArea());
        Console.WriteLine("三角形面積：" + triangle.CalculateArea());
    }
}
