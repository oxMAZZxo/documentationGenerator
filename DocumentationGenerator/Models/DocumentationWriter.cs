using System.Threading.Tasks;
using DocumentationGenerator.Helpers;
using DocumentationGenerator.Models.Declarations;
using DocumentationGenerator.Models.DocumentInfo;
using DocumentationGenerator.Models.DocumentWriters;

namespace DocumentationGenerator.Models;

public class DocumentationWriter
{
    private PdfWriter pdfWriter;
    private HtmlWriter htmlWriter;

    public DocumentationWriter()
    {
        pdfWriter = new PdfWriter();
        htmlWriter = new HtmlWriter();
    }

    public async Task<bool> WriteDocumentationAsync(DocumentationType type, ClassDeclaration[]? classDeclarations, EnumDeclaration[]? enumDeclarations, InterfaceDeclaration[]? interfaceDeclarations, StructDeclaration[]? structDeclarations, DocumentInformation docInfo)
    {
        if (docInfo.GenerateInheritanceGraphs)
        {
            docInfo.GlobalInheritanceGraph = InheritanceGraphGenerator.GenerateGlobalGraph(classDeclarations, interfaceDeclarations, docInfo.DeclarationColours);
            
            // docInfo.IndividualObjsGraphs = InheritanceGraphGenerator.GenerateIndividualGraphs(classDeclarations,docInfo.DeclarationColours);
        }

        bool valid = false;
        if (type == DocumentationType.PDF)
        {
            valid = await pdfWriter.WriteAsync(classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
        }
        else
        {
            valid = await htmlWriter.WriteAsync(classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
        }

        if(App.Instance != null && App.Instance.TopLevel != null)
        {
            await App.Instance.TopLevel.Launcher.LaunchUriAsync(docInfo.SavePath.Path);
        }


        return valid;
    }

}

public enum DocumentationType
{
    PDF,
    HTML
}