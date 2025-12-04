using System;
using System.Collections.Generic;
using System.Linq;
using DocumentationGenerator.Helpers;
using DocumentationGenerator.Models.Declarations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace DocumentationGenerator.Models.LanguageParsers;

public static class VbDeclarationsParser
{
    public static ParsedSourceResults ReadAllDeclarations(SyntaxTree syntaxTree)
    {
        CompilationUnitSyntax root = (CompilationUnitSyntax)syntaxTree.GetRoot();
        ParsedSourceResults results = new ParsedSourceResults();

        foreach (StatementSyntax node in root.Members)
        {
            switch (node)
            {
                case ClassBlockSyntax classBlock:
                    results.Classes.Add(HandleClassBlockSyntax(classBlock));
                    break;
            }
        }

        return results;
    }

    private static ClassDeclaration HandleClassBlockSyntax(ClassBlockSyntax classBlock)
    {
        TypeStatementSyntax header = classBlock.BlockStatement;
        string className = header.Identifier.Text;

        Declaration[]? newProperties;
        Declaration[]? newfields;
        Declaration[]? newMethods;

        PropertyBlockSyntax[] properties = classBlock.Members.OfType<PropertyBlockSyntax>().ToArray();
        FieldDeclarationSyntax[] fields = classBlock.Members.OfType<FieldDeclarationSyntax>().ToArray();
        MethodBlockSyntax[] methods = classBlock.Members.OfType<MethodBlockSyntax>().ToArray();

        newProperties = GetPropertyDeclarations(properties);
        newfields = GetFieldsDeclarations(fields);
        newMethods = GetMethodsDeclarations(methods);

        return new ClassDeclaration(className, GetXML(classBlock,XmlTag.summary), [], newMethods, newfields, newProperties);
    }

    private static Declaration[]? GetFieldsDeclarations(FieldDeclarationSyntax[] fields)
    {
        if(fields.Length < 0) {return null;}

        Declaration[] declarations = new Declaration[fields.Length];

        int index = 0;
        foreach (FieldDeclarationSyntax field in fields)
        {
            foreach(VariableDeclaratorSyntax variable in field.Declarators)
            {
                declarations[index] = new Declaration(variable.Names[0].ToString(),GetXML(field, XmlTag.summary), variable.AsClause.Type().ToString());
            }
            index++;
        }

        return declarations;
    }

    private static Declaration[]? GetMethodsDeclarations(MethodBlockSyntax[] methods)
    {
        if(methods.Length < 0) {return null;}

        Declaration[] declarations = new Declaration[methods.Length];

        int index = 0;
        foreach (MethodBlockSyntax method in methods)
        {
            MethodStatementSyntax header = method.SubOrFunctionStatement;

            declarations[index] = new Declaration(header.Identifier.Text, GetXML(method,XmlTag.summary),header.AsClause?.Type.ToString() ?? "Void", GetXML(method,XmlTag.returns));
            index++;
        }

        return declarations;
    }

    private static Declaration[]? GetPropertyDeclarations(PropertyBlockSyntax[] properties)
    {
        if(properties.Length < 0) {return null;}

        Declaration[] declarations = new Declaration[properties.Length];

        int index = 0;
        foreach (PropertyBlockSyntax property in properties)
        {
            declarations[index] = new Declaration(property.PropertyStatement.Identifier.Text, GetXML(property,XmlTag.summary),property.PropertyStatement.AsClause.Type().ToString(), null, null, null);
            index++;
        }

        return declarations;
    }

    /// <summary>
    /// Attempts to find the XML comment for the given syntax node.
    /// </summary>
    /// <param name="node">The node that may contain a XML comment trivia.</param>
    /// <param name="tag">The type of XML comment to look for.</param>
    /// <returns>Returns a string which may contain the XML Comment, or an indication that theres no valid comment.</returns>
    private static string GetXML(SyntaxNode node, XmlTag tag)
    {
        SyntaxToken token = node.GetFirstToken();
        SyntaxTriviaList triviaList = token.LeadingTrivia;

        foreach (SyntaxTrivia trivia in triviaList)
        {
            if (trivia.IsKind(SyntaxKind.DocumentationCommentTrivia))
            {
                SyntaxNode? structure = trivia.GetStructure();
                if (structure == null) { return $" NO {tag.ToString().ToUpper()} "; }

                XmlElementSyntax? summary = structure.ChildNodes()
                    .OfType<XmlElementSyntax>()
                    .FirstOrDefault(e => e.StartTag.Name.GetText().ToString() == tag.ToString());

                if (summary != null)
                {
                    return CleanXML(summary.GetText().ToString(), tag);
                }
            }
        }

        return $" NO {tag.ToString().ToUpper()} ";
    }

    /// <summary>
    /// Cleans the XML Comment by removing the comment characters (such as '/') and returns the declaration itself.
    /// </summary>
    /// <param name="rawComment">The XML comment.</param>
    /// <param name="tag">The type of XML (returns, summary, etc.)</param>
    /// <returns></returns>
    private static string CleanXML(string rawComment, XmlTag tag)
    {
        var lines = rawComment.Split('\n')
                              .Select(line => line.Trim())
                              .Where(line => !string.IsNullOrWhiteSpace(line))
                              .Select(line =>
                              {
                                  if (line.StartsWith("'''"))
                                      line = line.Substring(3).Trim();
                                  line = line.Replace($"<{tag.ToString()}>", "")
                                             .Replace($"</{tag.ToString()}>", "")
                                             .Trim();
                                  return line;
                              });

        return string.Join(" ", lines);
    }
}