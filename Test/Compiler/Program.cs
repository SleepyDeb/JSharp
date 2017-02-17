using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JSharpPackage;
using JSharpPackage.Class;

namespace JSharp.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var pargs = Parameters(args);

            if(!pargs.ContainsKey("jarfile")) {
                Console.WriteLine($"Synthax: {Assembly.GetExecutingAssembly().EntryPoint.Name} /jarfile:file.jar");
                return;
            }

            string jfile = pargs["jarfile"];
            if(File.Exists(jfile) || Path.GetExtension(jfile).Equals(".jar", StringComparison.OrdinalIgnoreCase)) {
                Console.WriteLine("File: {0} Not found or wrong extension.", jfile);
                return;
            }

            var j = new JAR(System.IO.File.Open(jfile, FileMode.Open, FileAccess.Read, FileShare.Read));
            foreach (var cp in (Dictionary<string, ClassFile>)j.Files)
            {
                var name = cp.Key;
                var c = cp.Value;

                foreach (var constant in c.Constants)
                {
                    if(constant==null) continue;
                    
                    switch(constant.Tag) {
                        case ConstantType.Class:
                            ConstantClassInfo classInfo = constant as ConstantClassInfo;
                            Console.WriteLine("Class, {0}", classInfo.Name);
                            break;

                        case ConstantType.Fieldref:
                            ConstantFieldrefInfo fieldrefInfo = constant as ConstantFieldrefInfo;
                            Console.WriteLine("Fieldref, To Class: {0} TypeName: {1}, TypeDescription: {2}", fieldrefInfo.Class.Name, fieldrefInfo.NameAndType.Name, fieldrefInfo.NameAndType.Description);
                            break;

                        case ConstantType.Methodref:
                            ConstantMethodrefInfo methodref = constant as ConstantMethodrefInfo;
                            Console.WriteLine("Methodref, To Class: {0} TypeName: {1}, TypeDescription: {2}", methodref.Class.Name, methodref.NameAndType.Name, methodref.NameAndType.Description);
                            break;

                        case ConstantType.InterfaceMethodref:
                            ConstantInterfaceMethodrefInfo interfaceMethodref = constant as ConstantInterfaceMethodrefInfo;
                            Console.WriteLine("InterfaceMethodref, To Interface: {0} TypeName: {1}, TypeDescription: {2}", interfaceMethodref.Class.Name, interfaceMethodref.NameAndType.Name, interfaceMethodref.NameAndType.Description);
                            break;
                            
                        case ConstantType.MethodHandle:
                            ConstantMethodHandleInfo methodhandle = constant as ConstantMethodHandleInfo;
                            Console.WriteLine("MethodHandle Kind: {0}, {1}, ClassName: {2} Type: {3}, Description: {4}", methodhandle.Reference.Type, methodhandle.ReferenceKind, methodhandle.Reference.Class.Name, methodhandle.Reference.NameAndType.Name, methodhandle.Reference.NameAndType.Description);
                            break;

                        case ConstantType.MethodType:
                            ConstantMethodType methodtype = constant as ConstantMethodType;
                            Console.WriteLine("MethodType, {0}", methodtype.Signature.Value);
                            break;

                        case ConstantType.InvokeDynamic:
                            ConstantInvokeDynamicInfo dynamicInfo = constant as ConstantInvokeDynamicInfo;
                            Console.WriteLine("DynamicInfo, {0}", dynamicInfo.BoostrapMethod.Name);
                            break;                        
                    }
                }

                foreach (var i in c.Interfaces)
                    Console.WriteLine("Interface: {0}", i.Name);

                foreach (var field in c.Fields)
                {
                    Console.WriteLine("Field: {0}, Description: {1}", field.Name, field.Description);
                    foreach (var attribute in field.Attributes)
                        Console.WriteLine("Field Attribute: {0}", attribute.Name);
                }

                foreach (var method in c.Methods) {
                    Console.WriteLine("Method: {0}, Description: {1}", method.Name, method.Description);
                    foreach(var attribute in method.Attributes)
                        Console.WriteLine("Method Attribute: {0}", attribute.Name);
                }

                foreach (var attribute in c.Attributes)
                        Console.WriteLine("Class Attribute: {0}", attribute.Name);
            }            
            Console.ReadLine();
        }

        static Dictionary<string, string> Parameters(string[] args) {
            var pargs = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach(var arg in args) {
                if(!arg.StartsWith("/")) {
                    return pargs;
                }

                int pIndex = arg.IndexOf('=');
                if(pIndex < 0) {
                    pargs.Add(arg.Substring(1), string.Empty);
                } else {
                    pargs.Add(arg.Substring(1, pIndex - 1), arg.Substring(pIndex + 1));
                }
            }
            return pargs;
        }
    }
}
