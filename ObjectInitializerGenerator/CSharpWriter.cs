using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectInitializerGenerator
{
    public class CSharpWriter : ICodeWriter
    {
        private readonly StringBuilder builder;
        bool UseAssignmentStatements;
        List<ObjectModel> objectModels;
        public CSharpWriter(Dictionary<string, string> settings = null)
        {
            builder = new StringBuilder();

            if (settings != null)
            {
                foreach (var item in settings)
                {
                    UseAssignmentStatements = item.Key == SettingTypes.UseAssignmentStatements.ToString() && bool.Parse(item.Value);
                }
            }
        }

        public string Write(List<ObjectModel> objectModels)
        {
            this.objectModels = objectModels;
            foreach (ObjectModel classObject in objectModels)
            {
                WriteClassMember(classObject);
                BuilderWriteClassClosing();
            }

            return builder.ToString();
        }

        private void WriteClassMember(ObjectModel classObject)
        {

            string accessorClassName = classObject.SyntaxName.ToLowerInvariant();

            BuilderWriteClass(classObject.SyntaxName, accessorClassName);

            if (classObject.Children.Any())
            {
                foreach (ObjectModel child in classObject.Children)
                {
                    List<string> accessorList = new List<string>();
                    accessorList.Add(accessorClassName);
                    WritePropertyMember(child, accessorList);
                }
            }
        }

        private void WritePropertyMember(ObjectModel propertyModel, List<string> accessorList, ObjectModel propertyClassObject = null)
        {
            if (propertyClassObject != null && propertyClassObject.TokenType == SyntaxKind.ClassDeclaration) // Handle class properties
            {
                bool hasChildren = propertyClassObject.Children != null && propertyClassObject.Children.Any();
                BuilderWriteClassMember(propertyModel.SyntaxName, propertyModel.PropertyType, hasChildren, accessorList);
                if (hasChildren)
                {
                    accessorList.Add(propertyModel.SyntaxName);
                    foreach (ObjectModel child in propertyClassObject.Children)
                    {
                        bool checkIfTypeExist = this.objectModels.Exists(p => p.SyntaxName == child.PropertyType);
                        var propertyClass = this.objectModels.FirstOrDefault(p => p.SyntaxName == child.PropertyType);
                        WritePropertyMember(child, accessorList, propertyClass);
                    }
                    
                    BuilderWriteChildClassClosing();
                }
            }
            else if (propertyModel.TokenType == SyntaxKind.PropertyDeclaration)
            {
                try
                {
                    switch (propertyModel.NodeType)
                    {
                        case SyntaxKind.ArrayType: // Handle Arrays
                            BuilderWriteMember(propertyModel.SyntaxName, CSharpTypesMapping.MapArray(propertyModel), accessorList);
                            break;
                        case SyntaxKind.GenericName: // Handle Lists
                            BuilderWriteMember(propertyModel.SyntaxName, CSharpTypesMapping.MapObject(propertyModel), accessorList);
                            break;
                        case SyntaxKind.PredefinedType: // Any Non Generic Property : bool
                        case SyntaxKind.NullableType: // Any Nullabe Type : Datetime?
                            BuilderWriteMember(propertyModel.SyntaxName, CSharpTypesMapping.Map(propertyModel.PropertyType), accessorList);
                            break;
                        case SyntaxKind.IdentifierName: // Classes, Guid, Boolean, String
                            {
                                try // Boolean, Guid, String classes
                                {
                                    BuilderWriteMember(propertyModel.SyntaxName, CSharpTypesMapping.Map(propertyModel.PropertyType), accessorList);
                                }
                                catch (Exception) // Custom Classes
                                {
                                    bool checkIfTypeExist = this.objectModels.Exists(p => p.SyntaxName == propertyModel.PropertyType);
                                    var propertyClass = this.objectModels.FirstOrDefault(p => p.SyntaxName == propertyModel.PropertyType);
                                    if (checkIfTypeExist)
                                    {
                                        WritePropertyMember(propertyModel, accessorList, propertyClass);
                                    }
                                    else
                                    {
                                        BuilderWriteMember(propertyModel.SyntaxName, CSharpTypesMapping.MapObject(propertyModel), accessorList);
                                    }
                                }
                            }
                            break;
                        default:
                            throw new NotImplementedException(string.Format("propertyModel.NodeType {0} not handled in WritePropertyMember, CSharpWriter", propertyModel.NodeType));
                    }
                }
                catch (Exception ex)
                {
                    builder.AppendFormat("// Unknown Property : {0} {1}", propertyModel.SyntaxName, Environment.NewLine);
                }
            }
        }

        private void BuilderWriteMember(string syntaxName, string typeExample, List<string> accessorList = null)
        {
            if (UseAssignmentStatements)
            {
                string compoundedAccessorNames = string.Empty;
                foreach (var accessor in accessorList)
                {
                    compoundedAccessorNames += accessor + ".";
                }
                builder.AppendFormat("{0}{1} = {2};{3}", compoundedAccessorNames, syntaxName, typeExample, Environment.NewLine);
            }
            else
            {
                builder.AppendFormat("{0} = {1},{2}", syntaxName, typeExample, Environment.NewLine);
            }
        }

        private void BuilderWriteClassMember(string syntaxName, string accessorClassName, bool hasChildren, List<string> accessorList = null)
        {
            if (UseAssignmentStatements)
            {
                string compoundedAccessorNames = string.Empty;
                foreach (var accessor in accessorList)
                {
                    compoundedAccessorNames += accessor + ".";
                }
                compoundedAccessorNames += syntaxName;
                builder.AppendFormat("{0} = new {1}(){2} {3}", compoundedAccessorNames, accessorClassName, ";", Environment.NewLine);
            }
            else
            {
                builder.AppendFormat("{0} = new {1}(){2} {3}", syntaxName, accessorClassName, hasChildren ? "{" : ",", Environment.NewLine);
            }
        }

        private void BuilderWriteClass(string syntaxName, string accessorClassName)
        {
            if (UseAssignmentStatements)
            {
                builder.AppendFormat("{0} {1} = new {0}();{2}", syntaxName, accessorClassName, Environment.NewLine);
            }
            else
            {
                builder.AppendFormat("{0} {1} = new {0}() {{ {2}", syntaxName, accessorClassName, Environment.NewLine);
            }
        }

        private void BuilderWriteChildClassClosing()
        {
            if (!UseAssignmentStatements)
            {
                builder.AppendFormat("}}, {0}", Environment.NewLine);
            }
        }

        private void BuilderWriteClassClosing()
        {
            if (!UseAssignmentStatements)
            {
                builder.AppendLine("};");
            }
            builder.AppendLine();
        }

        private void BuilderWriteAccessor(string parent, string child)
        {
            if (UseAssignmentStatements)
            {
                builder.AppendLine("};");
            }
        }
    }
}
