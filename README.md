## Special Thanks to Our Contributors !!
Updated Pluralization Package & Test cleanup
[marblekirby](https://github.com/marblekirby) <br/> 

Implemented Immutable Classes Feature <br/>
[Holly-HaCKer](https://github.com/HoLLy-HaCKeR) <br/>

Implemented Immutable Classes Feature <br/>
[Jehoel](https://github.com/Jehoel) <br/>
 
Assisted in Bug Fixing <br/>
[tyeth](https://github.com/tyeth), [dogac00](https://github.com/dogac00)

## Architecture Overview
![Json2CsharpArchitecture drawio](https://user-images.githubusercontent.com/67238834/200273043-6e13fc15-2884-44d8-bf77-be3fc36b46d3.png)
The architecture of the project is segmented into two parts. 
- Part 1: Consuming the JSON input. In this phase, the below is generally done: 
  - Parse the Json string
  - Create a list of JObjects from the parsed string
  - Create a dictionary of JObject types/ Understand how the json is structured
- Part 2: Write The Output using the supplied ICodeWriter interface
  - Using the property injected in the *JsonClassGenerator* class, the corresponding concrete code writer is called and the writing process begins
  - The code write can be configured using the configuration classes in the *CodeWriteConfiguration* folder
  - Create a dictionary of JObject types/ Understand how the json is structured
  - Finally, the *GenerateClasses* in the *JsonClassGenerator* class will return a StringBuilder instance that contains all the written classes 

## Roadmap
- Support for dictionaries https://github.com/Json2CSharp/Json2CSharpCodeGenerator/issues/18: The tool should have the option to detect dictionaries, currently dictionaries are treated as classes. Can be done using Markov Chain approach
- Support for nested arrays https://github.com/Json2CSharp/Json2CSharpCodeGenerator/issues/5: Currently, nested arrays return empty values
- Support for datetime https://github.com/Json2CSharp/Json2CSharpCodeGenerator/issues/90 ability to detect various types of datetime formats and a setting to turn this feature off


## Bug Fixing
### 1- Choose a problem from the issues labeled "Bug" or "Help Wanted"
### 2- Clone the repository and build it
### 3- Create a Unit Test using "CreatTest.ps1" Powershell Script or copy paste existing unit tests and follow naming conventions
* Use the format to name your unit tests: "[TestNumber]_[DescribeProblem]"**
* Run the script in Powershell
* This will create 3 files: the csharp test, the Json Input text file, and the Output C# or JAVA etc.. file
![alt text](https://json2csharp.azureedge.net/images/github-repo-images/Test%20Files.png)
* Start Debugging, Put some test Json and Get to Know the solution 

