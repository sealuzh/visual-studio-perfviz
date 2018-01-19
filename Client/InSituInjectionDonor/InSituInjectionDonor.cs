using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSituInjectionDonor
{
    public class InSituInjectionDonor
    {
        public static void PreMethodBodyHook()
        {
            System.Diagnostics.Debug.WriteLine("Before Method Body");
            Console.WriteLine("Before Method Body Injection");
        }
        public static void PostMethodBodyHook()
        {
            System.Diagnostics.Debug.WriteLine("After Method Body");
            Console.WriteLine("After Method Body Injection");
        }
    }
}
