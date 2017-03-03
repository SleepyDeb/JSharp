using System;
using System.Collections.Generic;
using System.Text;
using JSharp.ByteCode;

namespace JSharp.Package
{
    public class JavaPackage
    {
        public string Name { get; }

        public JavaPackage Parent { get; }
        public JavaPackageCollection Packages { get; }
        
        public ClassFileCollection Classes { get; }
        public JavaResourceCollection Resources { get; }

        public JavaPackage(string name, JavaPackage parent = null) {
            Name = name;
            Parent = parent;

            Packages = new JavaPackageCollection();
            Classes = new ClassFileCollection();
            Resources = new JavaResourceCollection();
        }

        public IEnumerable<ClassFile> EnumerateClasses() {
            return EnumerateClasses(this);
        }

        static IEnumerable<ClassFile> EnumerateClasses(JavaPackage package) {
            foreach(var f in package.Packages)
                foreach(var c in EnumerateClasses(f))
                    yield return c;

            foreach(var c in package.Classes)
                yield return c;
        }

        public override string ToString() {
            if(Parent == null)
                return Name;

            return Parent + "." + Name;
        }
    }
}
