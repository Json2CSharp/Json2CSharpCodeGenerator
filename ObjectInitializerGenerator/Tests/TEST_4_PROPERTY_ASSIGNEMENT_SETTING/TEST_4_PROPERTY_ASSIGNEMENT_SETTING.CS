using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestClass]
    public class TEST_4_PROPERTY_ASSIGNEMENT_SETTING : Base
    {
        [TestMethod]
        public void Run()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add("UseAssignmentStatements", "true");
            Run("TEST_4_PROPERTY_ASSIGNEMENT_SETTING", settings);
        }
    }
}
