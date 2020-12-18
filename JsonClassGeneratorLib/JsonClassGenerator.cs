// Copyright © 2010 Xamasoft

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace Xamasoft.JsonClassGenerator
{
    public class JsonClassGenerator : IJsonClassGeneratorConfig
    {
        public string Example { get; set; }
        public string Namespace { get; set; }
        public string SecondaryNamespace { get; set; }
        public bool UseFields { get; set; }
        public bool InternalVisibility { get; set; }
        public bool ExplicitDeserialization { get; set; }
        public bool NoHelperClass { get; set; }
        public string MainClass { get; set; }

        public class SouRce    {
        public List<string> includes { get; set; } 

    }

        public bool UsePascalCase { get; set; }
        public bool UseJsonAttributes { get; set; }
        public bool UseJsonPropertyName { get; set; }


        public bool UseNestedClasses { get; set; }
        public bool UseProperties { get; set; }
        public bool ApplyObfuscationAttributes { get; set; }
        public bool SingleFile { get; set; }
        public ICodeBuilder CodeWriter { get; set; }
        public TextWriter OutputStream { get; set; }
        public bool AlwaysUseNullableValues { get; set; }
        public bool ExamplesInDocumentation { get; set; }

        private PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-us"));

        private bool used = false;
        public bool UseNamespaces { get { return Namespace != null; } }

        public StringBuilder GenerateClasses(string jsonInput, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                JObject[] examples = null;
                try
                {
                    using (var sr = new StringReader(jsonInput))
                    using (var reader = new JsonTextReader(sr))
                    {
                        var json = JToken.ReadFrom(reader);
                        if (json is JArray)
                        {
                            examples = ((JArray)json).Cast<JObject>().ToArray();
                        }
                        else if (json is JObject)
                        {
                            examples = new[] { (JObject)json };
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "Exception: " + ex.Message;
                    return new StringBuilder();
                }

                if (CodeWriter == null) CodeWriter = new CSharpCodeWriter();

                Types = new List<JsonType>();
                Names.Add("Root");
                var rootType = new JsonType(this, examples[0]);
                rootType.IsRoot = true;
                rootType.AssignName("Root");
                GenerateClass(examples, rootType);
                StringBuilder builder = new StringBuilder();
                WriteClassesToFile(builder, Types);

                return builder;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                return new StringBuilder();
            }
        }

        private void WriteClassesToFile(string path, IEnumerable<JsonType> types)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                //WriteClassesToFile(sw, types);
            }
        }

        private void WriteClassesToFile(StringBuilder sw, IEnumerable<JsonType> types)
        {
            var inNamespace = false;
            var rootNamespace = false;

            CodeWriter.WriteFileStart(this, sw);
            CodeWriter.WriteDeserializationComment(this, sw);

            foreach (var type in types)
            {
                if (UseNamespaces && inNamespace && rootNamespace != type.IsRoot && SecondaryNamespace != null) { CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace); inNamespace = false; }
                if (UseNamespaces && !inNamespace) { CodeWriter.WriteNamespaceStart(this, sw, type.IsRoot); inNamespace = true; rootNamespace = type.IsRoot; }
                CodeWriter.WriteClass(this, sw, type);
            }
            if (UseNamespaces && inNamespace) CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace);
            CodeWriter.WriteFileEnd(this, sw);
        }


        private void GenerateClass(JObject[] examples, JsonType type)
        {
            var jsonFields = new Dictionary<string, JsonType>();
            var fieldExamples = new Dictionary<string, IList<object>>();

            var first = true;

            foreach (var obj in examples)
            {
                foreach (var prop in obj.Properties())
                {
                    JsonType fieldType;
                    var currentType = new JsonType(this, prop.Value);
                    var propName = prop.Name;
                    if (jsonFields.TryGetValue(propName, out fieldType))
                    {

                        var commonType = fieldType.GetCommonType(currentType);

                        jsonFields[propName] = commonType;
                    }
                    else
                    {
                        var commonType = currentType;
                        if (first) commonType = commonType.MaybeMakeNullable(this);
                        else commonType = commonType.GetCommonType(JsonType.GetNull(this));
                        jsonFields.Add(propName, commonType);
                        fieldExamples[propName] = new List<object>();
                    }
                    var fe = fieldExamples[propName];
                    var val = prop.Value;
                    if (val.Type == JTokenType.Null || val.Type == JTokenType.Undefined)
                    {
                        if (!fe.Contains(null))
                        {
                            fe.Insert(0, null);
                        }
                    }
                    else
                    {
                        var v = val.Type == JTokenType.Array || val.Type == JTokenType.Object ? val : val.Value<object>();
                        if (!fe.Any(x => v.Equals(x)))
                            fe.Add(v);
                    }
                }
                first = false;
            }

            if (UseNestedClasses)
            {
                foreach (var field in jsonFields)
                {
                    Names.Add(field.Key.ToLower());
                }
            }

            foreach (var field in jsonFields)
            {
                var fieldType = field.Value;
                if (fieldType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(examples.Length);
                    foreach (var obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Object)
                            {
                                subexamples.Add((JObject)value);
                            }
                        }
                    }

                    fieldType.AssignName(CreateUniqueClassName(field.Key));
                    GenerateClass(subexamples.ToArray(), fieldType);
                }

                if (fieldType.InternalType != null && fieldType.InternalType.Type == JsonTypeEnum.Object)
                {
                    var subexamples = new List<JObject>(examples.Length);
                    foreach (var obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Array)
                            {
                                if (value.Count() > 50)
                                {
                                    var takeABunchOfItems = (JArray)value; // Take like 30 items from the array this will increase the chance of getting all the objects accuralty while not analyzing all the data
                                    int count = 0;
                                    foreach (var item in (JArray)takeABunchOfItems)
                                    {
                                        if (count > 50)
                                            break;

                                        // if (!(item is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                        if ((item is JObject))
                                        subexamples.Add((JObject)item);
                                        ++count;
                                    }
                                }
                                else
                                {
                                    foreach (var item in (JArray)value)
                                    {
                                        // if (!(item is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                        if ((item is JObject))
                                        subexamples.Add((JObject)item);
                                    }
                                }
                            }
                            else if (value.Type == JTokenType.Object) //TODO J2C : ONLY LOOP OVER 50 OBJECT AND NOT THE WHOLE THING
                            {
                                foreach (var item in (JObject)value)
                                {
                                    // if (!(item.Value is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                    if ((item.Value is JObject))
                                    subexamples.Add((JObject)item.Value);
                                }
                            }
                        }
                    }

                    field.Value.InternalType.AssignName(CreateUniqueClassNameFromPlural(field.Key));
                    GenerateClass(subexamples.ToArray(), field.Value.InternalType);
                }
            }

            type.Fields = jsonFields.Select(x => new FieldInfo(this, x.Key, x.Value, UsePascalCase, UseJsonAttributes, UseJsonPropertyName, fieldExamples[x.Key])).ToArray();

            if (!string.IsNullOrEmpty(type.AssignedName))
                Types.Add(type);
        }

        public IList<JsonType> Types { get; private set; }
        private HashSet<string> Names = new HashSet<string>();

        private string CreateUniqueClassName(string name)
        {
            name = ToTitleCase(name);

            var finalName = name;
            var i = 2;
            while (Names.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
            {
                finalName = name + i.ToString();
                i++;
            }

            Names.Add(finalName);
            return finalName;
        }

        private string CreateUniqueClassNameFromPlural(string plural)
        {
            plural = ToTitleCase(plural);
            return CreateUniqueClassName(pluralizationService.Singularize(plural));
        }



        internal static string ToTitleCase(string str)
        {
            var sb = new StringBuilder(str.Length);
            var flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(flag ? char.ToUpper(c) : c);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            return sb.ToString();
        }
        

        public bool HasSecondaryClasses
        {
            get { return Types.Count > 1; }
        }
    
    }
}
