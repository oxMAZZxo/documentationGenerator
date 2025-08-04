using MigraDoc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Visitors;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.IO;


namespace DocumentationGenerator.MVVM.Model
{
    public class DocumentationWriter
    {
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
        public bool WriteDocumentation(string path, ClassDeclaration[]? classes = null, EnumDeclaration[]? enums = null)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) { return false; }
            Document document = new Document();
            Style style = document.AddStyle(StyleNames.Heading1, StyleNames.Normal);
            style.Font.Size = 20;
            style.Font.Bold = true;
            style.Font.Italic = false;
            style.ParagraphFormat.SpaceAfter = "5pt";

            style = document.AddStyle(StyleNames.Heading2, StyleNames.Normal);
            style.Font.Size = 18;
            style.Font.Bold = false;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceAfter = "15pt";

            style = document.AddStyle(StyleNames.Heading3, StyleNames.Normal);
            style.Font.Size = 16;
            style.Font.Bold = true;
            style.Font.Italic = false;
            style.ParagraphFormat.SpaceAfter = "2pt";

            style = document.AddStyle(StyleNames.Heading4, StyleNames.Normal);
            style.Font.Size = 14;
            style.Font.Bold = false;
            style.Font.Italic = false;
            style.ParagraphFormat.SpaceAfter = "1pt";


            bool alterations = false;
            if (classes != null && classes.Length > 0)
            {
                alterations = true;
                WriteClasses(classes, document);
            }

            if (enums != null && enums.Length > 0)
            {
                alterations = true;
                WriteEnums(enums, document);
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
            pdfRenderer.Save($"{path}\\Docs.pdf");

            return alterations;
        }

        private void WriteEnums(EnumDeclaration[] enums, Document document)
        {
            Section section;
            foreach (EnumDeclaration current in enums)
            {
                section = document.AddSection();
                Paragraph paragraph = section.AddParagraph($"Enum {current.Name}");
                paragraph.Style = StyleNames.Heading1;
                paragraph = section.AddParagraph($"Definition: {current.Definition}");
                paragraph.Style = StyleNames.Heading2;
                if (current.EnumMembers.Length > 0)
                {
                    WriteEnumMembers(current.EnumMembers, section);
                }
            }
        }

        private void WriteEnumMembers(Declaration[] enumMembers, Section section)
        {
            Paragraph paragraph = section.AddParagraph($" Members: ");
            paragraph.Style = StyleNames.Heading3;

            foreach (Declaration member in enumMembers)
            {
                paragraph = section.AddParagraph($" {member.Name} - {member.Definition}");
                paragraph.Style = StyleNames.Heading4;
            }
        }

        private void WriteClasses(ClassDeclaration[] classDeclarations, Document document)
        {
            Section section;
            foreach (ClassDeclaration current in classDeclarations)
            {
                section = document.AddSection();
                Paragraph paragraph = section.AddParagraph($"Class {current.Name}");
                paragraph.Style = StyleNames.Heading1;

                paragraph = section.AddParagraph($"Definition: {current.Definition}");
                paragraph.Style = StyleNames.Heading2;

                if (current.Properties != null && current.Properties.Length > 0)
                {
                    paragraph = section.AddParagraph($" Properties: ");
                    paragraph.Style = StyleNames.Heading3;
                    WriteVariables(current.Properties, section);
                }

                if (current.Fields != null && current.Fields.Length > 0)
                {
                    paragraph = section.AddParagraph($" Fields: ");
                    paragraph.Style = StyleNames.Heading3;
                    WriteVariables(current.Fields, section);
                }
                if (current.Methods != null && current.Methods.Length > 0)
                {
                    paragraph = section.AddParagraph($" Methods & Functions: ");
                    paragraph.Style = StyleNames.Heading3;
                    WriteMethods(current.Methods, section);
                }
            }
        }

        private void WriteMethods(Declaration[] methods, Section section)
        {
            foreach (Declaration method in methods)
            {
                Paragraph paragraph = section.AddParagraph($" {method.Type} {method.Name} - {method.Definition} - {method.ReturnDefinition}");
                paragraph.Style = StyleNames.Heading4;
            }
        }

        private void WriteVariables(Declaration[] fields, Section section)
        {
            foreach (Declaration field in fields)
            {
                Paragraph paragraph = section.AddParagraph($" {field.Type} {field.Name} - {field.Definition}");
                paragraph.Style = StyleNames.Heading4;

            }
        }
    }
}