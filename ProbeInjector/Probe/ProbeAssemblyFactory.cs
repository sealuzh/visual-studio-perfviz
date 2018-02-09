using System.Reflection;

namespace ProbeInjector.Probe
{
    /// <summary>
    /// Loads a ProbeAssembly with the Methods
    /// <see cref="IProbeAssembly.OnBeforeMethod"/>, 
    /// <see cref="IProbeAssembly.OnAfterMethod"/> and 
    /// <see cref="IProbeAssembly.OnException"/> 
    /// </summary>
    internal static class ProbeAssemblyFactory
    {
        public static IProbeAssembly LoadAssembly(string probeFilePath)
        {
            var assembly = Assembly.LoadFrom(probeFilePath);
            MethodInfo onBeforeMethod = null;
            MethodInfo onAfterMethod = null;
            MethodInfo onException = null;
            foreach (var probeType in assembly.GetTypes())
            {
                onBeforeMethod = probeType.GetMethod(nameof(IProbeAssembly.OnBeforeMethod));
                onAfterMethod = probeType.GetMethod(nameof(IProbeAssembly.OnAfterMethod));
                onException = probeType.GetMethod(nameof(IProbeAssembly.OnException));
            }

            return new ProbeAssembly(assembly, onBeforeMethod, onAfterMethod, onException);
        }
    }
}
