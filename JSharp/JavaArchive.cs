using System.IO;
using System;
using System.Diagnostics;
using JSharp.Helpers;
using JSharp.ByteCode;
using JSharp.Package;

namespace JSharp {
    public class JavaArchive : JavaPackage
    {
        public JavaArchive(string jarName, Stream jarStream) : base(Path.GetFileNameWithoutExtension(jarName))
        {
            using (var jstream = new System.IO.Compression.ZipArchive(jarStream))
            {
                foreach (var entry in jstream.Entries)
                {

                    string name = Path.GetFileNameWithoutExtension(entry.FullName);

                    Debug.WriteLine("Entry: {0}", entry.FullName);
                    
                    var pPackage = EnsurePackage(GetPath(entry.FullName)); // Parent Package
                    
                    var entryType = JavaPackageElementEx.GetTypeFromString(entry.FullName);

                    switch(entryType) {
                        case JavaPackageElementTypes.Class:
                            using(var classStream = entry.Open()) {
                                var reader = new BigEndianBinaryReader(classStream);
                                var jclass = new ClassFile(name, pPackage, reader);
                                pPackage.Classes.Add(jclass);
                            }
                            break;

                        case JavaPackageElementTypes.Other:
                            using(var resStream = entry.Open()) {
                                pPackage.Resources.Add(new JavaPackageResource(name, pPackage, resStream, Path.GetExtension(entry.FullName)));
                            }
                            break;

                        case JavaPackageElementTypes.Manifest:
                            using(var resStream = entry.Open()) {
                                pPackage.Resources.Add(new JavaPackageResource(name, pPackage, resStream, ".MD"));
                            }
                            break;

                        case JavaPackageElementTypes.Package:
                            continue;
                    }
                }
            }
        }

        string GetPath(string fullPath) {
            int li = fullPath.LastIndexOf('/');

            if(li < 0)
                return fullPath;

            return fullPath.Substring(0, li);
        }

        JavaPackage EnsurePackage(string path) {
            JavaPackage cp = this;

            foreach(var token in path.Split('/')) {
                var cp1 = cp.Packages.GetByName(token);
                if(cp1 == null) {
                    cp1 = new JavaPackage(token, cp);
                    cp.Packages.Add(cp1);
                }

                cp = cp1;
            }

            return cp;
        }
    }
}
