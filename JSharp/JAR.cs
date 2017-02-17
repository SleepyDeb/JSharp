using System.IO;
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using JSharpPackage.Class;
using JSharpPackage.Helpers;
using System.Collections.Generic;

namespace JSharpPackage
{
    public class JAR
    {
        private Dictionary<string, ClassFile> files;
        public IDictionary Files => files;
        public JAR(Stream fileStream)
        {
            files = new Dictionary<string, ClassFile>();
            using (var zip = new System.IO.Compression.ZipArchive(fileStream))
            {
                var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("custom")
                {
                    ContentType = AssemblyContentType.Default,
                    Flags = AssemblyNameFlags.None,
                    Version = new Version(1, 1, 1)
                }, AssemblyBuilderAccess.RunAndCollect);

                foreach (var entry in zip.Entries)
                {
                    //Debug.WriteLine("Entry: {0}", entry.FullName);
                    if(Path.GetExtension(entry.FullName) == ".class") {
                        using(var classStream = entry.Open()) {
                            var j = new ClassFile(new BigEndianBinaryReader(classStream));
                            files.Add(entry.FullName, j);
                        }
                    }
                }
            }

        }

        public JAR(byte[] buffer)
        {

        }
    }
}
