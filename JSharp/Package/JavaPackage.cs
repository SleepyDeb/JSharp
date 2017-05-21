using System;
using System.Collections.Generic;
using System.Text;
using JSharp.ByteCode;

namespace JSharp.Package
{
    public class JavaPackage : JavaPackageElement
    {
        public JavaPackageCollection Packages { get; }
        
        public ClassFileCollection Classes { get; }

        public JavaPackageResourceCollection Resources { get; }

        public JavaPackage(string name, JavaPackage parent = null) : base(parent, name, JavaPackageElementTypes.Package) {
            Packages = new JavaPackageCollection();

            Classes = new ClassFileCollection();
            Resources = new JavaPackageResourceCollection();
        }

        public IEnumerable<ClassFile> EnumerateClasses() {
            return EnumerateClasses(this);
        }

        static IEnumerable<ClassFile> EnumerateClasses(JavaPackage package) {
            foreach (var c in package.Classes)
                yield return c;

            foreach (var f in package.Packages)
                foreach(var c in EnumerateClasses(f))
                    yield return c;
        }

        public IEnumerable<JavaPackageResource> EnumerateResources() {
            return EnumerateResources(this);
        }

        static IEnumerable<JavaPackageResource> EnumerateResources(JavaPackage package) {
            var lol = new (int, int);
            foreach (var r in package.Resources)
                yield return r;

            foreach (var f in package.Packages)
                foreach(var r in EnumerateResources(f))
                    yield return r;
        }

        public IEnumerable<JavaPackage> EnumeratePackages() {
            return EnumeratePackages(this);
        }

        static IEnumerable<JavaPackage> EnumeratePackages(JavaPackage package) {
            foreach(var f in package.Packages)
                yield return f;
            
            yield return package;
        }
    }
}
