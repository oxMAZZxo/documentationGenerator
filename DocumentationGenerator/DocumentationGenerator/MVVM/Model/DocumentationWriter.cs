using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model.DocumentationWriters;
using Microsoft.CodeAnalysis;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;

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

        public bool WriteDocumentation(DocumentationType type, string outputPath, ClassDeclaration[]? classDeclarations, EnumDeclaration[]? enumDeclarations, InterfaceDeclaration[]? interfaceDeclarations, StructDeclaration[]? structDeclarations, DocumentInformation docInfo)
        {
            bool valid = false;
            if (docInfo.GenerateRelationshipGraph)
            {
                docInfo.GlobalRelationshipGraphPath = GenerateRelationshipGraph(classDeclarations, interfaceDeclarations, docInfo.DeclarationColours, outputPath);
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