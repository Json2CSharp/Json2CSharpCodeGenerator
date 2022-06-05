using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectInitializerGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests
{
    public class Base
    {
        public void Run(string testName, Dictionary<string,string> settings = null)
        {
            string input = File.ReadAllText(Directory.GetCurrentDirectory().Replace("bin\\Debug\\netcoreapp3.1", "\\") + testName + "\\" + testName + "_INPUT" + ".txt");
            string expected = File.ReadAllText(Directory.GetCurrentDirectory().Replace("bin\\Debug\\netcoreapp3.1", "") + testName + "\\" + testName + "_OUTPUT" + ".txt");

            CSharpWriter writer = null; 
            if (settings == null )
            {
                writer = new CSharpWriter();
            }
            else
            {
                writer = new CSharpWriter(settings);
            }

            Generator generator = new Generator(writer);
            string output = generator.Analyse(input).Write();
            
            Assert.AreEqual(expected.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), output.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
        }
    }
}
