using System;
using System.Reflection;

namespace ProbeInjector
{

    /// <summary>
    /// Loads a ProbeAssembly and looks for the <see cref="PreMethodExecutionHook"/> and <see cref="PostMethodExecutionHook"/> Methods in the ProbeAssembly Assembly.
    /// </summary>
    internal class ProbeAssembly
    {
        private readonly string _probeFilePath;

        public ProbeAssembly(string probeFilePath)
        {
            _probeFilePath = probeFilePath;
        }

        public bool IsLoaded => PreMethodExecutionHook != null && PostMethodExecutionHook != null;

        public Assembly Assembly { get; private set; }

        public MethodInfo PreMethodExecutionHook { get; private set; }
        public MethodInfo PostMethodExecutionHook { get; private set; }

        public void Load()
        {
            if (IsLoaded)
            {
                return;
            }

            Assembly = Assembly.LoadFrom(_probeFilePath);
            foreach (var probeType in Assembly.GetTypes())
            {
                PreMethodExecutionHook = probeType.GetMethod(nameof(PreMethodExecutionHook));
                PostMethodExecutionHook = probeType.GetMethod(nameof(PostMethodExecutionHook));

                if (IsLoaded)
                {
                    return;
                }
            }

            if (!IsLoaded)
            {
                throw new InvalidOperationException($"Unable to load ProbeAssembly '{_probeFilePath}'. Possibly the Methods '{nameof(PreMethodExecutionHook)}' and '{nameof(PostMethodExecutionHook)}' could not be found.");
            }
        }

    }
}
