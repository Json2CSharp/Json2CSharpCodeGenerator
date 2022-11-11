
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.CodeWriters;
using Xamasoft.JsonClassGenerator.Models;

namespace TESTS_JSON_TO_CSHARP
{
    [TestClass]
    public class Test_11_NoListSetter
    {
        [TestMethod]
        public void Run()
        {
            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_11_NoListSetter_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_11_NoListSetter_OUTPUT.txt";
            string input      = File.ReadAllText(path);

            CSharpCodeWriterConfig csharpCodeWriterConfig = new CSharpCodeWriterConfig();
            csharpCodeWriterConfig.AttributeLibrary = JsonLibrary.SystemTextJson;
            csharpCodeWriterConfig.AttributeUsage = JsonPropertyAttributeUsage.Always;
            csharpCodeWriterConfig.UsePascalCase = true;
            csharpCodeWriterConfig.ReadOnlyCollectionProperties = true;
            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter(csharpCodeWriterConfig);

            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator()
            {
                CodeWriter = csharpCodeWriter
            };

            jsonClassGenerator.CodeWriter = csharpCodeWriter;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);

            Assert.AreEqual(expected: resultsCompare.NormalizeOutput(), actual: returnVal.NormalizeOutput());
            Assert.AreEqual(expected: string.Empty, errorMessage);
        }

        [TestMethod]
        public void Run2()
        {
            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_11_NoListSetter_INPUT1.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_11_NoListSetter_OUTPUT1.txt";
            string input      = File.ReadAllText(path);

            CSharpCodeWriterConfig csharpCodeWriterConfig = new CSharpCodeWriterConfig();
            csharpCodeWriterConfig.AttributeLibrary = JsonLibrary.SystemTextJson;
            csharpCodeWriterConfig.AttributeUsage = JsonPropertyAttributeUsage.Always;
            csharpCodeWriterConfig.UsePascalCase = true;
            csharpCodeWriterConfig.ReadOnlyCollectionProperties = true;
            csharpCodeWriterConfig.CollectionType = OutputCollectionType.Array;
            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter(csharpCodeWriterConfig);

            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);

            Assert.AreEqual(expected: resultsCompare.NormalizeOutput(), actual: returnVal.NormalizeOutput());
            Assert.AreEqual(string.Empty, errorMessage);
        }
    }

}
