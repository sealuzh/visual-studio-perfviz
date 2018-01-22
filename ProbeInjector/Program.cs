#define jeromedebug

using System;
using Mono.Cecil;

namespace ProbeInjector
{
    class Program
    {
        private static string _dllFilePathVictimIn;
        private static string _dllFilePathVictimOut;
        private static AssemblyDefinition _assembly;

        static void Main(string[] args)
        {

#if jeromedebug
            _dllFilePathVictimIn = "C:\\Users\\jerom\\VisualStudioWorkspace\\visual-studio-perfviz\\Client\\InSituInjectionVictim\\bin\\Debug\\InSituInjectionVictim.exe";
            _dllFilePathVictimOut = "C:\\Users\\jerom\\VisualStudioWorkspace\\visual-studio-perfviz\\Client\\InSituInjectionVictim\\bin\\Debug\\InSituInjectionVictim2.exe";
#else
            _dllFilePathVictimIn = "C:\\Users\\jerom\\VisualStudioWorkspace\\MonoCecilTest\\MonoCecilTest\\bin\\Debug\\MonoCecilTest.exe";
            _dllFilePathVictimOut = "C:\\Users\\jerom\\VisualStudioWorkspace\\MonoCecilTest\\MonoCecilTest\\bin\\Debug\\MonoCecilTest2.exe";
#endif

            _assembly = AssemblyDefinition.ReadAssembly(_dllFilePathVictimIn);

            IlRewriter rewriter = new IlRewriter(_assembly);

            for (int i = 0; i < _assembly.MainModule.Types.Count; i++)
            {
                for (int j = 0; j < _assembly.MainModule.Types[i].Methods.Count; j++) 
                {
                    //IsSpecialName not optimal.
                    if (!_assembly.MainModule.Types[i].Methods[j].IsSpecialName)
                    {
                        _assembly.MainModule.Types[i].Methods[j] = rewriter.HookMethod(_assembly.MainModule.Types[i].Methods[j]);
                    }
                }
            }
            

            //TypeDefinition typeDefinition = new TypeDefinition("InSituCodeInject", "IlRewriter", TypeAttributes.Class | TypeAttributes.Public, _assembly.MainModule.ImportReference(typeof(InSituCodeInject.IlRewriter)));
            //MethodDefinition methodDefinition = new MethodDefinition("EmbraceCode", MethodAttributes.Public | MethodAttributes.Static, _assembly.MainModule.TypeSystem.Void);

            //var writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
            //var writeLineRef = _assembly.MainModule.ImportReference(writeLineMethod);

            //Collection<Instruction> methodInstructions = new Collection<Instruction>
            //{
            //    Instruction.Create(OpCodes.Ldstr, "INJECTED!"),
            //    Instruction.Create(OpCodes.Call, writeLineRef),
            //    //Instruction.Create(OpCodes.Pop)
            //};

            //methodDefinition.Body.Instructions.Add(methodInstructions[0]);
            //methodDefinition.Body.Instructions.Add(methodInstructions[1]);
            ////methodDefinition.Body.Instructions.Add(methodInstructions[2]);

            //typeDefinition.Methods.Add(methodDefinition);
            //_assembly.MainModule.Types.Add(typeDefinition);


            //var importedReference = _assembly.MainModule.ImportReference(typeof(IlRewriter));
            //_assembly.MainModule.Types.Add(importedReference.Resolve());


            //_assembly.MainModule.Types.Add(ilRewriterClass.Resolve());



            //_dllFilePathVictimIn = "C:\\Users\\jerom\\VisualStudioWorkspace\\visual-studio-perfviz\\Client\\InSituCodeInject\\resources\\visual-studio-aspnet-test.dll";
            //_dllFilePathVictimOut = "C:\\Users\\jerom\\VisualStudioWorkspace\\visual-studio-perfviz\\Client\\InSituCodeInject\\resources\\visual-studio-aspnet-test2.dll";

            //_assembly = AssemblyDefinition.ReadAssembly(_dllFilePathVictimIn);
            //IlRewriter rewriter = new IlRewriter(_assembly);
            //_assembly.MainModule.Types[12].Methods[0] = rewriter.HookMethod(_assembly.MainModule.Types[12].Methods[0]);








            //_assembly.Modules.Add();





            _assembly.Write(_dllFilePathVictimOut);
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
            _assembly.Write(_dllFilePathVictimIn);
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
