using Xunit;
using JSharp.Package;
using System.IO;
using Xunit.Abstractions;
using System.Diagnostics;

namespace JSharp.Test {
    public class PackageTest
    {
        string BasePath = @"C:\Develop\NetCore\JSharp\JSharp.Test\bin\Debug\netcoreapp1.1";
        string[] TestFiles = new string[]{ "test.jar" };

        readonly ITestOutputHelper os;

        public PackageTest(ITestOutputHelper output) {
            os = output;
        }

        [Fact]
        public void JSharpCtor()
        {
            var sw = new Stopwatch();

            foreach(var file in TestFiles) {
                os.WriteLine("Depacking file: {0}", file);
                
                using(var js = File.OpenRead(Path.Combine(BasePath, file))) {
                    os.WriteLine("Size: {0} bytes", js.Length);

                    sw.Restart();
                    var ja = new JavaArchive(Path.GetFileName(file), js);
                    sw.Stop();

                    int count = 0;
                    foreach(var c in ja.EnumerateClasses()) {
                        os.WriteLine("Depacked: {0}", c);
                        count++;
                    }
                    os.WriteLine("Depacked succefully in: {0}ms, {1} Classes ", sw.ElapsedMilliseconds, count);
                }
            }
        }
        
    }
}
