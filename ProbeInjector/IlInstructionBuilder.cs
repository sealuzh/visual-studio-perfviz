using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ProbeInjector
{
    internal class IlInstructionBuilder
    {

        public IlInstructionBuilder(ModuleDefinition mainModule)
        {
            MainModule = mainModule ?? throw new ArgumentNullException(nameof(mainModule));
        }

        public ModuleDefinition MainModule { get; }

        public IList<Instruction> BuildMethodCall(ILProcessor ilProcessor, MethodBase method, params object[] arguments)
        {
            if (ilProcessor == null)
            {
                throw new ArgumentNullException(nameof(ilProcessor));
            }
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            var instructions = new List<Instruction>();
            foreach (var argument in arguments)
            {
                switch (argument)
                {
                    case string s:
                        var loadStringInstruction = ilProcessor.Create(OpCodes.Ldstr, s);
                        instructions.Add(loadStringInstruction);
                        break;
                    case int i:
                        var loadIntInstruction = ilProcessor.Create(OpCodes.Ldc_I4, i);
                        instructions.Add(loadIntInstruction);
                        break;
                    default:
                        throw new NotImplementedException("Argument Type Unknown");
                }
            }
            return BuildMethodCall(ilProcessor, method, instructions);
        }

        private IList<Instruction> BuildMethodCall(ILProcessor ilProcessor, MethodBase method, IList<Instruction> instructions)
        {
            var hookMethodReference = MainModule.ImportReference(method);
            instructions.Add(ilProcessor.Create(OpCodes.Call, hookMethodReference));
            return instructions;
        }
    }
}
