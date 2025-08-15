using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationGenerator.MVVM.Model
{
    public class DocumentStyling
    {
        //public DeclarationColours DeclarationColours { get; set; }
        public DeclarationFonts DeclarationFonts { get; set; }
        public bool GenerateTableOfContents { get; set; }
        public bool GeneratePageNumbers { get; set; }

    }

    public struct DeclarationFonts
    {
        public string FontFamilyName { get; set; }
        public FontDeclarationStyle PrimitiveDeclarationFontStyle { get; set; }
        public FontDeclarationStyle ClassDeclarationFontStyle { get; set; }
        public FontDeclarationStyle StructDeclarationFontStyle { get; set; }
        public FontDeclarationStyle EnumDeclarationFontStyle { get; set; }
        public FontDeclarationStyle InterfaceDeclarationFontStyle { get; set; }
    }

    public struct FontDeclarationStyle
    {
        public int FontSize { get; set; }
        public Color Color { get; set; }
        public bool IsItalic { get; set; }
        public bool IsBold { get; set; }
    }
}
