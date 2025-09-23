using DocumentationGenerator.MVVM.Helpers;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.IO;
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

        #region PDF
        /// <summary>
        /// Creates a PDF Document, attempts to write all the declarations provided and saves.
        /// </summary>
        /// <param name="path">The path where you want to save the file.</param>
        /// <param name="classes">The Declaration of classes read from the SourceFileReader.</param>
        /// <param name="enums">The Declaration of enums read from the SourceFileReader.</param>
        /// <returns></returns>
        public bool WritePDFDocumentation(string path, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docInfo)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) { return false; }
            Document document = new Document();

            Styles styles = InitialiseDocumentStylesForPDF(document.Styles, docInfo);

            // Add TOC before main content
            if (docInfo.GenerateTableOfContents)
            {
                List<string> tocEntries = new List<string>();

                if (classes != null) { tocEntries.AddRange(classes.Select(c => c.Name)); }
                if (structs != null) { tocEntries.AddRange(structs.Select(s => s.Name)); }
                if (interfaces != null) { tocEntries.AddRange(interfaces.Select(i => i.Name)); }
                if (enums != null) { tocEntries.AddRange(enums.Select(e => e.Name)); }

                AddTableOfContentsToPDF(document, tocEntries);
            }

            if (docInfo.GenerateRelationshipGraph)
            {
                string graphPath = GenerateRelationshipGraph(classes, interfaces, docInfo.DeclarationColours, path);
                Image image = document.AddSection().AddImage(graphPath);
                image.Width = 500;
            }


            bool alterations = false;
            if (classes != null && classes.Length > 0)
            {
                alterations = true;
                WriteClassesToPDF(classes, docInfo.DeclarationColours, document, docInfo.PrintBaseTypes);
            }

            if (structs != null && structs.Length > 0)
            {
                WriteStructsToPDF(structs, docInfo.DeclarationColours, document);
            }

            if (interfaces != null && interfaces.Length > 0)
            {
                WriteInterfacesToPDF(interfaces, docInfo.DeclarationColours, document);
            }

            if (enums != null && enums.Length > 0)
            {
                alterations = true;
                WriteEnumsToPDF(enums, docInfo.DeclarationColours.EnumDeclarationColour, document);
            }

            if (docInfo.GeneratePageNumbers) { AddPageNumbersToPDF(document); }

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

        private void AddTableOfContentsToPDF(Document document, List<string> entryNames)
        {
            Section tocSection = document.AddSection();

            // Title styling
            Paragraph tocHeading = tocSection.AddParagraph("Table of Contents");
            tocHeading.Format.Font.Size = 28;
            tocHeading.Format.Font.Bold = true;
            tocHeading.Format.SpaceAfter = "1.5cm";
            tocHeading.Format.Alignment = ParagraphAlignment.Center;

            foreach (var name in entryNames)
            {
                Paragraph tocEntry = tocSection.AddParagraph();

                // Left align entry name
                var link = tocEntry.AddHyperlink(name, HyperlinkType.Bookmark);
                link.AddText(name);

                // Add leader dots before page number
                tocEntry.AddTab();
                tocEntry.AddText(new string('.', 10)); // Long enough to stretch across the page
                tocEntry.AddTab();

                // Right align page number
                tocEntry.AddPageRefField(name);

                // Formatting for the TOC entries
                tocEntry.Format.Font.Size = 14;
                tocEntry.Format.SpaceBefore = 3;
                tocEntry.Format.SpaceAfter = 3;
                tocEntry.Format.Font.Color = Color.FromRgb(31, 78, 121);
                tocEntry.Format.Font.Bold = true;
                tocEntry.Format.TabStops.Clear();
                tocEntry.Format.TabStops.AddTabStop(Unit.FromCentimeter(12), TabAlignment.Right);
            }

            // Add extra space after TOC before starting main content
            tocSection.AddParagraph().Format.SpaceAfter = "1cm";
        }

        private void AddPageNumbersToPDF(Document document)
        {
            foreach (Section? section in document.Sections)
            {
                if (section == null) { continue; }
                // Add footer
                Paragraph footer = section.Footers.Primary.AddParagraph();
                footer.AddPageField();

                footer.Format.Alignment = ParagraphAlignment.Right;
                footer.Format.Font.Size = 8;
            }
        }

        private void WriteStructsToPDF(StructDeclaration[] structs, DeclarationColours declarationColours, Document document)
        {
            Section section;
            foreach (StructDeclaration current in structs)
            {
                section = document.AddSection();

                Paragraph paragraph = section.AddParagraph("Struct ");
                paragraph.AddBookmark(current.Name);
                paragraph.Style = ObjectStyle;

                FormattedText formattedText = paragraph.AddFormattedText(current.Name);
                formattedText.Font.Color = declarationColours.StructDeclarationColour;


                paragraph = section.AddParagraph($"Definition: {current.Definition}");
                paragraph.Style = ObjectDefinitionStyle;

                if (current.Properties != null && current.Properties.Length > 0)
                {
                    paragraph = section.AddParagraph(" Properties: ");
                    paragraph.Style = MemberHeading;
                    WriteVariablesToPDF(current.Properties, declarationColours, section);
                }

                if (current.Fields != null && current.Fields.Length > 0)
                {
                    paragraph = section.AddParagraph($" Fields: ");
                    paragraph.Style = MemberHeading;
                    WriteVariablesToPDF(current.Fields, declarationColours, section);
                }

                if (current.Methods != null && current.Methods.Length > 0)
                {
                    paragraph = section.AddParagraph(" Methods & Functions: ");
                    paragraph.Style = MemberHeading;
                    WriteVariablesToPDF(current.Methods, declarationColours, section);
                }
            }
        }

        private void WriteInterfacesToPDF(InterfaceDeclaration[] interfaces, DeclarationColours declarationColours, Document document)
        {
            Section section;

            foreach (InterfaceDeclaration current in interfaces)
            {
                section = document.AddSection();

                Paragraph paragraph = section.AddParagraph("Interface ");
                paragraph.AddBookmark(current.Name);
                paragraph.Style = ObjectStyle;

                FormattedText formattedText = paragraph.AddFormattedText(current.Name);
                formattedText.Font.Color = declarationColours.InterfaceDeclarationColour;

                paragraph = section.AddParagraph($"Definition: {current.Definition}");
                paragraph.Style = ObjectDefinitionStyle;

                if (current.Properties != null && current.Properties.Length > 0)
                {
                    paragraph = section.AddParagraph(" Properties: ");
                    paragraph.Style = MemberHeading;
                    WriteVariablesToPDF(current.Properties, declarationColours, section);
                }

                if (current.Methods != null && current.Methods.Length > 0)
                {
                    paragraph = section.AddParagraph(" Methods & Functions: ");
                    paragraph.Style = MemberHeading;
                    WriteVariablesToPDF(current.Methods, declarationColours, section);
                }
            }
        }

        private Styles InitialiseDocumentStylesForPDF(Styles styles, DocumentInformation styling)
        {
            Style style = styles.AddStyle(ObjectStyle, StyleNames.Normal);
            style.Font.Size = styling.DeclarationFonts.ObjectDeclarationStyle.FontSize;
            style.Font.Bold = styling.DeclarationFonts.ObjectDeclarationStyle.IsBold;
            style.Font.Italic = styling.DeclarationFonts.ObjectDeclarationStyle.IsItalic;
            style.Font.Name = styling.DeclarationFonts.FontFamilyName;
            style.ParagraphFormat.SpaceAfter = styling.DeclarationFonts.ObjectDeclarationStyle.SpaceAfter;

            style = styles.AddStyle(ObjectDefinitionStyle, StyleNames.Normal);
            style.Font.Size = styling.DeclarationFonts.ObjectDefinitionStyle.FontSize;
            style.Font.Bold = styling.DeclarationFonts.ObjectDefinitionStyle.IsBold;
            style.Font.Italic = styling.DeclarationFonts.ObjectDefinitionStyle.IsItalic;
            style.Font.Name = styling.DeclarationFonts.FontFamilyName;
            style.ParagraphFormat.SpaceAfter = styling.DeclarationFonts.ObjectDefinitionStyle.SpaceAfter;

            style = styles.AddStyle(MemberHeading, StyleNames.Normal);
            style.Font.Size = styling.DeclarationFonts.MemberHeadingStyle.FontSize;
            style.Font.Bold = styling.DeclarationFonts.MemberHeadingStyle.IsBold;
            style.Font.Italic = styling.DeclarationFonts.MemberHeadingStyle.IsItalic;
            style.Font.Name = styling.DeclarationFonts.FontFamilyName;
            style.ParagraphFormat.SpaceAfter = styling.DeclarationFonts.MemberHeadingStyle.SpaceAfter;

            style = styles.AddStyle(MemberStyle, StyleNames.Normal);
            style.Font.Size = styling.DeclarationFonts.MemberStyle.FontSize;
            style.Font.Bold = styling.DeclarationFonts.MemberStyle.IsBold;
            style.Font.Italic = styling.DeclarationFonts.MemberStyle.IsItalic;
            style.Font.Name = styling.DeclarationFonts.FontFamilyName;
            style.ParagraphFormat.SpaceAfter = styling.DeclarationFonts.MemberStyle.SpaceAfter;

            style = styles.AddStyle(MemberTypeStyle, StyleNames.Normal);
            style.Font.Size = styling.DeclarationFonts.MemberTypeStyle.FontSize;
            style.Font.Bold = styling.DeclarationFonts.MemberTypeStyle.IsBold;
            style.Font.Italic = styling.DeclarationFonts.MemberTypeStyle.IsItalic;
            style.Font.Name = styling.DeclarationFonts.FontFamilyName;
            style.ParagraphFormat.SpaceAfter = styling.DeclarationFonts.MemberTypeStyle.SpaceAfter;

            style = styles.AddStyle(MemberDefinitionStyle, StyleNames.Normal);
            style.Font.Size = styling.DeclarationFonts.MemberDefinitionStyle.FontSize;
            style.Font.Bold = styling.DeclarationFonts.MemberDefinitionStyle.IsBold;
            style.Font.Italic = styling.DeclarationFonts.MemberDefinitionStyle.IsItalic;
            style.Font.Name = styling.DeclarationFonts.FontFamilyName;
            style.ParagraphFormat.SpaceAfter = styling.DeclarationFonts.MemberDefinitionStyle.SpaceAfter;

            return styles;
        }

        private void WriteEnumsToPDF(EnumDeclaration[] enums, Color enumColour, Document document)
        {
            Section section;
            foreach (EnumDeclaration current in enums)
            {
                section = document.AddSection();
                Paragraph paragraph = section.AddParagraph($"Enum ");
                paragraph.AddBookmark(current.Name);
                paragraph.Style = ObjectStyle;

                FormattedText formattedText = paragraph.AddFormattedText(current.Name);
                formattedText.Font.Color = enumColour;

                paragraph = section.AddParagraph($"Definition: {current.Definition}");
                paragraph.Style = ObjectDefinitionStyle;
                if (current.EnumMembers.Length > 0)
                {
                    WriteEnumMembersToPDF(current.EnumMembers, section);
                }
            }
        }

        private void WriteEnumMembersToPDF(Declaration[] enumMembers, Section section)
        {
            Paragraph paragraph = section.AddParagraph($" Members: ");
            paragraph.Style = MemberHeading;

            foreach (Declaration member in enumMembers)
            {
                paragraph = section.AddParagraph($" {member.Name} - ");
                paragraph.Style = MemberStyle;

                if (member.Definition == null) { continue; }

                FormattedText formatted = paragraph.AddFormattedText(member.Definition);
                formatted.Style = MemberDefinitionStyle;
            }
        }

        private void WriteClassesToPDF(ClassDeclaration[] classDeclarations, DeclarationColours declarationColours, Document document, bool printBaseTypes)
        {
            Section section;
            foreach (ClassDeclaration current in classDeclarations)
            {
                section = document.AddSection();
                Paragraph paragraph = section.AddParagraph($"Class ");
                paragraph.AddBookmark(current.Name);
                paragraph.Style = ObjectStyle;

                FormattedText formatted = paragraph.AddFormattedText(current.Name);
                formatted.Font.Color = declarationColours.ClassDeclarationColour;

                if (printBaseTypes && current.BaseTypes != null && current.BaseTypes.Length > 0)
                {
                    WriteClassInheritancesAndInterfacesToPDF(current, paragraph, declarationColours);
                }

                paragraph = section.AddParagraph($"Definition: {current.Definition}");

                paragraph.Style = ObjectDefinitionStyle;

                if (current.Properties != null && current.Properties.Length > 0)
                {
                    paragraph = section.AddParagraph($" Properties: ");
                    paragraph.Style = MemberHeading;
                    WriteVariablesToPDF(current.Properties, declarationColours, section);
                }

                if (current.Fields != null && current.Fields.Length > 0)
                {
                    paragraph = section.AddParagraph($" Fields: ");
                    paragraph.Style = MemberHeading;
                    WriteVariablesToPDF(current.Fields, declarationColours, section);
                }

                if (current.Methods != null && current.Methods.Length > 0)
                {
                    paragraph = section.AddParagraph($" Methods & Functions: ");
                    paragraph.Style = MemberHeading;
                    WriteMethodsToPDF(current.Methods, declarationColours, section);
                }
            }
        }

        private void WriteClassInheritancesAndInterfacesToPDF(ClassDeclaration current, Paragraph paragraph, DeclarationColours declarationColours)
        {
            FormattedText formatted;

            if (current.BaseTypes[0][0] == 'I')
            {
                WriteInterfaceImplementationsToPDF(false, current, paragraph, declarationColours);
                return;
            }
            else
            {
                formatted = paragraph.AddFormattedText($"\nInherits ");
                formatted.Style = ObjectStyle;
                formatted = paragraph.AddFormattedText($"{current.BaseTypes[0]}");
                formatted.Color = declarationColours.ClassDeclarationColour;
            }

            if (current.BaseTypes.Length > 1)
            {
                WriteInterfaceImplementationsToPDF(true, current, paragraph, declarationColours);
            }
        }

        private void WriteInterfaceImplementationsToPDF(bool skipFirst, ClassDeclaration current, Paragraph paragraph, DeclarationColours declarationColours)
        {
            string interfaces = "";
            if (!skipFirst)
            {
                interfaces = $"{current.BaseTypes[0]}";
                interfaces += ", ";
            }
            if (current.BaseTypes.Length > 1)
            {
                for (int i = 1; i < current.BaseTypes.Length; i++)
                {
                    if (i == current.BaseTypes.Length - 1)
                    {
                        interfaces += $"{current.BaseTypes[i]}";
                    }
                    else
                    {
                        interfaces += $"{current.BaseTypes[i]}, ";
                    }
                }
            }

            FormattedText formatted = paragraph.AddFormattedText($"\nImplements ");
            formatted.Style = ObjectStyle;

            formatted = paragraph.AddFormattedText($"{interfaces}");
            formatted.Color = declarationColours.InterfaceDeclarationColour;
        }

        private void WriteMethodsToPDF(Declaration[] methods, DeclarationColours declarationColours, Section section)
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

        private void WriteVariablesToPDF(Declaration[] fields, DeclarationColours declarationColours, Section section)
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
                        case ObjectType.Struct: paragraph.Format.Font.Color = declarationColours.ClassDeclarationColour; break;
                    }
                }


                FormattedText formatted = paragraph.AddFormattedText($"{field.Name} - ");
                formatted.Style = MemberStyle;

                if (field.Definition == null) { continue; }
                formatted = paragraph.AddFormattedText(field.Definition);
                formatted.Style = MemberDefinitionStyle;

            }
        }
        #endregion

        #region HTML
        public void WriteHTMLDocumentation(string path, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docInfo)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) { return; }
            DirectoryInfo outputPath = Directory.CreateDirectory(Path.Combine(path, "Documentation"));
            GenerateHomePage(outputPath.FullName, classes,enums,interfaces,structs, docInfo);
            if (docInfo.GenerateRelationshipGraph)
            {
                // string graphPath = GenerateRelationshipGraph(classes, interfaces, documentStyling.DeclarationColours, path);
            }

            Directory.CreateDirectory(Path.Combine(path, "objs"));

            //if (classes != null && classes.Length > 0)
            //{

            //}

            //if (structs != null && structs.Length > 0)
            //{

            //}

            //if (interfaces != null && interfaces.Length > 0)
            //{

            //}

            //if (enums != null && enums.Length > 0)
            //{

            //}
        }

        private string GenerateHomePage(string outputPath, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docInfo)
        {
            string filePath = Path.Combine(outputPath, "index.html");

            File.Create(filePath).Close();
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/helper.js"), Path.Combine(outputPath, "helper.js"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/homePageStyles.css"), Path.Combine(outputPath, "homePageStyles.css"), true);
            StreamWriter streamWriter = new StreamWriter(filePath, false);

            
            streamWriter.Write(@$"<!DOCTYPE html>
                <html lang=""en"">

                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>{filePath}</title>
                    <link rel=""stylesheet"" href=""./homePageStyles.css"">
                    <script src=""./helper.js""></script>
                </head>
                ");

            GenerateSideBar(streamWriter,classes,enums,interfaces,structs);

            streamWriter.Write(@$"<div class=""content"">
                        <h1>{docInfo.ProjectName}</h1>
                        <p>{docInfo.ProjectDescription}</p>
                    </div>
                </body>
                ");

            streamWriter.Write("<html>");
            streamWriter.Close();
            streamWriter.Dispose();

            return filePath;
        }

        private void GenerateSideBar(StreamWriter streamWriter, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs)
        {

            streamWriter.Write(@$"<body>
            <div class=""sidebar"">
                <a href=""./""><h2>My Project</h2></a>
                <div class=""nav-section"">
                    <button onclick=""toggleMenu('classes')"">Classes ▼</button>
                    <div id=""classes"" class=""nav-links"">
                    ");

            if (classes != null)
            {
                foreach (ClassDeclaration current in classes)
                {
                    streamWriter.Write(@$"<a href=""./objs/{current.Name}.html"">{current.Name}</a>");
                }
            }

            streamWriter.Write(@"       </div>
                    </div>
                </div>
                ");
            // <body>
            //         <!-- Sidebar -->
            //         <div class=""sidebar"">
            //             <a href=""./""><h2>My Project</h2></a>
            //             <div class=""nav-section"">
            //                 <button onclick=""toggleMenu('classes')"">Classes ▼</button>
            //                 <div id=""classes"" class=""nav-links"">
            //                     <a href=""./objs/npc.html"">NPC</a>
            //                     <a href=""player.html"">Player</a>
            //                     <a href=""enemy.html"">Enemy</a>
            //                 </div>
            //             </div>
            //         </div>

            //         <!-- Main Content -->
            //         <div class=""content"">
            //             <h1>Welcome to My Project</h1>
            //             <p>This project is a top-down open-world zombie game. Explore the docs using the sidebar.</p>
            //         </div>
            //     </body>

        }
        #endregion

        private string GenerateRelationshipGraph(ClassDeclaration[]? classes, InterfaceDeclaration[]? interfaces, DeclarationColours declarationColours, string path)
        {
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("");

            if (classes != null && classes.Length > 0)
            {
                foreach (ClassDeclaration dec in classes)
                {
                    if (dec.BaseTypes != null && dec.BaseTypes.Length > 0)
                    {
                        foreach (string type in dec.BaseTypes)
                        {
                            Microsoft.Msagl.Drawing.Edge edge = graph.AddEdge(dec.Name, type);
                            edge.SourceNode.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.ClassDeclarationColour);
                            if (type[0] == 'I')
                            {
                                edge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.InterfaceDeclarationColour);
                            }
                            else
                            {
                                edge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.ClassDeclarationColour);
                            }
                        }
                    }
                }
            }

            if (interfaces != null && interfaces.Length > 0)
            {
                foreach (InterfaceDeclaration dec in interfaces)
                {

                    Microsoft.Msagl.Drawing.Node node = graph.AddNode(dec.Name);
                    node.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.InterfaceDeclarationColour);

                }
            }


            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
            renderer.CalculateLayout();
            int width = 1920;
            int height = 1080;
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            renderer.Render(bitmap);
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            string imagePath = "";
            if (directoryInfo.Parent != null)
            {
                imagePath = Path.Combine(directoryInfo.Parent.FullName, "graph.png");

                bitmap.Save(imagePath);
            }
            else
            {
                Debug.WriteLine($"Directory '{directoryInfo.Parent}' does not exist, therefore could not save the image.");
            }
            return imagePath;
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