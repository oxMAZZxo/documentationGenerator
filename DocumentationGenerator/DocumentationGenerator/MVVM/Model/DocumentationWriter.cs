using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model.DocumentationWriters;
using System.Diagnostics;
using System.IO;

namespace DocumentationGenerator.MVVM.Model
{
    public class DocumentationWriter
    {
        private PdfWriter pdfWriter;
        private HtmlWriter htmlWriter;

        public DocumentationWriter()
        {
            pdfWriter = new PdfWriter();
            htmlWriter = new HtmlWriter();
        }

        public bool WriteDocumentation(DocumentationType type, string outputPath, ClassDeclaration[]? classDeclarations, EnumDeclaration[]? enumDeclarations, InterfaceDeclaration[]? interfaceDeclarations, StructDeclaration[]? structDeclarations, DocumentInformation docInfo)
        {
            if (docInfo.GenerateInheritanceGraphs)
            {
                Debug.WriteLine($"Generating Graphs at this location: {outputPath}");
                docInfo.GlobalInheritanceGraph = InheritanceGraphGenerator.GenerateGlobalGraph(classDeclarations, interfaceDeclarations, docInfo.DeclarationColours);
                docInfo.IndividualObjsGraphs = InheritanceGraphGenerator.GenerateIndividualGraphs(classDeclarations,docInfo.DeclarationColours);
            
            }

            bool valid = false;
            if (type == DocumentationType.PDF)
            {
                valid = pdfWriter.Write(outputPath, classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
            }
            else
            {
                valid = htmlWriter.Write(outputPath, classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
            }

            return valid;
        }

    }

    public enum DocumentationType
    {
        PDF,
        HTML
    }

    
}