using System;
using MyLibrary; // 引入 MyLibrary 命名空間
namespace MyProject;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Calculator calculator = new Calculator();
        int result = calculator.Add(5, 3);
         Console.WriteLine(result);
    }
}
