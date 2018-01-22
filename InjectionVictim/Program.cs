using System;

namespace InjectionVictim
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("OutputTest1");
            PrintAThing1("OutputTest2");
            Console.ReadLine();
        }

        public static void PrintAThing1(string toPrint)
        {
            Console.WriteLine(toPrint);
        }
    }
}
