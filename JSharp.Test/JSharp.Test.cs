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
                    
                    int pCount = 0;
                    foreach(var r in ja.EnumeratePackages()) {
                        os.WriteLine("Depacked Paks: {0}", r);
                        pCount++;
                    }

                    int cCount = 0;
                    foreach(var c in ja.EnumerateClasses()) {
                        os.WriteLine("Depacked Class: {0}", c);
                        cCount++;
                    }

                    int rCount = 0;
                    foreach(var r in ja.EnumerateResources()) {
                        os.WriteLine("Depacked Res: {0}", r);
                        rCount++;
                    }

                    os.WriteLine("Depacked succefully in: {0}ms, {1} Classes, {2} Resource, {3} Packages ", sw.ElapsedMilliseconds, cCount, rCount, pCount);
                }
            }
        }
        
    }
}
