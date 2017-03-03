using System.IO;
using System;
using System.Diagnostics;
using JSharp.Helpers;
using JSharp.ByteCode;

namespace JSharp.Package {
    public class JavaArchive : JavaPackage
    {
        public JavaArchive(string jarName, Stream jarStream) : base(jarName)
        {
            using (var jstream = new System.IO.Compression.ZipArchive(jarStream))
            {
                foreach (var entry in jstream.Entries)
                {

                    string fname = entry.FullName;
                    string entryExtension = Path.GetExtension(fname);

                    Debug.WriteLine("Entry: {0}", fname);

                    if(fname[fname.Length - 1] == '/')
                        continue;

                    var ParentPackage = EnsurePackage(GetPath(fname));
                    if(entryExtension == ".class") {
                        using(var classStream = entry.Open()) {
                            var reader = new BigEndianBinaryReader(classStream);
                            var jclass = new ClassFile(ParentPackage, entry.Name, reader);
                            ParentPackage.Classes.Add(jclass);
                        }
                    } else {
                        // Manifest and resources
                        using(var resStream = entry.Open()) {
                            ParentPackage.Resources.Add(new JavaPackageResource(entry.Name, entryExtension, resStream));
                        }
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
