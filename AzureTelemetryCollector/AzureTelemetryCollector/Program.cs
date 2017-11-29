using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureTelemetryCollector.TelemetryCollector;

namespace AzureTelemetryCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            
            AzureTelemetryStore store = AzureTelemetryFactory.GetInstance();
            while(true)
            {
                System.Threading.Thread.Sleep(1000);
                Dictionary<String, TimeSpan> dict = new Dictionary<String,TimeSpan>(store.GetAverageMemberTelemetry());
                if (dict != null)
                {
                    foreach (TimeSpan timeSpan in dict.Values)
                    {
                        Console.WriteLine(timeSpan);
                    }
                }
            }

        }
    }
}
