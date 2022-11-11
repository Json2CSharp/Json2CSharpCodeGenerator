using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{
	[TestClass]
	public class Test_24_ADD_NULLVALUEHANDLINGIGNORE_SETTING
	{

        [TestMethod]
        public void ShouldNotAddAttribute()
        {
            string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "")  + @"Test_24_ADD_NULLVALUEHANDLINGIGNORE_SETTING_INPUT.txt";
            string resultPath =  Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_24_ADD_NULLVALUEHANDLINGIGNORE_SETTING_OUTPUT2.txt";
            string input = File.ReadAllText(path);
            string errorMessage = string.Empty;
            
            CSharpCodeWriterConfig config = new CSharpCodeWriterConfig();
            config.NullValueHandlingIgnore = true;

            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter(config);
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
        }

        [TestMethod]
		public void NewtonsoftJson()
		{
			string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "")  + @"Test_24_ADD_NULLVALUEHANDLINGIGNORE_SETTING_INPUT.txt";
			string resultPath =  Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_24_ADD_NULLVALUEHANDLINGIGNORE_SETTING_OUTPUT.txt";
			string input = File.ReadAllText(path);
			string errorMessage = string.Empty;

			CSharpCodeWriterConfig config = new CSharpCodeWriterConfig();
            config.NullValueHandlingIgnore = true;
            config.AttributeLibrary = Xamasoft.JsonClassGenerator.Models.JsonLibrary.NewtonsoftJson;
            config.AttributeUsage = Xamasoft.JsonClassGenerator.Models.JsonPropertyAttributeUsage.Always;

            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter(config);
			JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
			jsonClassGenerator.CodeWriter = csharpCodeWriter;
			string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
			string resultsCompare = File.ReadAllText(resultPath);
			Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
		}

        [TestMethod]
        public void NewtonsoftAndSystemTextJson()
        {
            string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "")  + @"Test_24_ADD_NULLVALUEHANDLINGIGNORE_SETTING_INPUT1.txt";
            string resultPath =  Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_24_ADD_NULLVALUEHANDLINGIGNORE_SETTING_OUTPUT1.txt";
            string input = File.ReadAllText(path);
            string errorMessage = string.Empty;

            CSharpCodeWriterConfig config = new CSharpCodeWriterConfig();
            config.NullValueHandlingIgnore = true;
            config.AttributeLibrary = Xamasoft.JsonClassGenerator.Models.JsonLibrary.NewtonsoftAndSystemTextJson;
            config.AttributeUsage = Xamasoft.JsonClassGenerator.Models.JsonPropertyAttributeUsage.Always;

            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter(config);
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
        }
    }
}
