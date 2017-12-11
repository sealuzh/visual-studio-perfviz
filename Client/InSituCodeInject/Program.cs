using System;
using System.Diagnostics;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;


namespace InSituCodeInject
{
    class Program
    {
        private static string _dllFilePath;
        private static AssemblyDefinition _assembly;

        static void Main(string[] args)
        {
            _dllFilePath = "C:\\Users\\jerom\\VisualStudioWorkspace\\visual-studio-perfviz\\Client\\InSituCodeInject\\resources\\VSIX-InSituVisualization.dll";
            _dllFilePath = "C:\\Users\\jerom\\VisualStudioWorkspace\\visual-studio-perfviz\\Client\\InSituCodeInject\\resources\\visual-studio-aspnet-test.dll";
            PrintTypes(_dllFilePath);
        }

        public static void PrintTypes(string fileName)
        {
            _assembly = AssemblyDefinition.ReadAssembly(fileName);
            CreateClass(_assembly);
            _assembly = AssemblyDefinition.ReadAssembly(fileName);
            
            ModuleDefinition module = _assembly.MainModule;

            foreach (TypeDefinition type in module.Types)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    Console.WriteLine(method.FullName);
                }
            }
            Console.ReadLine();

            //var processor = method.Body.GetILProcessor();
            //var newInstruction = processor.Create(OpCodes.Call, someMethodReference);
            //var firstInstruction = method.Body.Instructions[0];

            //processor.InsertBefore(firstInstruction, newInstruction);
        }

        public static void CreateClass(AssemblyDefinition assembly)
        {
            //create new class
            TypeDefinition Class = new TypeDefinition(_assembly.Name.Name, "InsightsHandler2", TypeAttributes.Class);
            _assembly.MainModule.Types.Add(Class);
            _assembly.Write(_dllFilePath);
            //Class.Methods.Add(newMtd);


            //Creating the Console.WriteLine() method and importing it into the target assembly
            //You can use any method you want
            //var writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            //var writeLineRef = _assembly.MainModule.Import(writeLineMethod);

            ////Creating the Process.Start() method and importing it into the target assembly
            //var pStartMethod = typeof(Process).GetMethod("Start", new Type[] { typeof(string) });
            //var pStartRef = _assembly.MainModule.Import(pStartMethod);

            //foreach (var typeDef in _assembly.MainModule.Types) //foreach type in the target assembly
            //{
            //    foreach (var method in typeDef.Methods) //and for each method in it too
            //    {
            //        //Let's push a string using the Ldstr Opcode to the stack
            //        method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, "INJECTED!"));

            //        //We add the call to the Console.WriteLine() method. It will read from the stack
            //        method.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, writeLineRef));

            //        //We push the path of the executable you want to run to the stack
            //        method.Body.Instructions.Insert(2, Instruction.Create(OpCodes.Ldstr, @"calc.exe"));

            //        //Adding the call to the Process.Start() method, It will read from the stack
            //        method.Body.Instructions.Insert(3, Instruction.Create(OpCodes.Call, pStartRef));

            //        //Removing the value from stack with pop
            //        method.Body.Instructions.Insert(4, Instruction.Create(OpCodes.Pop));
            //    }
            //}
            //_assembly.Write("12345.exe"); //Now we just save the new assembly and it's ready to go

            //Console.WriteLine("> DONE!");
            //Console.ReadKey(true);
        }
    }
}
