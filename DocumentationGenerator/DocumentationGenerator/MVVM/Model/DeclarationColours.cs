using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;

namespace DocumentationGenerator.MVVM.Model
{
    public struct DeclarationColours
    {
        public Color ClassDeclarationColour { get; set; }
        public Color EnumDeclarationColour { get; set; }
        public Color PrimitiveDeclarationColour { get; set; }
        public Color InterfaceDeclarationColour { get; set; }
        public Color StructDeclarationColour { get; set; }

        public DeclarationColours(Color classDeclarationColour, Color enumDeclarationColour, Color primitiveDeclarationColour, Color interfaceDeclarationColour, Color structDeclarationColour)
        {
            ClassDeclarationColour = classDeclarationColour;
            EnumDeclarationColour = enumDeclarationColour;
            PrimitiveDeclarationColour = primitiveDeclarationColour;
            InterfaceDeclarationColour = interfaceDeclarationColour;
            StructDeclarationColour = structDeclarationColour;
        }
    }
}
