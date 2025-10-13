using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model.Declarations;
using DocumentationGenerator.MVVM.Model.DocumentationWriters;
using DocumentationGenerator.MVVM.Model.DocumentInfo;
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

        public bool WriteDocumentation(DocumentationType type, ClassDeclaration[]? classDeclarations, EnumDeclaration[]? enumDeclarations, InterfaceDeclaration[]? interfaceDeclarations, StructDeclaration[]? structDeclarations, DocumentInformation docInfo)
        {
            if (docInfo.GenerateInheritanceGraphs)
            {
                docInfo.GlobalInheritanceGraph = InheritanceGraphGenerator.GenerateGlobalGraph(classDeclarations, interfaceDeclarations, docInfo.DeclarationColours);
                docInfo.IndividualObjsGraphs = InheritanceGraphGenerator.GenerateIndividualGraphs(classDeclarations,docInfo.DeclarationColours);
            
            }

            bool valid = false;
            if (type == DocumentationType.PDF)
            {
                valid = pdfWriter.Write(classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
            }
            else
            {
                valid = htmlWriter.Write(classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
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