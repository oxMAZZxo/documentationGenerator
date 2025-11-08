using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Avalonia.Media.Imaging;
using SkiaSharp;

/// <summary>
/// Renders a simple hierarchical tree graph into a bitmap image.
/// Nodes are arranged top-down, centered among their children, and drawn with
/// rounded rectangles and connecting lines.
/// </summary>
public static class DirectedGraphRenderer
{
    /// <summary>
    /// Small example runner to build a sample tree and save it in the provided output path.
    /// </summary>
    public static void TestRun(string outputPath)
    {
        DirectedGraph g = new DirectedGraph("Example");
        g.AddEdge("A", "B");
        g.AddEdge("A", "C");
        g.AddEdge("C", "E");
        g.AddEdge("C", "F");
        g.AddEdge("C", "D");
        g.AddEdge("D", "G");

        var bitmap = RenderGraph(g);
        using FileStream fs = File.OpenWrite(outputPath);
        bitmap.Save(fs);
    }

    public static List<Node> FindRootNodes(List<Node> nodes)
    {
        var indegree = ComputeInDegree(nodes);
        return indegree.Where(x => x.Value == 0).Select(x => x.Key).ToList();
    }

    private static Dictionary<Node, int> ComputeIncomingCounts(List<Node> nodes)
    {
        var incoming = nodes.ToDictionary(n => n, n => 0);

        foreach (var n in nodes)
            foreach (var c in n.Children)
                incoming[c]++;

        return incoming;
    }

    /// <summary>
    /// Draws the given root node and its hierarchy into a bitmap.
    /// </summary>
    public static Bitmap RenderGraph(DirectedGraph graph, bool inverted = false)
    {
        List<Node> nodes = graph.Nodes.Values.ToList();
        // Used to measure text and determine node sizes

        Dictionary<Node, int> incoming = ComputeIncomingCounts(nodes);
        List<Node> roots = incoming.Where(p => p.Value == 0).Select(p => p.Key).ToList();

        // Compute Sugiyama layers
        List<List<Node>> layers = ComputeLayers(roots, nodes);

        // Convert layers -> (x,y) positions
        Dictionary<Node, Vector2> positions = ComputeNodePositions(layers);

        SKFont font = new SKFont(SKTypeface.Default, 16);
        foreach (Node node in nodes)
        {
            MeasureNode(node, font);
        }

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
    private static void MeasureNode(Node node, SKFont font)
    {
        font.MeasureText(node.Name, out SKRect bounds);

        node.Width = bounds.Width + 20;
        node.Height = bounds.Height + 20;

        foreach (Node child in node.Children)
            MeasureNode(child, font);
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
            float childX = LayoutNodes(child, depth + 1, pos, spacing, maxDepth, inverted);
            if (childX < minX) minX = childX;
            if (childX > maxX) maxX = childX;
        }

        float nodeX = (minX + maxX) / 2;
        float nodeY = 0;

        if (inverted)
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
        foreach (KeyValuePair<Node, Vector2> kvp in positions)
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

    private static List<Node> CollectNodes(Node root)
    {
        var result = new HashSet<Node>();
        var stack = new Stack<Node>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (result.Add(node))
            {
                foreach (var child in node.Children)
                    stack.Push(child);
            }
        }

        return result.ToList();
    }

    private static Dictionary<Node, int> ComputeInDegree(IEnumerable<Node> nodes)
    {
        var indegree = nodes.ToDictionary(n => n, n => 0);

        foreach (var node in nodes)
            foreach (var child in node.Children)
                indegree[child]++;

        return indegree;
    }

    private static List<List<Node>> ComputeLayers(List<Node> roots, List<Node> allNodes)
    {
        var layers = new List<List<Node>>();
        var assigned = new HashSet<Node>();

        Queue<(Node node, int depth)> queue = new();

        foreach (var r in roots)
            queue.Enqueue((r, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();

            if (layers.Count <= depth)
                layers.Add(new List<Node>());

            if (!assigned.Contains(node))
            {
                layers[depth].Add(node);
                assigned.Add(node);

                foreach (var child in node.Children)
                    queue.Enqueue((child, depth + 1));
            }
        }

        return layers;
    }

    private static Dictionary<Node, Vector2> ComputeNodePositions(List<List<Node>> layers)
    {
        var positions = new Dictionary<Node, Vector2>();

        float layerSpacing = 150;
        float nodeSpacing = 200;

        for (int depth = 0; depth < layers.Count; depth++)
        {
            var layer = layers[depth];
            float y = depth * layerSpacing + 50;

            for (int i = 0; i < layer.Count; i++)
            {
                float x = i * nodeSpacing + 100;
                positions[layer[i]] = new Vector2(x, y);
            }
        }

        return positions;
    }


    /// <summary>
    /// Tracks current X position for placing leaf nodes.
    /// </summary>
    private static float CurrentLeafX = 50;
}

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
    public List<Node> Children { get; }

    public SKPaint ShapePaint { get; set; }
    public SKPaint TextPaint { get; set; }


    public Node(string name)
    {
        Name = name;
        ShapePaint = new SKPaint { Color = SKColors.Black, StrokeWidth = 2, IsAntialias = true };
        TextPaint = new SKPaint { Color = SKColors.White, IsAntialias = true };
        Children = new List<Node>();
    }

    public Node(string name, SKPaint shapePaint, SKPaint textPaint)
    {
        Name = name;
        ShapePaint = shapePaint;
        TextPaint = textPaint;
        Children = new List<Node>();
    }
}

public class DirectedGraph
{
    public string Name { get; }
    public Dictionary<string, Node> Nodes { get; }
    public List<Edge> Edges { get; }

    public DirectedGraph(string name)
    {
        Name = name;
        Nodes = new Dictionary<string, Node>();
        Edges = new List<Edge>();
    }

    public Node AddNode(string name)
    {
        Node node;
        if (Nodes.ContainsKey(name))
        {
            node = Nodes[name];
        }
        else
        {
            node = new Node(name);
        }

        return node;
    }

    public Edge AddEdge(string parentName, string childName)
    {
        Node parent; Node child;

        if (Nodes.ContainsKey(parentName))
        {
            parent = Nodes[parentName];
        }
        else
        {
            parent = new Node(parentName);
            Nodes.Add(parentName, parent);

        }

        if (Nodes.ContainsKey(childName))
        {
            child = Nodes[childName];
        }
        else
        {
            child = new Node(childName);
            Nodes.Add(childName, child);

        }

        Edge edge = new Edge(parent, child);
        return edge;
    }

}

public class Edge
{
    public Node Parent { get; }
    public Node Child { get; }

    public Edge(Node parent, Node child)
    {
        Parent = parent;
        Child = child;
        Parent.Children.Add(child);
    }
}