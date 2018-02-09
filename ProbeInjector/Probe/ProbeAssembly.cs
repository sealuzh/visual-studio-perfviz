using System.Reflection;

namespace ProbeInjector.Probe
{
    internal class ProbeAssembly : IProbeAssembly
    {
        public ProbeAssembly(Assembly assembly, MethodInfo onBeforeMethod, MethodInfo onAfterMethod, MethodInfo onException)
        {
            Assembly = assembly;
            OnBeforeMethod = onBeforeMethod;
            OnAfterMethod = onAfterMethod;
            OnException = onException;
        }

        public Assembly Assembly { get; }
        public MethodInfo OnBeforeMethod { get; }
        public MethodInfo OnAfterMethod { get; }
        public MethodInfo OnException { get; }
    }
}
