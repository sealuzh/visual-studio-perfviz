using System.Reflection;

namespace ProbeInjector.Probe
{
    internal interface IProbeAssembly
    {
        Assembly Assembly { get; }
        MethodInfo OnAfterMethod { get; }
        MethodInfo OnBeforeMethod { get; }
        MethodInfo OnException { get; }
    }
}