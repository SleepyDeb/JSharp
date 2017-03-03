using System.IO;
using JSharp.Helpers;

namespace JSharp {
    public class JavaPackageResource
    {
        public string Name { get; }
        public string Extension { get; }
        public byte[] Buffer { get; }

        public JavaPackageResource(string name, string extension, Stream resStream) {
            Name = name;
            Extension = extension;

            Buffer = resStream.ReadAllBytes();
        }
    }
}
