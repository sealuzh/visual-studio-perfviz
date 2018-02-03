using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ProbeInjector
{
    internal class IlRewriter
    {
        private readonly IlInstructionBuilder _instructionBuilder;

        public IlRewriter(AssemblyDefinition targetAssemblyDefinition)
        {
            AssemblyDefinition = targetAssemblyDefinition ?? throw new ArgumentNullException(nameof(targetAssemblyDefinition));
            _instructionBuilder = new IlInstructionBuilder(AssemblyDefinition.MainModule);
        }

        public AssemblyDefinition AssemblyDefinition { get; }

        /// <summary>
        /// Injecting the Probe into all methods of the Assembly
        /// </summary>
        public void Inject(ProbeAssembly probeAssembly)
        {
            if (probeAssembly == null)
            {
                throw new ArgumentNullException(nameof(probeAssembly));
            }

            foreach (var typeDefinition in AssemblyDefinition.MainModule.Types)
            {
                foreach (var methodDefinition in typeDefinition.Methods)
                {
                    Inject(methodDefinition, probeAssembly);
                }
            }
        }

        /// <summary>
        /// Injecting the Probe into a single method
        /// </summary>
        private void Inject(MethodDefinition methodDefinition, ProbeAssembly probeAssembly)
        {
            if (!IsEligableForInjection(methodDefinition))
            {
                return;
            }

            var ilProcessor = methodDefinition.Body.GetILProcessor();

            // Loading String of Method Fullname
            var documentationCommentId = DocumentationCommentIdDeriver.GetDocumentationCommentId(methodDefinition.FullName);

            // Exceptions
            if (probeAssembly.OnException != null)
            {
                var onExceptionInstructions = _instructionBuilder.BuildMethodCall(ilProcessor, probeAssembly.OnException, documentationCommentId);
                // iterating from last to first, since newly inserted instructions will push existing ones further down towards last
                for (var i = methodDefinition.Body.Instructions.Count - 1; i > 0; i--)
                {
                    var instruction = methodDefinition.Body.Instructions[i];
                    if (instruction.OpCode == OpCodes.Throw)
                    {
                        ilProcessor.InsertBefore(instruction, onExceptionInstructions);
                    }
                }
            }

            // BeforeMethod
            if (probeAssembly.OnBeforeMethod != null)
            {
                ilProcessor.InsertBefore(methodDefinition.Body.Instructions.First(), _instructionBuilder.BuildMethodCall(ilProcessor, probeAssembly.OnBeforeMethod, documentationCommentId));
            }

            // AfterMethod
            if (probeAssembly.OnAfterMethod != null)
            {
                ilProcessor.InsertBefore(methodDefinition.Body.Instructions.Last(), _instructionBuilder.BuildMethodCall(ilProcessor, probeAssembly.OnAfterMethod, documentationCommentId));
            }
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

    }
}
