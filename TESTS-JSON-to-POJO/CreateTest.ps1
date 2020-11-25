Set-ExecutionPolicy RemoteSigned -Confirm:$false -Force;
$TestName = Read-Host -Prompt 'Test Name With Number: THE FORMAT IS [TESTNUMBER]_[TESTNAME], EXAMPLE : 10_TESTNAME.'
# TO CHANGE $Dir TO CHANGE $Dir TO CHANGE $Dir TO CHANGE $Dir TO CHANGE $Dir TO CHANGE $Dir TO CHANGE $Dir TO CHANGE $Dir 

$Dir = "C:\Users\Hilal\Desktop\Json2CSharpCodeGenerator\TESTS-JSON-to-POJO\" 
$fullPath = $Dir + "Test_" + $TestName + ".cs"
$fullPathInput = $Dir + "Test_" + $TestName + "_INPUT"+ ".txt"
$fullPathOutput = $Dir + "Test_" + $TestName + "_OUTPUT"+".txt"

New-Item -Path $fullPath  -ItemType File
New-Item -Path $fullPathInput  -ItemType File
New-Item -Path $fullPathOutput  -ItemType File


$testFileContentStart = "
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_to_POJO
{

    [TestClass]
";
$testFileClassName = "  public class Test_{0}" -f $TestName;

$testFileContentStart1 = "{
   
        [TestMethod]
        public void Run()
        { 
        ";

$content1 = "string path = Directory.GetCurrentDirectory().Replace(""bin\\Debug"", """")  + @`"Test_{0}_INPUT.txt`";
" -f $TestName;
$content2 = "string resultPath = Directory.GetCurrentDirectory().Replace(""bin\\Debug"", """") + @`"Test_{0}_OUTPUT.txt`";" -f $TestName;
 
$testFileContentMiddle = "            
            string input = File.ReadAllText(path);
            string errorMessage = string.Empty;
			JavaCodeWriter javaCodeWriter = new JavaCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
			jsonClassGenerator.CodeWriter = javaCodeWriter;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath); 
            ";

$testFileAssertion = "    Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, `"`").Replace(`" `", `"`").Replace(`"\t`", `"`"), returnVal.Replace(Environment.NewLine, `"`").Replace(`" `", `"`").Replace(`"\t`", `"`"));";
$testFileContentEnd = "
        }
    }
}";


$tesformattedString = $testFileContentStart + $testFileClassName +$testFileContentStart1+ $content1 + [Environment]::NewLine + $content2  + $testFileContentMiddle + $testFileAssertion +$testFileContentEnd

Set-Content -Path $fullPath -Value $tesformattedString 

Set-Content -Path $fullPathOutput -Value "dfasdfadfasdf" # DO NOT REMOVE : IF THIS IS EMPTY THE TEST WILL SUCCEED, WE WANT TO FAIL INITIALLY


Set-ExecutionPolicy Restricted -Confirm:$false -Force;