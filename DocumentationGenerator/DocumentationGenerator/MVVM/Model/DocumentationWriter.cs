using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.Security.AccessControl;

namespace DocumentationGenerator.MVVM.Model
{
    public class DocumentationWriter
    {
        private const string ObjectStyle = "Object";
        private const string ObjectDefinitionStyle = "ObjectDefinition";
        private const string MemberHeading = "MemberHeading";
        private const string MemberStyle = "Member";
        private const string MemberTypeStyle = "MemberType";
        private const string MemberDefinitionStyle = "MemberDefinition";

        public DocumentationWriter()
        {

        }

        /// <summary>
        /// Creates a PDF Document, attempts to write all the declarations provided and saves.
        /// </summary>
        /// <param name="path">The path where you want to save the file.</param>
        /// <param name="classes">The Declaration of classes read from the SourceFileReader.</param>
        /// <param name="enums">The Declaration of enums read from the SourceFileReader.</param>
        /// <returns></returns>
        public bool WriteDocumentation(string path, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DeclarationColours declarationColours)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) { return false; }
            Document document = new Document();
            Styles styles = InitialiseDocumentStyles(document.Styles, declarationColours);

            bool alterations = false;
            if (classes != null && classes.Length > 0)
            {
                alterations = true;
                WriteClasses(classes,declarationColours, document);
            }

            if(structs != null && structs.Length > 0)
            {
                WriteStructs(structs, declarationColours, document);
            }

            if(interfaces != null && interfaces.Length > 0)
            {
                WriteInterfaces(interfaces,declarationColours,document);
            }

            if (enums != null && enums.Length > 0)
            {
                alterations = true;
                WriteEnums(enums,declarationColours.EnumDeclarationColour ,document);
            }


            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer
            {
                // Associate the MigraDoc document with a renderer.
                Document = document,
                // Let the PDF viewer show this PDF with full pages.
                PdfDocument =
                {
                    PageLayout = PdfPageLayout.SinglePage,
                    ViewerPreferences =
                    {
                        FitWindow = true
                    }
                }
            };

            // Layout and render document to PDF.
            pdfRenderer.RenderDocument();

            // Add sample-specific heading with sample project helper function.

            // Save the document.
            pdfRenderer.Save(path);

            return alterations;
        }

        private void WriteStructs(StructDeclaration[] structs, DeclarationColours declarationColours, Document document)
        {
            Section section;
            foreach(StructDeclaration current in structs)
            {
                section = document.AddSection();

                Paragraph paragraph = section.AddParagraph("Struct ");
                paragraph.Style = ObjectStyle;

                FormattedText formattedText = paragraph.AddFormattedText(current.Name);
                formattedText.Font.Color = declarationColours.StructDeclarationColour;


                paragraph = section.AddParagraph($"Definition: {current.Definition}");
                paragraph.Style = ObjectDefinitionStyle;

                if (current.Properties != null && current.Properties.Length > 0)
                {
                    paragraph = section.AddParagraph(" Properties: ");
                    paragraph.Style = MemberHeading;
                    WriteVariables(current.Properties, declarationColours, section);
                }

                if (current.Fields != null && current.Fields.Length > 0)
                {
                    paragraph = section.AddParagraph($" Fields: ");
                    paragraph.Style = MemberHeading;
                    WriteVariables(current.Fields, declarationColours, section);
                }

                if (current.Methods != null && current.Methods.Length > 0)
                {
                    paragraph = section.AddParagraph(" Methods & Functions: ");
                    paragraph.Style = MemberHeading;
                    WriteVariables(current.Methods, declarationColours, section);
                }
            }
        }

        private void WriteInterfaces(InterfaceDeclaration[] interfaces, DeclarationColours declarationColours, Document document)
        {
            Section section;

            foreach(InterfaceDeclaration current in interfaces)
            {
                section = document.AddSection();

                Paragraph paragraph = section.AddParagraph("Interface ");
                paragraph.Style = ObjectStyle;

                FormattedText formattedText = paragraph.AddFormattedText(current.Name);
                formattedText.Font.Color = declarationColours.InterfaceDeclarationColour;

                paragraph = section.AddParagraph($"Definition: {current.Definition}");
                paragraph.Style = ObjectDefinitionStyle;

                if(current.Properties != null && current.Properties.Length > 0)
                {
                    paragraph = section.AddParagraph(" Properties: ");
                    paragraph.Style = MemberHeading;
                    WriteVariables(current.Properties, declarationColours, section);
                }

                if (current.Methods != null && current.Methods.Length > 0)
                {
                    paragraph = section.AddParagraph(" Methods & Functions: ");
                    paragraph.Style = MemberHeading;
                    WriteVariables(current.Methods, declarationColours, section);
                }
            }
        }

        private Styles InitialiseDocumentStyles(Styles styles, DeclarationColours declarationColours)
        {
            Style style = styles.AddStyle(ObjectStyle, StyleNames.Normal);
            style.Font.Size = 20;
            style.Font.Bold = true;
            style.Font.Italic = false;
            style.ParagraphFormat.SpaceAfter = "5pt";

            style = styles.AddStyle(ObjectDefinitionStyle, StyleNames.Normal);
            style.Font.Size = 18;
            style.Font.Bold = false;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceAfter = "15pt";

            style = styles.AddStyle(MemberHeading, StyleNames.Normal);
            style.Font.Size = 16;
            style.Font.Bold = true;
            style.Font.Italic = false;
            style.ParagraphFormat.SpaceAfter = "2pt";

            style = styles.AddStyle(MemberStyle, StyleNames.Normal);
            style.Font.Size = 14;
            style.Font.Bold = false;
            style.Font.Italic = false;
            style.ParagraphFormat.SpaceAfter = "1pt";

            style = styles.AddStyle(MemberTypeStyle, StyleNames.Normal);
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Font.Italic = false;
            style.ParagraphFormat.SpaceAfter = "1pt";

            style = styles.AddStyle(MemberDefinitionStyle, StyleNames.Normal);
            style.Font.Size = 14;
            style.Font.Bold = false;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceAfter = "1pt";

            return styles;
        }

        private void WriteEnums(EnumDeclaration[] enums, Color enumColour, Document document)
        {
            Section section;
            foreach (EnumDeclaration current in enums)
            {
                section = document.AddSection();
                Paragraph paragraph = section.AddParagraph($"Enum ");
                paragraph.Style = ObjectStyle;

                FormattedText formattedText = paragraph.AddFormattedText(current.Name);
                formattedText.Font.Color = enumColour;

                paragraph = section.AddParagraph($"Definition: {current.Definition}");
                paragraph.Style = ObjectDefinitionStyle;
                if (current.EnumMembers.Length > 0)
                {
                    WriteEnumMembers(current.EnumMembers, section);
                }
            }
        }

        private void WriteEnumMembers(Declaration[] enumMembers, Section section)
        {
            Paragraph paragraph = section.AddParagraph($" Members: ");
            paragraph.Style = MemberHeading;

            foreach (Declaration member in enumMembers)
            {
                paragraph = section.AddParagraph($" {member.Name} - ");
                paragraph.Style = MemberStyle;

                if(member.Definition == null) { continue; }

                FormattedText formatted = paragraph.AddFormattedText(member.Definition);
                formatted.Style = MemberDefinitionStyle;
            }
        }

        private void WriteClasses(ClassDeclaration[] classDeclarations, DeclarationColours declarationColours, Document document)
        {
            Section section;
            foreach (ClassDeclaration current in classDeclarations)
            {
                section = document.AddSection();
                Paragraph paragraph = section.AddParagraph($"Class ");
                paragraph.Style = ObjectStyle;

                FormattedText formatted = paragraph.AddFormattedText(current.Name);
                formatted.Font.Color = declarationColours.ClassDeclarationColour;

                paragraph = section.AddParagraph($"Definition: {current.Definition}");
                paragraph.Style = ObjectDefinitionStyle;

                if (current.Properties != null && current.Properties.Length > 0)
                {
                    paragraph = section.AddParagraph($" Properties: ");
                    paragraph.Style = MemberHeading;
                    WriteVariables(current.Properties,declarationColours ,section);
                }

                if (current.Fields != null && current.Fields.Length > 0)
                {
                    paragraph = section.AddParagraph($" Fields: ");
                    paragraph.Style = MemberHeading;
                    WriteVariables(current.Fields,declarationColours ,section);
                }

                if (current.Methods != null && current.Methods.Length > 0)
                {
                    paragraph = section.AddParagraph($" Methods & Functions: ");
                    paragraph.Style = MemberHeading;
                    WriteMethods(current.Methods,declarationColours ,section);
                }
            }
        }

        private void WriteMethods(Declaration[] methods, DeclarationColours declarationColours, Section section)
        {
            foreach (Declaration method in methods)
            {
                Paragraph paragraph = section.AddParagraph($" {method.Type} ");
                paragraph.Style = MemberTypeStyle;

                paragraph.Format.Font.Color = declarationColours.PrimitiveDeclarationColour;

                if (method.IsTypePrimitive.HasValue && method.IsTypePrimitive.Value == false)
                {
                    switch (method.WhatIsType)
                    {
                        case ObjectType.Class: paragraph.Format.Font.Color = declarationColours.ClassDeclarationColour; break;
                        case ObjectType.Enum: paragraph.Format.Font.Color = declarationColours.EnumDeclarationColour; break;
                        case ObjectType.Interface: paragraph.Format.Font.Color = declarationColours.InterfaceDeclarationColour; break;
                        //case ObjectType.Struct: paragraph.Format.Font.Color = declarationColours.ClassDeclarationColour; break;
                    }
                }

                FormattedText formatted = paragraph.AddFormattedText($"{method.Name} - ");
                formatted.Style = MemberStyle;

                formatted = paragraph.AddFormattedText($"{method.Definition} - {method.ReturnDefinition}");
                formatted.Style = MemberDefinitionStyle;
            }
        }

        private void WriteVariables(Declaration[] fields, DeclarationColours declarationColours ,Section section)
        {
            foreach (Declaration field in fields)
            {
                Paragraph paragraph = section.AddParagraph($" {field.Type} ");
                paragraph.Style = MemberTypeStyle;

                paragraph.Format.Font.Color = declarationColours.PrimitiveDeclarationColour;

                if (field.IsTypePrimitive.HasValue && field.IsTypePrimitive.Value == false) 
                {
                    switch (field.WhatIsType)
                    {
                        case ObjectType.Class: paragraph.Format.Font.Color = declarationColours.ClassDeclarationColour; break;
                        case ObjectType.Enum: paragraph.Format.Font.Color = declarationColours.EnumDeclarationColour; break;
                        case ObjectType.Interface: paragraph.Format.Font.Color = declarationColours.InterfaceDeclarationColour; break;
                        //case ObjectType.Struct: paragraph.Format.Font.Color = declarationColours.ClassDeclarationColour; break;
                    }
                }


                FormattedText formatted = paragraph.AddFormattedText($"{field.Name} - ");
                formatted.Style = MemberStyle;

                if(field.Definition == null) { continue; }
                formatted = paragraph.AddFormattedText(field.Definition);
                formatted.Style = MemberDefinitionStyle;

            }
        }
    }

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