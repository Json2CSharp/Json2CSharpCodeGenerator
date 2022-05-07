using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{
	[TestClass]
	public class Test_19_PLURAL_NAMES
	{
		[TestMethod]
		public void Run()
		{
			string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "")  + @"Test_19_PLURAL_NAMES_INPUT.txt";
			string resultPath =  Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_19_PLURAL_NAMES_OUTPUT.txt";
			string input = File.ReadAllText(path);
			string errorMessage = string.Empty;
			CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter();
			JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
			jsonClassGenerator.CodeWriter = csharpCodeWriter;
			string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
			string resultsCompare = File.ReadAllText(resultPath);
			Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
		}
	}
}
