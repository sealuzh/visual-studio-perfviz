using System.Reflection;

namespace ProbeInjector
{

    /// <summary>
    /// Loads a ProbeAssembly and looks for the 
    /// <see cref="OnBeforeMethod"/>, 
    /// <see cref="OnAfterMethod"/> and 
    /// <see cref="OnException"/> 
    /// Methods in the ProbeAssembly Assembly.
    /// </summary>
    internal class ProbeAssembly
    {
        private readonly string _probeFilePath;
        private Assembly _assembly;

        private MethodInfo _onBeforeMethod;
        private MethodInfo _onAfterMethod;
        private MethodInfo _onException;

        public ProbeAssembly(string probeFilePath)
        {
            _probeFilePath = probeFilePath;
        }

        public MethodInfo OnBeforeMethod => LoadAndGet(ref _onBeforeMethod);
        public MethodInfo OnAfterMethod => LoadAndGet(ref _onAfterMethod);
        public MethodInfo OnException => LoadAndGet(ref _onException);

        private MethodInfo LoadAndGet(ref MethodInfo methodInfo)
        {
            if (_assembly == null)
            {
                Load();
            }
            return methodInfo;
        }

        private void Load()
        {
            _assembly = Assembly.LoadFrom(_probeFilePath);
            foreach (var probeType in _assembly.GetTypes())
            {
                _onBeforeMethod = probeType.GetMethod(nameof(OnBeforeMethod));
                _onAfterMethod = probeType.GetMethod(nameof(OnAfterMethod));
                _onException = probeType.GetMethod(nameof(OnException));
            }
        }

    }
}
