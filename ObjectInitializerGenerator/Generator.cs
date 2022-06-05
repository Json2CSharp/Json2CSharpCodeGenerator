using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ObjectInitializerGenerator
{
    public class Generator
    {
        private readonly ICodeWriter codeWriter;
        List<ObjectModel> objectModelList;
        public Generator(ICodeWriter codeWriter)
        {
            this.codeWriter = codeWriter;
        }

        public Generator Analyse(string input)
        {
            try
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(input);
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

                objectModelList = new List<ObjectModel>();

                foreach (SyntaxNode child in root.ChildNodes())
                {
                    if (child.Kind() == SyntaxKind.ClassDeclaration)
                    {
                        ObjectModel parent = new ObjectModel();
                        foreach (SyntaxToken item in child.ChildTokens())
                        {
                            if (item.Kind() == SyntaxKind.IdentifierToken)
                            {
                                parent.TokenType = SyntaxKind.ClassDeclaration;
                                parent.SyntaxName = item.Value.ToString();
                                break;
                            }
                        }

                        if (child.ChildNodes().Count() > 0)
                        {
                            AnalyseChildNodes(child, parent);
                        }

                        objectModelList.Add(parent);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return this;
        }


        private void AnalyseChildNodes(SyntaxNode node, ObjectModel parent)
        {

            foreach (SyntaxNode child in node.ChildNodes())
            {
                if (child.Kind() == SyntaxKind.ClassDeclaration)
                {
                    ObjectModel subParent = new ObjectModel();
                    foreach (SyntaxToken item in child.ChildTokens())
                    {
                        if (item.Kind() == SyntaxKind.IdentifierToken)
                        {
                            subParent.TokenType = SyntaxKind.ClassDeclaration;
                            subParent.SyntaxName = item.Value.ToString();
                            break;
                        }
                    }

                    if (child.ChildNodes().Count() > 0)
                    {
                        AnalyseChildNodes(child, subParent);
                    }

                    parent.Children.Add(subParent);
                }
                else if (child.Kind() == SyntaxKind.PropertyDeclaration)
                {
                    ObjectModel property = new ObjectModel();
                    foreach (SyntaxToken item in child.ChildTokens())
                    {
                        if (item.Kind() == SyntaxKind.IdentifierToken)
                        {
                            property.TokenType = SyntaxKind.PropertyDeclaration;
                            property.SyntaxName = item.Value.ToString();
                            break;
                        }
                    }

                    foreach (SyntaxNode item in child.ChildNodes())
                    {
                        if (item.Kind() == SyntaxKind.PredefinedType // int, bool, string
                            || item.Kind() == SyntaxKind.NullableType // Nullable kinds
                            || item.Kind() == SyntaxKind.IdentifierName) // Classes, Guid, Boolean, String
                        {
                            property.PropertyType = item.ToString();
                            property.NodeType = item.Kind();
                            break;
                        }

                        if (item.Kind() == SyntaxKind.GenericName // Lists, Dictionaries
                            || item.Kind() == SyntaxKind.ArrayType) // Arrays
                        {
                            // get the generic type inside the array, or list
                            foreach (var genericItem in item.ChildNodes())
                            {
                                if (genericItem.Kind() == SyntaxKind.PredefinedType)
                                {
                                    property.GenericType = genericItem.ToString();
                                    break;
                                }
                            }
                            property.PropertyType = item.ToString();
                            property.NodeType = item.Kind();
                            break;
                        }
                    }
                    parent.Children.Add(property);
                }
            }
        }

        public string Write()
        {

            try
            {
                return codeWriter.Write(objectModelList);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
