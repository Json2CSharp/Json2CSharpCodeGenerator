using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Tests
{
    [TestClass]
    public class TEST_3_NESTED_CLASSES_TEST : Base
    {
        [TestMethod]
        public void Run()
        {
            Run("TEST_3_NESTED_CLASSES_TEST");
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