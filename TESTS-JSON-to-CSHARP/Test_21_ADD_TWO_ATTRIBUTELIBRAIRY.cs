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
	public class Test_21_ADD_TWO_ATTRIBUTELIBRAIRY
	{
		[TestMethod]
		public void Run()
		{
			string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "")  + @"Test_21_ADD_TWO_ATTRIBUTELIBRAIRY_INPUT.txt";
			string resultPath =  Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_21_ADD_TWO_ATTRIBUTELIBRAIRY_OUTPUT.txt";
			string input = File.ReadAllText(path);
			string errorMessage = string.Empty;

            CSharpCodeWriterConfig csharpCodeWriterConfig = new CSharpCodeWriterConfig();
            csharpCodeWriterConfig.AttributeLibrary = JsonLibrary.NewtonsoftAndSystemTextJson;
            csharpCodeWriterConfig.AttributeUsage = JsonPropertyAttributeUsage.Always;
            csharpCodeWriterConfig.UsePascalCase = true;
            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter(csharpCodeWriterConfig);

            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
			string resultsCompare = File.ReadAllText(resultPath);
			Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
		}
	}
}
