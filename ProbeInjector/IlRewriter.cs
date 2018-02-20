using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ProbeInjector.Probe;

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
        public void Inject(IProbeAssembly probeAssembly)
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
        private void Inject(MethodDefinition methodDefinition, IProbeAssembly probeAssembly)
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
                var onAfterMethodInstructions = _instructionBuilder.BuildMethodCall(ilProcessor, probeAssembly.OnAfterMethod, documentationCommentId);
                var returnInstructions = GetReturnInstructions(methodDefinition.Body);

                foreach (var returnInstruction in returnInstructions)
                {
                    ilProcessor.InsertBefore(returnInstruction, onAfterMethodInstructions);
                    ReplaceBranchOperands(ilProcessor, returnInstruction, onAfterMethodInstructions.First());
                }
            }
        }

        private static IList<Instruction> GetReturnInstructions(MethodBody methodBody)
        {
            return methodBody.Instructions.Where(instruction => instruction.OpCode == OpCodes.Ret).ToList();
        }

        /// <summary>
        /// Replacing all branches that jump to the <see cref="originalBranchOperand"/> with branches that jump to the <see cref="replacementBranchOperand"/>
        /// </summary>
        /// <param name="ilProcessor">The ILProcessor with the Body to check</param>
        /// <param name="originalBranchOperand">The original branch operand</param>
        /// <param name="replacementBranchOperand">The original branch operand</param>
        private static void ReplaceBranchOperands(ILProcessor ilProcessor, Instruction originalBranchOperand, Instruction replacementBranchOperand)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var j = 0; j < ilProcessor.Body.Instructions.Count; j++)
            {
                var instruction = ilProcessor.Body.Instructions[j];
                if (IsBranchOpCode(instruction.OpCode) && instruction.Operand == originalBranchOperand)
                {
                    ilProcessor.Replace(instruction, Instruction.Create(instruction.OpCode, replacementBranchOperand));
                }
            }
        }

        /// <summary>
        /// According to https://en.wikipedia.org/wiki/List_of_CIL_instructions
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        private static bool IsBranchOpCode(OpCode opCode)
        {
            return opCode == OpCodes.Br ||
                   opCode == OpCodes.Br_S ||
                   opCode == OpCodes.Beq ||
                   opCode == OpCodes.Beq_S ||
                   opCode == OpCodes.Bge ||
                   opCode == OpCodes.Bge_S ||
                   opCode == OpCodes.Bge_Un ||
                   opCode == OpCodes.Bge_Un_S ||
                   opCode == OpCodes.Bgt ||
                   opCode == OpCodes.Bgt_S ||
                   opCode == OpCodes.Bgt_Un ||
                   opCode == OpCodes.Bgt_Un_S ||
                   opCode == OpCodes.Ble ||
                   opCode == OpCodes.Ble_S ||
                   opCode == OpCodes.Ble_Un ||
                   opCode == OpCodes.Ble_Un_S ||
                   opCode == OpCodes.Blt ||
                   opCode == OpCodes.Blt_S ||
                   opCode == OpCodes.Blt_Un ||
                   opCode == OpCodes.Blt_Un_S ||
                   opCode == OpCodes.Bne_Un ||
                   opCode == OpCodes.Bne_Un_S ||
                   opCode == OpCodes.Br ||
                   opCode == OpCodes.Br_S ||
                   opCode == OpCodes.Brfalse ||
                   opCode == OpCodes.Brfalse_S ||
                   opCode == OpCodes.Brtrue ||
                   opCode == OpCodes.Brtrue_S;
            // Missing: 
            // 	brinst <int32 (target)>
            // brinst.s <int8 (target)>
            // 	brnull <int32 (target)>
            // 	brnull <int32 (target)>
            // brzero <int32 (target)>
            // brzero.s <int8 (target)>
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
