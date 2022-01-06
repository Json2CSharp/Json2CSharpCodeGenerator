
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{
    [TestClass]
    public class Test_4_BracketError
    {
        [TestMethod]
        public void Run_1()
        {
            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_4_BracketError_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_4_BracketError_OUTPUT.txt";
            string input      = File.ReadAllText(path);

            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.NormalizeOutput(), returnVal.NormalizeOutput());
        }

        [TestMethod]
        public void Run_2()
        {
            string path1       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_4_BracketError_INPUT_1.txt";
            string resultPath1 = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_4_BracketError_OUTPUT_1.txt";
            string input1      = File.ReadAllText(path1);

            CSharpCodeWriter csharpCodeWriter1 = new CSharpCodeWriter();
            JsonClassGenerator jsonClassGenerator1 = new JsonClassGenerator();
            jsonClassGenerator1.CodeWriter = csharpCodeWriter1;

            string returnVal1 = jsonClassGenerator1.GenerateClasses(input1, out string errorMessage1).ToString();
            string resultsCompare1 = File.ReadAllText(resultPath1);
            Assert.AreEqual(resultsCompare1.NormalizeOutput(), returnVal1.NormalizeOutput());
        }
    }
}
