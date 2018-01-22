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

            // Loading String of Method Fullname
            var loadStringInstruction = ilProcessor.Create(OpCodes.Ldstr, DocumentationCommentIdDeriver.GetDocumentationCommentId(methodDefinition.FullName));
            ilProcessor.InsertBefore(methodDefinition.Body.Instructions.First(), loadStringInstruction);

            var preMethodBodyHookReference = MainModule.ImportReference(typeof(AzureTelemetryProbe).GetMethod(nameof(AzureTelemetryProbe.PreMethodBodyHook), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
            var callPreInitializeInstruction = ilProcessor.Create(OpCodes.Call, preMethodBodyHookReference);
            ilProcessor.InsertAfter(methodDefinition.Body.Instructions.First(), callPreInitializeInstruction);

            var postMethodBodyHookReference = MainModule.ImportReference(typeof(AzureTelemetryProbe).GetMethod(nameof(AzureTelemetryProbe.PostMethodBodyHook), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
            var callPostMethodHookInstruction = ilProcessor.Create(OpCodes.Call, postMethodBodyHookReference);
            ilProcessor.InsertBefore(methodDefinition.Body.Instructions.Last(), loadStringInstruction);
            ilProcessor.InsertBefore(methodDefinition.Body.Instructions.Last(), callPostMethodHookInstruction);

            return methodDefinition;
        }
    }
}
