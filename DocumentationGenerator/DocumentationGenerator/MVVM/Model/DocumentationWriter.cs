using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model.DocumentationWriters;
using System.Diagnostics;
using System.IO;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Drawing;

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

        private string GenerateGlobalRelationshipGraph(ClassDeclaration[]? classes, InterfaceDeclaration[]? interfaces, DeclarationColours declarationColours, string path)
        {
            Graph globalGraph = new Graph("");

            if (classes != null && classes.Length > 0)
            {
                foreach (ClassDeclaration dec in classes)
                {
                    if (dec.BaseTypes != null && dec.BaseTypes.Length > 0)
                    {
                        foreach (string type in dec.BaseTypes)
                        {
                            Edge edge = globalGraph.AddEdge(dec.Name, type);
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

                    Node node = globalGraph.AddNode(dec.Name);
                    node.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.InterfaceDeclarationColour);

                }
            }


            GraphRenderer renderer = new GraphRenderer(globalGraph);
            renderer.CalculateLayout();
            int width = 1920;
            int height = 1080;
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

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

        public bool WriteDocumentation(DocumentationType type, string outputPath, ClassDeclaration[]? classDeclarations, EnumDeclaration[]? enumDeclarations, InterfaceDeclaration[]? interfaceDeclarations, StructDeclaration[]? structDeclarations, DocumentInformation docInfo)
        {
            bool valid = false;
            if (docInfo.GenerateRelationshipGraph)
            {
                docInfo.GlobalRelationshipGraphPath = GenerateGlobalRelationshipGraph(classDeclarations, interfaceDeclarations, docInfo.DeclarationColours, outputPath);
            }

            if(type == DocumentationType.PDF)
            {
                valid = pdfWriter.Write(outputPath, classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
            }
            else
            {
                valid = htmlWriter.Write(outputPath, classDeclarations, enumDeclarations, interfaceDeclarations, structDeclarations, docInfo);
            }

            if(docInfo.GenerateRelationshipGraph && docInfo.GlobalRelationshipGraphPath != null)
            {
                File.Delete(docInfo.GlobalRelationshipGraphPath);
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