using Xunit;
using JSharp.Package;
using System.IO;
using Xunit.Abstractions;
using System.Diagnostics;
using System.Net.Http;
using System;

namespace JSharp.Test {
    public class PackageTest
    {
        /// <summary>
        /// TestAllPackages Test Cases
        /// </summary>
        readonly string[] urls = new string[]{
            "http://central.maven.org/maven2/net/sf/jt400/jt400/9.3/jt400-9.3.jar",
            "http://central.maven.org/maven2/junit/junit/4.12/junit-4.12.jar"
        };

        readonly ITestOutputHelper os;

        public PackageTest(ITestOutputHelper output) {
            os = output;
        }

        [Fact]
        public async void TestAllPackages() {
            using(var hClient = new HttpClient()) {
                foreach(var urlstr in urls) {
                    var packagetUrl = new Uri(urlstr);
                    var packageName = packagetUrl.LocalPath;
                    
                    os.WriteLine("Downloading file from: {0}", packagetUrl);
                    
                    using (var response = await hClient.GetAsync(packagetUrl))
                    using (var content = response.Content)
                    using (var jarStream = await content.ReadAsStreamAsync()) {
                        TestJavaArchiveCTor(packageName, jarStream);
                    }
                }
            }
        }

        public void TestJavaArchiveCTor(string jarName, Stream jarStream) {            
            os.WriteLine("Depacking file: {0}, Size: {1} bytes", jarName, jarStream.Length);

            var sw = new Stopwatch();
            sw.Start();

            var ja = new JavaArchive(jarName, jarStream);

            sw.Stop();

            int pCount = 0;
            foreach (var r in ja.EnumeratePackages())
            {
                os.WriteLine("Depacked Paks: {0}", r);
                pCount++;
            }

            int cCount = 0;
            foreach (var c in ja.EnumerateClasses())
            {
                os.WriteLine("Depacked Class: {0}", c);
                cCount++;
            }

            int rCount = 0;
            foreach (var r in ja.EnumerateResources())
            {
                os.WriteLine("Depacked Res: {0}", r);
                rCount++;
            }

            os.WriteLine("Depacked succefully in: {0}ms, {1} Classes, {2} Resource, {3} Packages ", sw.ElapsedMilliseconds, cCount, rCount, pCount);
        }
    }
}
