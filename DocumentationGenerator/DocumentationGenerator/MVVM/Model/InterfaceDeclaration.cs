using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationGenerator.MVVM.Model
{
    public struct InterfaceDeclaration
    {
        public string Name { get; }
        public string? Definition { get; }
        public Declaration[]? Properties { get; set; }
        public Declaration[]? Methods { get; set; }
        
        public InterfaceDeclaration(string name, string? definition, Declaration[]? properties, Declaration[]? methods)
        {
            this.Name = name;
            this.Definition = definition;
            this.Properties = properties;
            this.Methods = methods;
        }
    }
}
