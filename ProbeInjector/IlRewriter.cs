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
            var preInitializeMethodReference = MainModule.ImportReference(typeof(AzureTelemetryProbe).GetMethod(nameof(AzureTelemetryProbe.PreMethodBodyHook), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
            var callPreInitializeInstruction = ilProcessor.Create(OpCodes.Call, preInitializeMethodReference);

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

        ///// <summary>
        ///// Waving returns to leaves for try finally...
        ///// https://stackoverflow.com/questions/12769699/mono-cecil-injecting-try-finally
        ///// </summary>
        ///// <param name="methodDefinition"></param>
        ///// <returns></returns>
        //private Instruction FixReturnsToLeave(MethodDefinition methodDefinition)
        //{
        //    if (methodDefinition.ReturnType == TypeSystem.Void)
        //    {
        //        var instructions = methodDefinition.Body.Instructions;
        //        var lastRet = Instruction.Create(OpCodes.Ret);
        //        instructions.Add(lastRet);

        //        for (var index = 0; index < instructions.Count - 1; index++)
        //        {
        //            var instruction = instructions[index];
        //            if (instruction.OpCode == OpCodes.Ret)
        //            {
        //                instructions[index] = Instruction.Create(OpCodes.Leave, lastRet);
        //            }
        //        }
        //        return lastRet;
        //    }
        //    else
        //    {
        //        var instructions = methodDefinition.Body.Instructions;
        //        var returnVariable = new VariableDefinition("methodTimerReturn", methodDefinition.ReturnType);
        //        methodDefinition.Body.Variables.Add(returnVariable);
        //        var lastLd = Instruction.Create(OpCodes.Ldloc, returnVariable);
        //        instructions.Add(lastLd);
        //        instructions.Add(Instruction.Create(OpCodes.Ret));

        //        for (var index = 0; index < instructions.Count - 2; index++)
        //        {
        //            var instruction = instructions[index];
        //            if (instruction.OpCode == OpCodes.Ret)
        //            {
        //                instructions[index] = Instruction.Create(OpCodes.Leave, lastLd);
        //                instructions.Insert(index, Instruction.Create(OpCodes.Stloc, returnVariable));
        //                index++;
        //            }
        //        }
        //        return lastLd;
        //    }
        //}


        //public static void PreMethodBodyHook()
        //{
        //    System.Diagnostics.Debug.WriteLine("Before Method Body");
        //}
        //public static void PostMethodBodyHook()
        //{
        //    System.Diagnostics.Debug.WriteLine("After Method Body");
        //}
    }
}
