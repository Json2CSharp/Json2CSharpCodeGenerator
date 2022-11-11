$TestName = Read-Host -Prompt 'Test Name With Number: THE FORMAT IS [TESTNUMBER]_[TESTNAME], EXAMPLE : 10_TESTNAME.'
#$Dir =  ($psise.CurrentFile.FullPath -replace "CreateTest.ps1", "") 
$Dir = "C:\Users\Hilal\OneDrive\Desktop\Json2CSharpCodeGenerator\TESTS-JSON-to-CSHARP\"
$fullPath = $Dir + "Test_" + $TestName + ".cs"
$fullPathInput = $Dir + "Test_" + $TestName + "_INPUT"+ ".txt"
$fullPathOutput = $Dir + "Test_" + $TestName + "_OUTPUT"+".txt"

New-Item -Path $fullPath  -ItemType File
New-Item -Path $fullPathInput  -ItemType File
New-Item -Path $fullPathOutput  -ItemType File

$testFileContentStart = "using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{
`t[TestClass]";
$testFileClassName = "`r`n`tpublic class Test_{0}" -f $TestName;

$testFileContentStart1 = 
"`r`n`t{
`t`t[TestMethod]
`t`tpublic void Run()
`t`t{";

$content1 = "`r`n`t`t`tstring path = Directory.GetCurrentDirectory().Replace(""bin\\Debug"", """")  + @`"Test_{0}_INPUT.txt`";" -f $TestName;

$content2 = "`r`n`t`t`tstring resultPath =  Directory.GetCurrentDirectory().Replace(""bin\\Debug"", """") + @`"Test_{0}_OUTPUT.txt`";" -f $TestName;
 
$testFileContentMiddle = 
"`r`n`t`t`tstring input = File.ReadAllText(path);
`t`t`tstring errorMessage = string.Empty;
`t`t`tCSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter();
`t`t`tJsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
`t`t`tjsonClassGenerator.CodeWriter = csharpCodeWriter;
`t`t`tstring returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
`t`t`tstring resultsCompare = File.ReadAllText(resultPath);";

$testFileAssertion = "`r`n`t`t`tAssert.AreEqual(resultsCompare.Replace(Environment.NewLine, `"`").Replace(`" `", `"`").Replace(`"\t`", `"`"), returnVal.Replace(Environment.NewLine, `"`").Replace(`" `", `"`").Replace(`"\t`", `"`"));";
$testFileContentEnd = "
`t`t}
`t}
}";


$tesformattedString = $testFileContentStart + $testFileClassName + $testFileContentStart1 + $content1 + $content2  + $testFileContentMiddle + $testFileAssertion +$testFileContentEnd

Set-Content -Path $fullPath -Value $tesformattedString 

Set-Content -Path $fullPathOutput -Value "dfasdfadfasdf" # DO NOT REMOVE : IF THIS IS EMPTY THE TEST WILL SUCCEED, WE WANT TO FAIL INITIALLY

#Set-ExecutionPolicy Restricted -Confirm:$false -Force;