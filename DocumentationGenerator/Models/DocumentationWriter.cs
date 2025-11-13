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
            docInfo.IndividualObjsGraphs = InheritanceGraphGenerator.GenerateIndividualGraphs(classDeclarations, docInfo.DeclarationColours);
        }

        bool success;
        if (type == DocumentationType.PDF)
        {
            success = await pdfWriter.WriteAsync(classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
            if (success && SettingsModel.Instance.KeepGraphFilesPostPDFGeneration == false && docInfo.GraphFolder != null)
            {
                await Utilities.DeleteFolder(docInfo.GraphFolder);
            }
        }
        else
        {
            success = await htmlWriter.WriteAsync(classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
        }

        if (success && App.Instance != null && App.Instance.TopLevel != null)
        {
            await App.Instance.TopLevel.Launcher.LaunchUriAsync(docInfo.SavePath.Path);
        }
        return success;
    }
}

public enum DocumentationType
{
    PDF,
    HTML
}