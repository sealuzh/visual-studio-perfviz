using System;
using System.Diagnostics;
using System.Threading;

namespace InjectionVictim
{
    class Program
    {
        static Program()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("OutputTest1");

            RunTests();

            CallingConventionTest("string1", "string2", 3);

            Console.ReadLine();
        }

        private static void RunTests()
        {
            SimpleMethodTest("OutputTest2");

            MultipleReturnsTest(true);

            LongRunningMethodTest1();

            LongRunningMethods();

            try
            {
                ExceptionsTest(true);
            }
            catch (Exception e)
            {
                Console.Write("Exception Catched:");
                Console.WriteLine(e);
            }

            LongRunningMethodTest1();

            var teststring = ReturnString();
        }

        private static void CallingConventionTest(string a, string b, int c)
        {
            a = b + c + a;
            b = a;
        }

        private static void LongRunningMethods()
        {
            LongRunningMethodTest1();
            LongRunningMethodTest2();
        }

        private static void LongRunningMethodTest1()
        {
            Thread.Sleep(1234);
        }

        private static void LongRunningMethodTest2()
        {
            double x = 0;
            for (int i = 0; i < 1000000; i++)
            {
                x += Math.Pow(121, 123);
                SimpleMethodTest("SimpleMethodTest");
            }
            x = x + 1;
            x = x + 2;
            Thread.Sleep(1234);
        }

        public static void SimpleMethodTest(string toPrint)
        {
            Console.WriteLine(toPrint);
        }

        public static string ReturnString()
        {
            return "somestring";
        }

        /// <summary>
        /// bool argument needed to outsmart optimizer
        /// </summary>
        /// <param name="abool"></param>
        public static void ExceptionsTest(bool abool)
        {
            Console.WriteLine("TestException");
            if (abool)
            {
                throw new Exception("Test");
            }
            Console.WriteLine("NeverReachedCode");
        }

        /// <summary>
        /// bool argument needed to outsmart optimizer
        /// </summary>
        /// <param name="abool"></param>
        public static void MultipleReturnsTest(bool abool)
        {
            Console.WriteLine("MultipleReturnsTest");
            if (abool)
            {
                return;
            }
            Console.WriteLine("NeverReachedCode");
        }
    }
}
