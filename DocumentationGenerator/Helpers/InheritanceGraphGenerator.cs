using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace DocumentationGenerator.Helpers;

/// <summary>
/// Renders a simple hierarchical tree graph into a bitmap image.
/// Nodes are arranged top-down, centered among their children, and drawn with
/// rounded rectangles and connecting lines.
/// </summary>
public static class SimpleTreeRenderer
{
    /// <summary>
    /// Represents a single node in the tree hierarchy.
    /// </summary>
    public class Node
    {
        /// <summary>Display text of the node.</summary>
        public string Name { get; }

        /// <summary>Measured width of the node (based on text).</summary>
        public float Width { get; set; }

        /// <summary>Measured height of the node (based on text).</summary>
        public float Height { get; set; }

        /// <summary>Child nodes below this node.</summary>
        public List<Node> Children { get; } = new();

        public SKPaint ShapePaint { get; }
        public SKPaint TextPaint { get; }


        public Node(string name, SKPaint shapePaint, SKPaint textPaint)
        {
            Name = name;
            ShapePaint = shapePaint;
            TextPaint = textPaint;
        }
    }

    /// <summary>
    /// Small example runner to build a sample tree and output it as PNG.
    /// </summary>
    public static void Run()
    {
        SKPaint textPaint = new SKPaint { Color = SKColors.White, IsAntialias = true };
        SKPaint shapePaint = new SKPaint { Color = SKColors.Teal, IsAntialias = true, StrokeWidth= 2 };

        // Build the example node tree
        var A = new Node("NetworkBehaviour", shapePaint, textPaint);
        var B = new Node("NPC", shapePaint, textPaint);
        var C = new Node("Zombie", shapePaint, textPaint);
        var D = new Node("BigZombie", shapePaint, textPaint);
        var F = new Node("PlayerCombat", shapePaint, textPaint);
        var G = new Node("IDamageable", shapePaint, textPaint);

        A.Children.Add(B);
        B.Children.Add(C);
        B.Children.Add(G);
        C.Children.Add(D);
        A.Children.Add(F);
        F.Children.Add(G);

        // Render and save
        var bitmap = DrawTree(A);
        using FileStream fs = File.OpenWrite(Path.Combine(AppContext.BaseDirectory, "tree.png"));
        bitmap.Save(fs);
    }

    /// <summary>
    /// Draws the given root node and its hierarchy into a bitmap.
    /// </summary>
    public static Bitmap DrawTree(Node root, bool inverted = false)
    {
        // Stores the computed (x,y) positions of each node
        var positions = new Dictionary<Node, Vector2>();

        // Used to measure text and determine node sizes
        SKFont font = new SKFont(SKTypeface.Default, 16);

        // Measure each node based on text to determine width/height
        MeasureTreeGraph(root, font);

        int maxDepth = GetMaxDepth(root);

        // Compute layout positions
        LayoutNodes(root, 0, positions, 150, maxDepth, inverted);

        // Determine required canvas size
        Vector2 boundingBox = FindBoundingBox(positions);
        int imgW = (int)(boundingBox.X + 100);
        int imgH = (int)(boundingBox.Y + 100);

        // Create draw surface
        using SKBitmap skBitmap = new SKBitmap(imgW, imgH);
        using SKCanvas canvas = new SKCanvas(skBitmap);
        canvas.Clear(SKColors.White);

        SKPaint linePaint = new SKPaint { Color = SKColors.Black, StrokeWidth = 2, IsAntialias = true };

        // Draw tree edges first (so nodes appear above them)
        DrawEdges(positions, canvas, linePaint);

        // Draw node shapes and text
        DrawNodes(positions, canvas, font);

        // Encode Skia bitmap → PNG → Avalonia Bitmap
        using SKData data = skBitmap.Encode(SKEncodedImageFormat.Png, 100);
        using Stream ms = data.AsStream();
        return new Bitmap(ms);
    }

    /// <summary>
    /// Finds the rightmost and bottom-most node to compute image bounds.
    /// </summary>
    private static Vector2 FindBoundingBox(Dictionary<Node, Vector2> positions)
    {
        float maxX = 0;
        float maxY = 0;

        foreach (Vector2 p in positions.Values)
        {
            if (p.X > maxX) maxX = p.X;
            if (p.Y > maxY) maxY = p.Y;
        }

        return new Vector2(maxX, maxY);
    }

    /// <summary>
    /// Measures node width & height from text so rectangles can be drawn fitting the label.
    /// This must be called before layout so we can center nodes properly.
    /// </summary>
    private static void MeasureTreeGraph(Node node, SKFont font)
    {
        font.MeasureText(node.Name, out SKRect bounds);

        node.Width = bounds.Width + 20;
        node.Height = bounds.Height + 20;

        foreach (Node child in node.Children)
            MeasureTreeGraph(child, font);
    }

    /// <summary>
    /// Recursively assigns each node an (x,y) position.
    /// Leaf nodes are spaced horizontally.
    /// Internal nodes are positioned centered between their children.
    /// </summary>
    private static float LayoutNodes(Node node, int depth, Dictionary<Node, Vector2> pos, float spacing, int maxDepth, bool inverted)
    {
        if (node.Children.Count == 0)
        {
            pos[node] = new Vector2(CurrentLeafX, depth * 100 + 50);
            CurrentLeafX += spacing;
            return pos[node].X;
        }

        float minX = float.MaxValue;
        float maxX = float.MinValue;

        foreach (var child in node.Children)
        {
            float childX = LayoutNodes(child, depth + 1, pos, spacing,maxDepth, inverted);
            if (childX < minX) minX = childX;
            if (childX > maxX) maxX = childX;
        }

        float nodeX = (minX + maxX) / 2;
        float nodeY = 0;
        
        if(inverted)
        {
            nodeY = (maxDepth - depth) * 100 + 50;
        }
        else
        {
            nodeY = depth * 100 + 50;
        }

        pos[node] = new Vector2(nodeX, nodeY);
        return nodeX;
    }

    /// <summary>
    /// Draws node rectangles and centered text.
    /// </summary>
    private static void DrawNodes(Dictionary<Node, Vector2> positions, SKCanvas canvas, SKFont font)
    {
        foreach (KeyValuePair<Node,Vector2> kvp in positions)
        {
            Node node = kvp.Key;
            Vector2 pos = kvp.Value;

            float left = pos.X - node.Width / 2;
            float top = pos.Y - node.Height / 2;
            float right = left + node.Width;
            float bottom = top + node.Height;

            SKRect rect = new SKRect(left, top, right, bottom);
            canvas.DrawRoundRect(rect, 8, 8, node.ShapePaint);

            font.MeasureText(node.Name, out SKRect bounds);

            float textX = pos.X - bounds.Width / 2;
            float textY = pos.Y + bounds.Height / 2;

            canvas.DrawText(node.Name, textX, textY, font, node.TextPaint);
        }
    }

    /// <summary>
    /// Draws connecting lines between parent and child nodes.
    /// </summary>
    private static void DrawEdges(Dictionary<Node, Vector2> positions, SKCanvas canvas, SKPaint linePaint)
    {
        foreach (var kvp in positions)
        {
            Node node = kvp.Key;
            Vector2 p = kvp.Value;

            foreach (Node child in node.Children)
            {
                Vector2 cp = positions[child];
                canvas.DrawLine(p.X, p.Y, cp.X, cp.Y, linePaint);
            }
        }
    }

    private static int GetMaxDepth(Node node)
    {
        if (node.Children.Count == 0)
            return 0;

        int max = 0;
        foreach (var child in node.Children)
        {
            int depth = GetMaxDepth(child);
            if (depth > max) max = depth;
        }
        return max + 1;
    }


    /// <summary>
    /// Tracks current X position for placing leaf nodes.
    /// </summary>
    private static float CurrentLeafX = 50;

}


public static class InheritanceGraphGenerator
{
    // public static Bitmap GenerateGlobalGraph(ClassDeclaration[]? classes, InterfaceDeclaration[]? interfaces, DeclarationColours declarationColours)
    // {
    //     Graph globalGraph = new Graph();

    //     if (classes != null && classes.Length > 0)
    //     {
    //         foreach (ClassDeclaration dec in classes)
    //         {
    //             if (dec.BaseTypes != null && dec.BaseTypes.Length > 0)
    //             {
    //                 foreach (string type in dec.BaseTypes)
    //                 {
    //                     Edge globalEdge = globalGraph.AddEdge(dec.Name, type);

    //                     globalEdge.SourceNode.Attr.FillColor = new Color();

    //                     if (type[0] == 'I')
    //                     {
    //                         globalEdge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMsaglColor(declarationColours.InterfaceDeclarationColour);
    //                     }
    //                     else
    //                     {
    //                         globalEdge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMsaglColor(declarationColours.ClassDeclarationColour);
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     if (interfaces != null && interfaces.Length > 0)
    //     {
    //         foreach (InterfaceDeclaration dec in interfaces)
    //         {
    //             Node node = globalGraph.AddNode(dec.Name);

    //             node.Attr.FillColor = Utilities.MigraDocColourToMsaglColor(declarationColours.InterfaceDeclarationColour);
    //         }
    //     }

    //     return new Bitmap("");
    // }

    // /// <summary>
    // /// Generate Individual Graphs for each Class Declaration, in the process assigning the right colours to the nodes, rendering each graph and saving it in the specified outputPath
    // /// </summary>
    // /// <param name="classes"></param>
    // /// <param name="declarationColours"></param>
    // /// <param name="outputPath"></param>
    // /// <returns>Returns a Dictionary of string to string key value pairs, where the key is the name of the Class, and the value being a bitmap which contains the rendered graph.</returns>
    // public static Dictionary<string, Bitmap> GenerateIndividualGraphs(ClassDeclaration[]? classes, DeclarationColours declarationColours)
    // {

    //     Dictionary<string, Graph> graphs = new Dictionary<string, Graph>();


    //     if (classes != null)
    //     {
    //         Dictionary<string, ClassDeclaration> classDictionary = classes.ToDictionary(x => x.Name, x => x);
    //         for (int i = 0; i < classes.Length; i++)
    //         {
    //             Graph graph;
    //             if (classes[i].BaseTypes != null && classes[i].BaseTypes.Length > 0)
    //             {
    //                 graph = new Graph();
    //                 HandleBaseTypes(graph, classes[i], classDictionary, declarationColours);
    //                 graphs.Add(classes[i].Name, graph);
    //             }
    //         }
    //     }

    //     Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();//RenderGraphs(graphs);

    //     return bitmaps;
    // }

    // private static void HandleBaseTypes(Graph graph, ClassDeclaration current, Dictionary<string, ClassDeclaration> sourceClasses, DeclarationColours declarationColours)
    // {
    //     foreach (string b in current.BaseTypes)
    //     {
    //         Edge edge = graph.AddEdge(current.Name, b);
    //         edge.SourceNode.Attr.FillColor = Utilities.MigraDocColourToMsaglColor(declarationColours.ClassDeclarationColour);

    //         if (b[0] == 'I')
    //         {
    //             edge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMsaglColor(declarationColours.InterfaceDeclarationColour);
    //         }
    //         else
    //         {
    //             edge.TargetNode.Attr.FillColor = Utilities.MigraDocColourToMsaglColor(declarationColours.ClassDeclarationColour);
    //         }

    //         if (sourceClasses.ContainsKey(b))
    //         {
    //             HandleBaseTypes(graph, sourceClasses[b], sourceClasses, declarationColours);
    //         }
    //     }
    // }

    // /// <returns>Returns a Dictionary of string to string key value pairs, where the key is the name of the Class, and the value being a bitmap which contains the rendered graph.</returns>
    // private static Dictionary<string, Bitmap> RenderGraphs(Dictionary<string, Graph> graphs)
    // {
    //     Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();

    //     foreach (string graphName in graphs.Keys)
    //     {
    //         Graph graph = graphs[graphName];
    //         GraphRenderer renderer = new GraphRenderer(graph);

    //         renderer.CalculateLayout();

    //         // Bitmap bitmap = new Bitmap(Avalonia.Platform.PixelFormat.Rgb32, Avalonia.Platform.AlphaFormat.Opaque, null,
    //         // new Avalonia.PixelSize(Convert.ToInt32(graph.BoundingBox.Width * 2), Convert.ToInt32(graph.BoundingBox.Width * 2)), Avalonia.Vector.One, 3);

    //         // renderer.Render(bitmap);
    //         // bitmaps.Add(graphName, bitmap);

    //     }

    //     return bitmaps;
    // }
}