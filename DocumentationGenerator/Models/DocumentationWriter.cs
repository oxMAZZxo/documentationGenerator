using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
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

        bool success = false;
        if (type == DocumentationType.PDF)
        {
            success = await pdfWriter.WriteAsync(classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
            if (success && SettingsModel.Instance.KeepGraphFilesPostPDFGeneration == false && docInfo.GraphFolder != null)
            {
                await DeleteGraphFolder(docInfo.GraphFolder);
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

    private async Task DeleteGraphFolder(IStorageFolder folder)
    {
        List<IStorageFile> files = await Utilities.EnumerateAllFilesAsync(folder);
        for (int i = 0; i < files.Count; i++)
        {
            await files[i].DeleteAsync();
        }

        files.Clear();
        await folder.DeleteAsync();
    }



}

public enum DocumentationType
{
    PDF,
    HTML
}