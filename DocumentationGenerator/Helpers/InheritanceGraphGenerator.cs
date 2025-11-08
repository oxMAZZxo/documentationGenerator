using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Avalonia.Media.Imaging;
using DocumentationGenerator.Models.Declarations;
using DocumentationGenerator.Models.DocumentInfo;
using SkiaSharp;

namespace DocumentationGenerator.Helpers;


public static class InheritanceGraphGenerator
{
    public static Bitmap GenerateGlobalGraph(ClassDeclaration[]? classes, InterfaceDeclaration[]? interfaces, DeclarationColours declarationColours)
    {
        DirectedGraph graph = new DirectedGraph("Global Inheritance Graph");
        if (classes != null && classes.Length > 0)
        {
            foreach (ClassDeclaration dec in classes)
            {
                if (dec.BaseTypes != null && dec.BaseTypes.Length > 0)
                {
                    foreach (string type in dec.BaseTypes)
                    {
                        Edge currentEdge = graph.AddEdge(type,dec.Name);
                        currentEdge.Child.ShapePaint = new SKPaint{ Color = SKColors.Teal, StrokeWidth = 2, IsAntialias = true };
                        if (type[0] == 'I')
                        {
                            currentEdge.Parent.ShapePaint = new SKPaint { Color = SKColors.Orange, StrokeWidth = 2, IsAntialias = true };
                        }
                        else
                        {
                            currentEdge.Parent.ShapePaint = new SKPaint { Color = SKColors.Teal, StrokeWidth = 2, IsAntialias = true };
                        }
                    }
                }
            }
        }

        if (interfaces != null && interfaces.Length > 0)
        {
            foreach (InterfaceDeclaration dec in interfaces)
            {
                Node node = graph.AddNode(dec.Name);

                node.ShapePaint = new SKPaint { Color = SKColors.Orange, StrokeWidth = 2, IsAntialias = true };
            }
        }

    
        return DirectedGraphRenderer.RenderGraph(graph);
    }

    /// <summary>
    /// Generate Individual Graphs for each Class Declaration, in the process assigning the right colours to the nodes, rendering each graph and saving it in the specified outputPath
    /// </summary>
    /// <param name="classes"></param>
    /// <param name="declarationColours"></param>
    /// <param name="outputPath"></param>
    /// <returns>Returns a Dictionary of string to string key value pairs, where the key is the name of the Class, and the value being a bitmap which contains the rendered graph.</returns>
    public static Dictionary<string, Bitmap> GenerateIndividualGraphs(ClassDeclaration[]? classes, DeclarationColours declarationColours)
    {
        // Dictionary<string, Graph> graphs = new Dictionary<string, Graph>();


        if (classes != null)
        {
            Dictionary<string, ClassDeclaration> classDictionary = classes.ToDictionary(x => x.Name, x => x);
            for (int i = 0; i < classes.Length; i++)
            {
                // Graph graph;
                // if (classes[i].BaseTypes != null && classes[i].BaseTypes.Length > 0)
                // {
                //     graph = new Graph();
                //     HandleBaseTypes(graph, classes[i], classDictionary, declarationColours);
                //     graphs.Add(classes[i].Name, graph);
                // }
            }
        }

        Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();//RenderGraphs(graphs);

        return bitmaps;
    }

    // private static void HandleBaseTypes(Graph graph, ClassDeclaration current, Dictionary<string, ClassDeclaration> sourceClasses, DeclarationColours declarationColours)
    // {
    //     foreach (string b in current.BaseTypes)
    //     {
    //         // Edge edge = graph.AddEdge(current.Name, b);
    //         // edge.SourceNode.Attr.FillColor = Utilities.MigraDocColourToMsaglColor(declarationColours.ClassDeclarationColour);

    //         if (b[0] == 'I')
    //         {
    //             // edge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMsaglColor(declarationColours.InterfaceDeclarationColour);
    //         }
    //         else
    //         {
    //             // edge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMsaglColor(declarationColours.ClassDeclarationColour);
    //         }

    //         if (sourceClasses.ContainsKey(b))
    //         {
    //             HandleBaseTypes(graph, sourceClasses[b], sourceClasses, declarationColours);
    //         }
    //     }
    // }

    /// <returns>Returns a Dictionary of string to string key value pairs, where the key is the name of the Class, and the value being a bitmap which contains the rendered graph.</returns>
    // private static Dictionary<string, Bitmap> RenderGraphs(Dictionary<string, Graph> graphs)
    // {
    //     Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();

    //     foreach (string graphName in graphs.Keys)
    //     {
    //         // Graph graph = graphs[graphName];
    //         // GraphRenderer renderer = new GraphRenderer(graph);

    //         // renderer.CalculateLayout();

    //         // Bitmap bitmap = new Bitmap(Avalonia.Platform.PixelFormat.Rgb32, Avalonia.Platform.AlphaFormat.Opaque, null,
    //         // new Avalonia.PixelSize(Convert.ToInt32(graph.BoundingBox.Width * 2), Convert.ToInt32(graph.BoundingBox.Width * 2)), Avalonia.Vector.One, 3);

    //         // renderer.Render(bitmap);
    //         // bitmaps.Add(graphName, bitmap);

    //     }

    //     return bitmaps;
    // }
}