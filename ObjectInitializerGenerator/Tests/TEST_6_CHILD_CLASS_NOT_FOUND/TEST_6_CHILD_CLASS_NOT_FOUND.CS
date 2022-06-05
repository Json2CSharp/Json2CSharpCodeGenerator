using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestClass]
    public class TEST_6_CHILD_CLASS_NOT_FOUND : Base
    {
        [TestMethod]
        public void Run()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            Run("TEST_6_CHILD_CLASS_NOT_FOUND", settings);
            
            settings.Add("UseAssignmentStatements", "true");
            // Run("TEST_6_CHILD_CLASS_NOT_FOUND", settings);
        }
    }
}
