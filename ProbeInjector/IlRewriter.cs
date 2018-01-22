using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Probe;

namespace ProbeInjector
{
    public class IlRewriter
    {
        private readonly AssemblyDefinition _assemblyDefinition;

        public IlRewriter(AssemblyDefinition assemblyDefinition)
        {
            _assemblyDefinition = assemblyDefinition ?? throw new ArgumentNullException();
        }

        private ModuleDefinition MainModule => _assemblyDefinition.MainModule;

        /// <summary>
        /// Only works if the current class is loaded .... since PreMethodBodyHook etc need to be loaded...
        /// </summary>
        /// <param name="methodDefinition"></param>
        public MethodDefinition HookMethod(MethodDefinition methodDefinition)
        {
            if (methodDefinition.IsConstructor || methodDefinition.IsAbstract || !methodDefinition.IsManaged)
            {
                return null;
            }

            var ilProcessor = methodDefinition.Body.GetILProcessor();
            var preMethodBodyHookReference = MainModule.ImportReference(typeof(AzureTelemetryProbe).GetMethod(nameof(AzureTelemetryProbe.PreMethodBodyHook), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
            var callPreInitializeInstruction = ilProcessor.Create(OpCodes.Call, preMethodBodyHookReference);

            // Pushing the name of the method onto the stack?

            // var pushArgumentInstruction = ilProcessor.Create(OpCodes.Ldarg_1);
            // ilProcessor.InsertBefore(methodDefinition.Body.Instructions.First(), pushArgumentInstruction);
            ilProcessor.InsertBefore(methodDefinition.Body.Instructions.First(), callPreInitializeInstruction);

            // case 1: it has a return?
            // case 2: it has multiple returns?
            // case 3: What if it has exceptions?


            var postMethodBodyHookReference = MainModule.ImportReference(typeof(AzureTelemetryProbe).GetMethod(nameof(AzureTelemetryProbe.PostMethodBodyHook), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
            var callPostMethodHookInstruction = ilProcessor.Create(OpCodes.Call, postMethodBodyHookReference);
            ilProcessor.InsertBefore(methodDefinition.Body.Instructions.Last(), callPostMethodHookInstruction);

            return methodDefinition;
        }
    }
}
