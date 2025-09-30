using DocumentationGenerator.MVVM.Model;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace DocumentationGenerator.MVVM.Helpers
{
    public static class InheritanceGraphGenerator
    {
        public static Bitmap GenerateGlobalGraph(ClassDeclaration[]? classes, InterfaceDeclaration[]? interfaces, DeclarationColours declarationColours)
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
                            Edge globalEdge = globalGraph.AddEdge(dec.Name, type);

                            globalEdge.SourceNode.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.ClassDeclarationColour);

                            if (type[0] == 'I')
                            {
                                globalEdge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.InterfaceDeclarationColour);
                            }
                            else
                            {
                                globalEdge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.ClassDeclarationColour);
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
            
            return bitmap;
        }

        /// <summary>
        /// Generate Individual Graphs for each Class Declaration, in the process assigning the right colours to the nodes, rendering each graph and saving it in the specified outputPath
        /// </summary>
        /// <param name="classes"></param>
        /// <param name="declarationColours"></param>
        /// <param name="outputPath"></param>
        /// <returns>Returns a Dictionary of string to string key value pairs, where the key is the name of the Class, and the value being a bitmap which contains the rendered graph.</returns>
        public static Dictionary<string,Bitmap> GenerateIndividualGraphs(ClassDeclaration[]? classes, DeclarationColours declarationColours)
        {
           
            Dictionary<string, Graph> graphs = new Dictionary<string, Graph>();
            

            if(classes != null)
            {
                Dictionary<string, ClassDeclaration> classDictionary = classes.ToDictionary(x => x.Name, x => x);
                for(int i = 0; i < classes.Length; i++)
                {
                    Graph graph;
                    if (classes[i].BaseTypes != null && classes[i].BaseTypes.Length > 0)
                    {
                        graph = new Graph();
                        HandleBaseTypes(graph, classes[i], classDictionary, declarationColours);
                        graphs.Add(classes[i].Name,graph);
                    }
                }
            }

            Dictionary<string, Bitmap> bitmaps = RenderGraphs(graphs);

            return bitmaps;
        }

        private static void HandleBaseTypes(Graph graph, ClassDeclaration current, Dictionary<string, ClassDeclaration> sourceClasses, DeclarationColours declarationColours)
        {
            foreach (string b in current.BaseTypes)
            {
                Edge edge = graph.AddEdge(current.Name, b);
                edge.SourceNode.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.ClassDeclarationColour);

                if (b[0] == 'I')
                {
                    edge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.InterfaceDeclarationColour);
                }
                else
                {
                    edge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMSAGLColour(declarationColours.ClassDeclarationColour);
                }

                if(sourceClasses.ContainsKey(b))
                {
                    HandleBaseTypes(graph, sourceClasses[b], sourceClasses, declarationColours);
                }
            }
        }

        /// <returns>Returns a Dictionary of string to string key value pairs, where the key is the name of the Class, and the value being a bitmap which contains the rendered graph.</returns>
        private static Dictionary<string, Bitmap> RenderGraphs(Dictionary<string, Graph> graphs)
        {
            Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();

            foreach(string graphName in graphs.Keys)
            {
                Graph graph = graphs[graphName];
                GraphRenderer renderer = new GraphRenderer(graph);

                renderer.CalculateLayout();

                Bitmap bitmap = new Bitmap(800, 800, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                renderer.Render(bitmap);
                bitmaps.Add(graphName, bitmap);
               
            }

            return bitmaps;
        }
    }
}
