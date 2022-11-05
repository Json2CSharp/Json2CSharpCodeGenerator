using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{
	[TestClass]
	public class Test_22_SUPPORT_FOR_NULLABLES
	{
		[TestMethod]
		public void Run()
		{

            Movie movie= new Movie();
            movie.Name = "Bad Boys III";
            movie.Description = "It's no Bad Boys";
            string included = JsonConvert.SerializeObject(movie, Formatting.Indented, new JsonSerializerSettings { });
            string ignored = JsonConvert.SerializeObject(movie, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            Movie movie2 = JsonConvert.DeserializeObject<Movie>(included, new JsonSerializerSettings { });
            Movie movie3 = JsonConvert.DeserializeObject<Movie>(ignored, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            /*
            string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "")  + @"Test_22_SUPPORT_FOR_NULLABLES_INPUT.txt";
			string resultPath =  Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_22_SUPPORT_FOR_NULLABLES_OUTPUT.txt";
			string input = File.ReadAllText(path);
			string errorMessage = string.Empty;
			CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter();
			JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
			jsonClassGenerator.CodeWriter = csharpCodeWriter;
			string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
			string resultsCompare = File.ReadAllText(resultPath);
			Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
		    */

        }


        public class Movie
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Classification { get; set; }
            public string Studio { get; set; }
            public DateTime? ReleaseDate { get; set; }
            public List<string> ReleaseCountries { get; set; }
        }


    }
}
