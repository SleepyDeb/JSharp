using System.IO;
using JSharp.Helpers;
using JSharp.Package;

namespace JSharp.Package {
    public class JavaPackageResource : JavaPackageElement
    {
        public override string Extension { get; }
        public byte[] Content { get; }

        public JavaPackageResource(string name, JavaPackage parent, Stream resStream, string extension) : base(parent, name, JavaPackageElementTypes.Other) {
            Extension = extension;

            Content = resStream.ReadAllBytes();
        }
    }
}
