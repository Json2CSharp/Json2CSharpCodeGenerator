using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectInitializerGenerator
{
    public class ObjectModel
    {
        public ObjectModel()
        {
            Children = new List<ObjectModel>();
        }

        public List<ObjectModel> Children { get; set; }

        /// <summary>
        /// The syntatic kind of the token, ClassDeclaration or PropertyDeclaration
        /// </summary>
        /// 
        public SyntaxKind TokenType { get; set; }

        /// <summary>
        /// The syntatic kind of the node, GenericName, PredefinedType, ArrayType
        /// </summary>
        /// 
        public SyntaxKind NodeType { get; set; }

        /// <summary>
        /// Class name in case the type is a class or Property Name in case the type is a property
        /// </summary>
        public string SyntaxName { get; set; } // testint

        /// <summary>
        /// The type of the property inside of the class
        /// </summary>
        public string PropertyType { get; set; } // int, byte[], string etc..

        /// <summary>
        /// What is the type in the generic list or array ? byte, string, int etc...
        /// </summary>
        public string GenericType { get; set; }
    }
}
