using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ReadingSourceCodeApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        ReadSourceFile("C:\\Users\\mario\\Documents\\VS Code\\ReadingSourceFiles\\ReadingSourceCodeApp\\Player.cs");
        Console.ReadLine();
    }

    static void ReadSourceFile(string sourceFile)
    {
        string code = File.ReadAllText(sourceFile);

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

        SyntaxNode root = syntaxTree.GetRoot();

        foreach (SyntaxNode node in root.ChildNodes())
        {
            Console.WriteLine($"==============");
            switch (node)
            {
                case ClassDeclarationSyntax classDec:

                    Console.WriteLine($"Class: {classDec.Identifier.Text} - {GetXMLSummary(classDec)}");


                    IEnumerable<PropertyDeclarationSyntax> properties = classDec.Members.OfType<PropertyDeclarationSyntax>();
                    foreach (PropertyDeclarationSyntax property in properties)
                    {
                        Console.WriteLine($"Property: {property.Type} {property.Identifier.Text} - {GetXMLSummary(property)}");
                    }
                    IEnumerable<MethodDeclarationSyntax> methods = classDec.Members.OfType<MethodDeclarationSyntax>();
                    foreach (MethodDeclarationSyntax method in methods)
                    {
                        Console.WriteLine($"Method: {method.Identifier} {method.ParameterList}, Returns {method.ReturnType} - {GetXMLSummary(method)} - {GetXMLReturnsSummary(method)}");
                    }
                    break;

                case EnumDeclarationSyntax enumDec:
                    Console.WriteLine($"Enum: {enumDec.Identifier.Text}");

                    foreach (EnumMemberDeclarationSyntax member in enumDec.Members)
                    {
                        Console.WriteLine($"    {member.Identifier.Text}");
                    }
                    break;

            }


        }
    }

    static string GetXMLSummary(SyntaxNode node)
    {
        SyntaxToken token = node.GetFirstToken();
        SyntaxTriviaList triviaList = token.LeadingTrivia;

        foreach (SyntaxTrivia trivia in triviaList)
        {
            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                SyntaxNode? structure = trivia.GetStructure();
                if (structure == null) { return " NO SUMMARY "; }

                XmlElementSyntax? summary = structure.ChildNodes()
                    .OfType<XmlElementSyntax>()
                    .FirstOrDefault(e => e.StartTag.Name.LocalName.Text == "summary");

                if (summary != null)
                {
                    return CleanXmlSummary(summary.GetText().ToString(), "summary");
                }
            }
        }

        return " NO SUMMARY ";
    }

    static string GetXMLReturnsSummary(SyntaxNode node)
    {
        SyntaxToken token = node.GetFirstToken();
        SyntaxTriviaList trivialist = token.LeadingTrivia;

        foreach (SyntaxTrivia trivia in trivialist)
        {
            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                SyntaxNode? structure = trivia.GetStructure();
                if (structure == null) { return " NO RETURN SUMMARY "; }

                XmlElementSyntax? returnSummary = structure.ChildNodes()
                    .OfType<XmlElementSyntax>()
                    .FirstOrDefault(e => e.StartTag.Name.LocalName.Text == "returns");

                if (returnSummary != null)
                {
                    return CleanXmlSummary(returnSummary.GetText().ToString(), "returns");
                }
            }
        }
        return " NO RETURN SUMMARY";
    }

    static string CleanXmlSummary(string rawComment, string tag)
    {
        var lines = rawComment.Split('\n')
                              .Select(line => line.Trim())
                              .Where(line => !string.IsNullOrWhiteSpace(line))
                              .Select(line =>
                              {
                                  if (line.StartsWith("///"))
                                      line = line.Substring(3).Trim();
                                  line = line.Replace($"<{tag}>", "")
                                             .Replace($"</{tag}>", "")
                                             .Trim();
                                  return line;
                              });

        return string.Join(" ", lines);
    }

}
