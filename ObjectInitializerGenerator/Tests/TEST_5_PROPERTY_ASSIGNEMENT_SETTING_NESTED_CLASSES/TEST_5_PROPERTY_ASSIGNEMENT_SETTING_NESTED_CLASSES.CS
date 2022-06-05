using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestClass]
    public class TEST_5_PROPERTY_ASSIGNEMENT_SETTING_NESTED_CLASSES : Base
    {
        [TestMethod]
        public void Run()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add("UseAssignmentStatements", "true");
            Run("TEST_5_PROPERTY_ASSIGNEMENT_SETTING_NESTED_CLASSES", settings);

            TestClass testclass = new TestClass();
            testclass.TestGuid = Guid.NewGuid();
            testclass.TestBoolean = true;
            testclass.TestChildClass = new ChildClass();
            testclass.TestChildClass.TestInt = 1;
            testclass.TestChildClass.TestChildChildClass = new ChildChildClass();
            testclass.TestChildClass.TestChildChildClass.TestBool = true;

            ChildClass childclass = new ChildClass();
            childclass.TestInt = 1;
            childclass.TestChildChildClass = new ChildChildClass();
            childclass.TestChildChildClass.TestBool = true;

            ChildChildClass childchildclass = new ChildChildClass();
            childchildclass.TestBool = true;


        }

        public class TestClass
        {
            public Guid TestGuid { get; set; }
            public Boolean TestBoolean { get; set; }
            public ChildClass TestChildClass { get; set; }
        }

        public class ChildClass
        {

            public int TestInt { get; set; }

            public ChildChildClass TestChildChildClass { get; set; }

        }


        public class ChildChildClass
        {

            public bool TestBool { get; set; }

        }

    }
}
