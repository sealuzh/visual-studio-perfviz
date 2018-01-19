using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSituInjectionVictim
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
