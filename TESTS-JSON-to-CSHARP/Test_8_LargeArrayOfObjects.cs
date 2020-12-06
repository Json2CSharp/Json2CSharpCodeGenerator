
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{
    [TestClass]
    public class Test_8_LargeArrayOfObjects
    {
        [TestMethod]
        public void Run()
        {
            string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_8_LargeArrayOfObjects_INPUT.txt"; 
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_8_LargeArrayOfObjects_OUTPUT.txt";
            string input = File.ReadAllText(path);
            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator
            {
                CodeWriter = csharpCodeWriter
            };
            string errorMessage;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            watch.Stop();

            var seconds = watch.ElapsedMilliseconds / 1000;
            Assert.IsTrue(false);

            string resultsCompare = File.ReadAllText(resultPath);
            /*Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), 
                              returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));*/
        }
    }
}
