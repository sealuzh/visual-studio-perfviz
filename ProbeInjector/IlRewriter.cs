using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ProbeInjector
{
    internal class IlRewriter
    {
        private readonly ProbeAssembly _probeAssembly;
        private readonly AssemblyDefinition _assemblyDefinition;

        public IlRewriter(AssemblyDefinition targetAssemblyDefinition, ProbeAssembly probeAssembly)
        {
            _assemblyDefinition = targetAssemblyDefinition ?? throw new ArgumentNullException(nameof(targetAssemblyDefinition));
            _probeAssembly = probeAssembly ?? throw  new ArgumentNullException(nameof(probeAssembly));
        }

        private ModuleDefinition MainModule => _assemblyDefinition.MainModule;

        public void InjectProbe()
        {
            foreach (var typeDefinition in _assemblyDefinition.MainModule.Types)
            {
                for (var j = 0; j < typeDefinition.Methods.Count; j++)
                {
                    //IsSpecialName not optimal.
                    if (!typeDefinition.Methods[j].IsSpecialName)
                    {
                        typeDefinition.Methods[j] = HookMethod(typeDefinition.Methods[j], _probeAssembly.PreMethodExecutionHook, _probeAssembly.PostMethodExecutionHook);
                    }
                }
            }
        }

        /// <summary>
        /// Only works if the current class is loaded .... since PreMethodBodyHook etc need to be loaded...
        /// </summary>
        /// <param name="methodDefinition">The target method to hook</param>
        /// <param name="preExecutionMethodInfo">The method to run before the target method </param>
        /// <param name="postExecutionMethodInfo">The method to run after the target method</param>
        private MethodDefinition HookMethod(MethodDefinition methodDefinition, MethodBase preExecutionMethodInfo, MethodBase postExecutionMethodInfo)
        {
            if (methodDefinition.IsConstructor || methodDefinition.IsAbstract || !methodDefinition.IsManaged)
            {
                return null;
            }

            var ilProcessor = methodDefinition.Body.GetILProcessor();

            // Loading String of Method Fullname
            var loadStringInstruction = ilProcessor.Create(OpCodes.Ldstr, DocumentationCommentIdDeriver.GetDocumentationCommentId(methodDefinition.FullName));
            ilProcessor.InsertBefore(methodDefinition.Body.Instructions.First(), loadStringInstruction);

            var preMethodBodyHookReference = MainModule.ImportReference(preExecutionMethodInfo);
            var callPreInitializeInstruction = ilProcessor.Create(OpCodes.Call, preMethodBodyHookReference);
            ilProcessor.InsertAfter(methodDefinition.Body.Instructions.First(), callPreInitializeInstruction);

            var postMethodBodyHookReference = MainModule.ImportReference(postExecutionMethodInfo);
            var callPostMethodHookInstruction = ilProcessor.Create(OpCodes.Call, postMethodBodyHookReference);
            ilProcessor.InsertBefore(methodDefinition.Body.Instructions.Last(), loadStringInstruction);
            ilProcessor.InsertBefore(methodDefinition.Body.Instructions.Last(), callPostMethodHookInstruction);

            return methodDefinition;
        }
    }
}
