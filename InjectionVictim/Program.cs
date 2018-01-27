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
                // Catchall
            }
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
            }
            x = x + 1;
            x = x + 2;
            Thread.Sleep(1234);
        }

        public static void SimpleMethodTest(string toPrint)
        {
            Console.WriteLine(toPrint);
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
