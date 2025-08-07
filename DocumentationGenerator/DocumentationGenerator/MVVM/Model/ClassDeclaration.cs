using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationGenerator.MVVM.Model
{
    /// <summary>
    /// A Class Declaration is a class that is declared within a script file (.cs for example), and can store the classes name, and definition (if has one), along with its fields, properties and methods (and functions).
    /// </summary>
    public struct ClassDeclaration
    {
        public string Name { get; }
        public string? Definition { get; }
        public Declaration[]? Methods { get; }
        public Declaration[]? Fields { get; }
        public Declaration[]? Properties { get; }

        public ClassDeclaration(string name, string? Definition, Declaration[]? methodDeclarations, Declaration[]? fieldDeclarations, Declaration[]? propertiesDeclarations)
        {
            this.Name = name;
            this.Definition = Definition;
            this.Methods = methodDeclarations;
            this.Fields = fieldDeclarations;
            this.Properties = propertiesDeclarations;
        }
    }

    /// <summary>
    /// A declaration refers to anything declared within a script file. This could be classes, enums, interfaces, fields, properties, methods.
    /// This can be used to stored any of these declarations along with their definition, types and return definitions.
    /// </summary>
    public struct Declaration
    {
        public string Name { get; set; }
        public string? Type { get; set; }
        public string? Definition { get; set; }
        public string? ReturnDefinition { get; set; }
        public bool? IsTypePrimitive { get; set; }

        public Declaration(string name, string? definition, string? type = null, string? returns = null, bool? isTypePrimitive = null)
        {
            this.Name = name;
            this.Type = type;
            this.Definition = definition;
            this.ReturnDefinition = returns;
            this.IsTypePrimitive = isTypePrimitive;
        }
    }
}
