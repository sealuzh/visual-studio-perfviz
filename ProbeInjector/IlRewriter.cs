using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ProbeInjector
{
    internal class IlRewriter
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable

        private readonly ModuleDefinition _mainModule;

        public IlRewriter(AssemblyDefinition targetAssemblyDefinition, ProbeAssembly probeAssembly)
        {
            AssemblyDefinition = targetAssemblyDefinition ?? throw new ArgumentNullException(nameof(targetAssemblyDefinition));
            _mainModule = AssemblyDefinition.MainModule ?? throw new ArgumentException("MainModule is null");

            ProbeAssembly = probeAssembly ?? throw  new ArgumentNullException(nameof(probeAssembly));
        }

        public AssemblyDefinition AssemblyDefinition { get; }

        public ProbeAssembly ProbeAssembly { get; }

        /// <summary>
        /// Injecting the Probe into all methods of the Assembyl
        /// </summary>
        public void InjectProbe()
        {
            foreach (var typeDefinition in _mainModule.Types)
            {
                for (var j = 0; j < typeDefinition.Methods.Count; j++)
                {
                    // Replacing MethodDefinition
                    InjectProbe(typeDefinition.Methods[j]);
                }
            }
        }

        /// <summary>
        /// Injecting the Probe into a single method
        /// </summary>
        private MethodDefinition InjectProbe(MethodDefinition methodDefinition)
        {
            if (!IsEligableForInjection(methodDefinition))
            {
                return methodDefinition;
            }

            var ilProcessor = methodDefinition.Body.GetILProcessor();

            // Loading String of Method Fullname
            var loadStringInstruction = ilProcessor.Create(OpCodes.Ldstr, DocumentationCommentIdDeriver.GetDocumentationCommentId(methodDefinition.FullName));
            var arguments = new List<Instruction> {loadStringInstruction};

            InjectHookBefore(ilProcessor, methodDefinition.Body.Instructions.First(), ProbeAssembly.OnBeforeMethod, arguments);
            InjectHookBefore(ilProcessor, methodDefinition.Body.Instructions.Last(), ProbeAssembly.OnAfterMethod, arguments);

            // iterating from last to first, since newly inserted instructions will push existing ones further down towards last
            for (var i = methodDefinition.Body.Instructions.Count -1 ; i > 0 ; i--)
            {
                var instruction = methodDefinition.Body.Instructions[i];
                if (instruction.OpCode == OpCodes.Throw)
                {
                    InjectHookBefore(ilProcessor, instruction, ProbeAssembly.OnException, arguments);
                }
            }

            return methodDefinition;
        }

        /// <summary>
        /// Filtering Methods that aren't eligable for Injection
        /// </summary>
        /// <param name="methodDefinition">the Method to check for eligability</param>
        /// <returns>true if eligable, false if not</returns>
        private static bool IsEligableForInjection(MethodDefinition methodDefinition)
        {
            // .ctor is always qualified with specialname and rtspecialname attribute
            return !methodDefinition.IsConstructor && !methodDefinition.IsSpecialName && !methodDefinition.IsAbstract && methodDefinition.IsManaged && methodDefinition.Body.Instructions.Count > 1;
        }

        /// <summary>
        /// Injection of a HookMethod Before the given Instruction
        /// </summary>
        /// <param name="ilProcessor">The ILProcessor</param>
        /// <param name="instructionAfterHook">The Instruction, before which the Hook should be inserted</param>
        /// <param name="hookMethod">The Hook Method to be injected</param>
        /// <param name="hookArguments">The Arguments to be passed to the HookMethod (load stack)</param>
        private void InjectHookBefore(ILProcessor ilProcessor, Instruction instructionAfterHook, MethodBase hookMethod, IEnumerable<Instruction> hookArguments)
        {
            if (hookMethod == null)
            {
                // do nothing
                return;
            }
            if (ilProcessor == null)
            {
                throw new ArgumentNullException(nameof(ilProcessor));
            }
            if (instructionAfterHook == null)
            {
                throw new ArgumentNullException(nameof(instructionAfterHook));
            }
            if (hookArguments != null)
            {
                foreach (var parameterInstruction in hookArguments)
                {
                    // TODO RR: check IL argument push order... function arguments are pushed on the stack from right to left?
                    ilProcessor.InsertBefore(instructionAfterHook, parameterInstruction);
                }
            }

            var hookMethodReference = _mainModule.ImportReference(hookMethod);
            var callPostMethodHookInstruction = ilProcessor.Create(OpCodes.Call, hookMethodReference);
            ilProcessor.InsertBefore(instructionAfterHook, callPostMethodHookInstruction);
        }
    }
}
