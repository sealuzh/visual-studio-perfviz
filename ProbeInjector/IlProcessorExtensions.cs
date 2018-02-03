using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace ProbeInjector
{
    internal static class IlProcessorExtensions
    {
        public static void InsertBefore(this ILProcessor ilProcessor, Instruction target, params Instruction[] instructions)
        {
            InsertBefore(ilProcessor, target, instructions.ToList());
        }

        public static void InsertBefore(this ILProcessor ilProcessor, Instruction target, IList<Instruction> instructions)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (instructions == null)
            {
                throw new ArgumentNullException(nameof(instructions));
            }
            foreach (var instruction in instructions)
            {
                ilProcessor.InsertBefore(target, instruction);
            }
        }

    }
}
