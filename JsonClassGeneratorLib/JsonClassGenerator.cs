// Copyright © 2010 Xamasoft

using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace Xamasoft.JsonClassGenerator
{
    public class JsonClassGenerator : IJsonClassGeneratorConfig
    {
        #region IJsonClassGeneratorConfig

        public string       Namespace                  { get; set; }
        public string       SecondaryNamespace         { get; set; }
        public bool         UseFields                  { get; set; }
        public bool         InternalVisibility         { get; set; }
        public bool         ExplicitDeserialization    { get; set; }
        public bool         NoHelperClass              { get; set; }
        public string       MainClass                  { get; set; }
        public bool         UseProperties              { get; set; }
        public bool         UsePascalCase              { get; set; }
        public bool         UseJsonAttributes          { get; set; }
        public bool         UseJsonPropertyName        { get; set; }
        public bool         UseNestedClasses           { get; set; }
        public bool         ApplyObfuscationAttributes { get; set; }
        public bool         SingleFile                 { get; set; }
        public ICodeBuilder CodeWriter                 { get; set; }
        public bool         HasSecondaryClasses        => this.Types.Count > 1;
        public bool         AlwaysUseNullableValues    { get; set; }
        public bool         UseNamespaces              => !String.IsNullOrEmpty(this.Namespace);
        public bool         ExamplesInDocumentation    { get; set; }
        public bool         ImmutableClasses           { get; set; }
        public bool         RecordTypes                { get; set; }
        public bool         NoSettersForCollections    { get; set; }

        #endregion

        public string Example { get; set; }

        public class SouRce
        {
            public List<string> includes { get; set; }

        }

        public TextWriter OutputStream { get; set; }

        private PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-us"));

        private bool used = false;

        public StringBuilder GenerateClasses(string jsonInput, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                JObject[] examples = null;
                try
                {
                    using (StringReader sr = new StringReader(jsonInput))
                    using (JsonTextReader reader = new JsonTextReader(sr))
                    {
                        JToken json = JToken.ReadFrom(reader);
                        if (json is JArray jArray)
                        {
                            examples = jArray.Cast<JObject>().ToArray();
                        }
                        else if (json is JObject jObject)
                        {
                            examples = new[] { jObject };
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "Exception: " + ex.Message;
                    return new StringBuilder();
                }

                if (this.CodeWriter == null) this.CodeWriter = new CSharpCodeWriter();

                this.Types = new List<JsonType>();
                this.Names.Add("Root");
                JsonType rootType = new JsonType(this, examples[0]);
                rootType.IsRoot = true;
                rootType.AssignName("Root");
                this.GenerateClass(examples, rootType);

                this.Types = this.HandleDuplicateClasses(this.Types);

                StringBuilder builder = new StringBuilder();
                this.WriteClassesToFile(builder, this.Types);

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
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                //WriteClassesToFile(sw, types);
            }
        }

        private void WriteClassesToFile(StringBuilder sw, IEnumerable<JsonType> types)
        {
            Boolean inNamespace = false;
            Boolean rootNamespace = false;

            this.CodeWriter.WriteFileStart(this, sw);
            this.CodeWriter.WriteDeserializationComment(this, sw);

            foreach (JsonType type in types)
            {
                if (this.UseNamespaces && inNamespace && rootNamespace != type.IsRoot && this.SecondaryNamespace != null) { this.CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace); inNamespace = false; }
                if (this.UseNamespaces && !inNamespace) { this.CodeWriter.WriteNamespaceStart(this, sw, type.IsRoot); inNamespace = true; rootNamespace = type.IsRoot; }
                this.CodeWriter.WriteClass(this, sw, type);
            }
            if (this.UseNamespaces && inNamespace) this.CodeWriter.WriteNamespaceEnd(this, sw, rootNamespace);
            this.CodeWriter.WriteFileEnd(this, sw);
        }

        private void GenerateClass(JObject[] examples, JsonType type)
        {
            Dictionary<String, JsonType> jsonFields = new Dictionary<string, JsonType>();
            Dictionary<String, List<Object>> fieldExamples = new Dictionary<string,List<object>>();

            Boolean first = true;

            foreach (JObject obj in examples)
            {
                foreach (JProperty prop in obj.Properties())
                {
                    JsonType fieldType;
                    JsonType currentType = new JsonType(this, prop.Value);
                    String propName = prop.Name;

                    if (jsonFields.TryGetValue(propName, out fieldType))
                    {

                        JsonType commonType = fieldType.GetCommonType(currentType);

                        jsonFields[propName] = commonType;
                    }
                    else
                    {
                        JsonType commonType = currentType;
                        if (first) commonType = commonType.MaybeMakeNullable(this);
                        else commonType = commonType.GetCommonType(JsonType.GetNull(this));
                        jsonFields.Add(propName, commonType);
                        fieldExamples[propName] = new List<object>();
                    }

                    List<Object> fe = fieldExamples[propName];
                    JToken val = prop.Value;
                    if (val.Type == JTokenType.Null || val.Type == JTokenType.Undefined)
                    {
                        if (!fe.Contains(null))
                        {
                            fe.Insert(0, null);
                        }
                    }
                    else
                    {
                        Object v = val.Type == JTokenType.Array || val.Type == JTokenType.Object ? val : val.Value<object>();
                        if (!fe.Any(x => v.Equals(x)))
                        {
                            fe.Add(v);
                        }
                    }
                }
                first = false;
            }

            if (this.UseNestedClasses)
            {
                foreach (KeyValuePair<String, JsonType> field in jsonFields)
                {
                    this.Names.Add(field.Key.ToLower());
                }
            }

            foreach (KeyValuePair<String, JsonType> field in jsonFields)
            {
                JsonType fieldType = field.Value;
                if (fieldType.Type == JsonTypeEnum.Object)
                {
                    List<JObject> subexamples = new List<JObject>(examples.Length);
                    foreach (JObject obj in examples)
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

                    fieldType.AssignOriginalName(field.Key);
                    fieldType.AssignName(this.CreateUniqueClassName(field.Key));
                    fieldType.AssignNewAssignedName(ToTitleCase(field.Key));

                    this.GenerateClass(subexamples.ToArray(), fieldType);
                }

                if (fieldType.InternalType != null && fieldType.InternalType.Type == JsonTypeEnum.Object)
                {
                    List<JObject> subexamples = new List<JObject>(examples.Length);
                    foreach (JObject obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Array)
                            {
                                if (value.Count() > 50)
                                {
                                    JArray takeABunchOfItems = (JArray)value; // Take like 30 items from the array this will increase the chance of getting all the objects accuralty while not analyzing all the data
                                    int count = 0;
                                    foreach (JToken item in (JArray)takeABunchOfItems)
                                    {
                                        if (count > 50)
                                            break;

                                        // if (!(item is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                        if (( item is JObject ))
                                            subexamples.Add((JObject)item);
                                        ++count;
                                    }
                                }
                                else
                                {
                                    foreach (JToken item in (JArray)value)
                                    {
                                        // if (!(item is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                        if (( item is JObject ))
                                        {
                                            subexamples.Add((JObject)item);
                                        }
                                    }
                                }
                            }
                            else if (value.Type == JTokenType.Object) //TODO J2C : ONLY LOOP OVER 50 OBJECT AND NOT THE WHOLE THING
                            {
                                foreach (KeyValuePair<String, JToken> item in (JObject)value)
                                {
                                    // if (!(item.Value is JObject)) throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                    if (( item.Value is JObject ))
                                    {
                                        subexamples.Add((JObject)item.Value);
                                    }
                                }
                            }
                        }
                    }

                    field.Value.InternalType.AssignOriginalName(field.Key);
                    field.Value.InternalType.AssignName(this.CreateUniqueClassNameFromPlural(field.Key));
                    field.Value.InternalType.AssignNewAssignedName(this.pluralizationService.Singularize(ToTitleCase(field.Key)));

                    this.GenerateClass(subexamples.ToArray(), field.Value.InternalType);
                }
            }

            type.Fields = jsonFields.Select(x => new FieldInfo(this, x.Key, x.Value, this.UsePascalCase, this.UseJsonAttributes, this.UseJsonPropertyName, fieldExamples[x.Key])).ToList();

            if (!string.IsNullOrEmpty(type.AssignedName))
                this.Types.Add(type);
        }

        /// <summary>
        /// Checks if there are any duplicate classes in the input, and merges its corresponding properties (TEST CASE 7)
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        private IList<JsonType> HandleDuplicateClasses(IList<JsonType> types)
        {

            List<JsonType> typesWithNoDuplicates = new List<JsonType>();

            foreach (JsonType type in types)
            {

                if (!typesWithNoDuplicates.Exists(p => p.OriginalName == type.OriginalName))
                    typesWithNoDuplicates.Add(type);
                // Handle duplication
                else
                {
                    JsonType duplicatedType = typesWithNoDuplicates.FirstOrDefault(p => p.OriginalName == type.OriginalName);

                    // Rename all references of this type to the original assigned name
                    foreach (FieldInfo field in type.Fields)
                    {
                        if (!duplicatedType.Fields.ToList().Exists(x => x.JsonMemberName == field.JsonMemberName))
                            duplicatedType.Fields.Add(field);
                    }
                }
            }
            return typesWithNoDuplicates;
        }

        public IList<JsonType> Types { get; private set; }
        private HashSet<string> Names = new HashSet<string>();

        private string CreateUniqueClassName(string name)
        {
            name = ToTitleCase(name);

            String finalName = name;
            Int32 i = 2;
            while (this.Names.Any(x => x.Equals(finalName, StringComparison.OrdinalIgnoreCase)))
            {

                finalName = name + i.ToString();
                i++;
            }

            this.Names.Add(finalName);
            return finalName;
        }

        private string CreateUniqueClassNameFromPlural(string plural)
        {
            plural = ToTitleCase(plural);
            return this.CreateUniqueClassName(this.pluralizationService.Singularize(plural));
        }

        internal static string ToTitleCase(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            Boolean flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                Char c = str[i];
                string specialCaseFirstCharIsNumber = string.Empty;

                // Handle the case where the first character is a number
                if (i == 0 && char.IsDigit(c))
                    specialCaseFirstCharIsNumber = "_" + c;

                if (char.IsLetterOrDigit(c))
                {
                    if (string.IsNullOrEmpty(specialCaseFirstCharIsNumber))
                        sb.Append(flag ? char.ToUpper(c) : c);
                    else
                        sb.Append(flag ? specialCaseFirstCharIsNumber.ToUpper() : specialCaseFirstCharIsNumber);

                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            return sb.ToString();
        }

        public bool ArrayAsList()
        {
            return !this.ExplicitDeserialization;
        }
    }
}
