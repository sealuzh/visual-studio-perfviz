using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ProbeInjector
{
    class UnusedCode
    {

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

        public static void SomeCode()
        {


            TypeDefinition typeDefinition = new TypeDefinition("InSituCodeInject", "IlRewriter", TypeAttributes.Class | TypeAttributes.Public, _assembly.MainModule.ImportReference(typeof(InSituCodeInject.IlRewriter)));
            MethodDefinition methodDefinition = new MethodDefinition("EmbraceCode", MethodAttributes.Public | MethodAttributes.Static, _assembly.MainModule.TypeSystem.Void);

            var writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            var writeLineRef = _assembly.MainModule.ImportReference(writeLineMethod);

            Collection<Instruction> methodInstructions = new Collection<Instruction>
            {
                Instruction.Create(OpCodes.Ldstr, "INJECTED!"),
                Instruction.Create(OpCodes.Call, writeLineRef),
                //Instruction.Create(OpCodes.Pop)
            };

            methodDefinition.Body.Instructions.Add(methodInstructions[0]);
            methodDefinition.Body.Instructions.Add(methodInstructions[1]);
            //methodDefinition.Body.Instructions.Add(methodInstructions[2]);

            typeDefinition.Methods.Add(methodDefinition);
            _assembly.MainModule.Types.Add(typeDefinition);


            var importedReference = _assembly.MainModule.ImportReference(typeof(IlRewriter));
            _assembly.MainModule.Types.Add(importedReference.Resolve());


            _assembly.MainModule.Types.Add(ilRewriterClass.Resolve());



            _dllFilePathVictimIn = "C:\\Users\\jerom\\VisualStudioWorkspace\\visual-studio-perfviz\\Client\\InSituCodeInject\\resources\\visual-studio-aspnet-test.dll";
            _dllFilePathVictimOut = "C:\\Users\\jerom\\VisualStudioWorkspace\\visual-studio-perfviz\\Client\\InSituCodeInject\\resources\\visual-studio-aspnet-test2.dll";

            _assembly = AssemblyDefinition.ReadAssembly(_dllFilePathVictimIn);
            IlRewriter rewriter = new IlRewriter(_assembly);
            _assembly.MainModule.Types[12].Methods[0] = rewriter.HookMethod(_assembly.MainModule.Types[12].Methods[0]);








            _assembly.Modules.Add();

        }

        public static void PrintTypes(string fileName)
        {
            _assembly = AssemblyDefinition.ReadAssembly(fileName);
            CreateClass(_assembly);
            //_assembly = AssemblyDefinition.ReadAssembly(fileName);

            ModuleDefinition module = _assembly.MainModule;

            foreach (TypeDefinition type in module.Types)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    Console.WriteLine(method.FullName);
                }
            }
            Console.ReadLine();

            var processor = method.Body.GetILProcessor();
            var newInstruction = processor.Create(OpCodes.Call, someMethodReference);
            var firstInstruction = method.Body.Instructions[0];

            processor.InsertBefore(firstInstruction, newInstruction);
        }

        public static void CreateClass(AssemblyDefinition assembly)
        {
            //create new class
            TypeDefinition Class = new TypeDefinition(_assembly.Name.Name, "InsightsHandler2", TypeAttributes.Class);
            _assembly.MainModule.Types.Add(Class);
            _assembly.Write(_dllFilePathVictimIn);
            Class.Methods.Add(newMtd);


            //Creating the Console.WriteLine() method and importing it into the target assembly
            // You can use any method you want
            var writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            var writeLineRef = _assembly.MainModule.Import(writeLineMethod);

            //Creating the Process.Start() method and importing it into the target assembly
            var pStartMethod = typeof(Process).GetMethod("Start", new Type[] { typeof(string) });
            var pStartRef = _assembly.MainModule.Import(pStartMethod);

            foreach (var typeDef in _assembly.MainModule.Types) //foreach type in the target assembly
            {
                foreach (var method in typeDef.Methods) //and for each method in it too
                {
                    //Let's push a string using the Ldstr Opcode to the stack
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, "INJECTED!"));

                    //We add the call to the Console.WriteLine() method. It will read from the stack
                    method.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, writeLineRef));

                    //We push the path of the executable you want to run to the stack
                    method.Body.Instructions.Insert(2, Instruction.Create(OpCodes.Ldstr, @"calc.exe"));

                    //Adding the call to the Process.Start() method, It will read from the stack
                    method.Body.Instructions.Insert(3, Instruction.Create(OpCodes.Call, pStartRef));

                    //Removing the value from stack with pop
                    method.Body.Instructions.Insert(4, Instruction.Create(OpCodes.Pop));
                }
            }
            _assembly.Write("12345.exe"); //Now we just save the new assembly and it's ready to go

            Console.WriteLine("> DONE!");
            Console.ReadKey(true);
        }
    }
}
