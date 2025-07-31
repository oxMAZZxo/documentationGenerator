using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Win32;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DocumentationGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private OpenFileDialog openFileDialog;

        public App()
        {
            openFileDialog = new OpenFileDialog();
            
            bool? valid = openFileDialog.ShowDialog();
            if (valid.HasValue && valid.Value == true)
            {
                ReadSourceFile(openFileDialog.FileName);
            }
        }

        private void ReadSourceFile(string sourceFile)
        {
            string code = File.ReadAllText(sourceFile);
            Debug.WriteLine(code);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            SyntaxNode root = syntaxTree.GetRoot();
            IEnumerable<SyntaxNode> nodes = root.ChildNodes();
            foreach (SyntaxNode node in nodes)
            {
                Debug.WriteLine($"==============");
                switch (node)
                {
                    case ClassDeclarationSyntax classDec:
                        HandleClassDeclaration(classDec);
                        break;

                    case EnumDeclarationSyntax enumDec:
                        HandleEnumDeclaration(enumDec);
                        break;

                }


            }
        }

        private void HandleEnumDeclaration(EnumDeclarationSyntax enumDec)
        {
            Debug.WriteLine($"Enum: {enumDec.Identifier.Text}");

            foreach (EnumMemberDeclarationSyntax member in enumDec.Members)
            {
                Debug.WriteLine($"    {member.Identifier.Text} - {GetXML(member,XmlTag.summary)}");
            }
        }

        private void HandleClassDeclaration(ClassDeclarationSyntax classDec)
        {
            Debug.WriteLine($"Class: {classDec.Identifier.Text} - {GetXML(classDec,XmlTag.summary)}");

            IEnumerable<PropertyDeclarationSyntax> properties = classDec.Members.OfType<PropertyDeclarationSyntax>();
            foreach (PropertyDeclarationSyntax property in properties)
            {
                Debug.WriteLine($"  Property: {property.Type} {property.Identifier.Text} - {GetXML(property, XmlTag.summary)}");
            }

            IEnumerable<FieldDeclarationSyntax> fields = classDec.Members.OfType<FieldDeclarationSyntax>();

            foreach(FieldDeclarationSyntax field in fields)
            { 
                foreach(VariableDeclaratorSyntax variable in field.Declaration.Variables)
                {
                    Debug.WriteLine($"  Field: {field.Declaration.Type} {variable.Identifier.Text} - {GetXML(field, XmlTag.summary)}");
                }
            }

            IEnumerable<MethodDeclarationSyntax> methods = classDec.Members.OfType<MethodDeclarationSyntax>();
            foreach (MethodDeclarationSyntax method in methods)
            {
                Debug.WriteLine($"  Method: {method.Identifier} {method.ParameterList}, Returns {method.ReturnType} - {GetXML(method, XmlTag.summary)} - {GetXML(method, XmlTag.returns)}");
            }
        }

        private string GetXML(SyntaxNode node, XmlTag tag)
        {
            SyntaxToken token = node.GetFirstToken();
            SyntaxTriviaList triviaList = token.LeadingTrivia;

            foreach (SyntaxTrivia trivia in triviaList)
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    SyntaxNode? structure = trivia.GetStructure();
                    if (structure == null) { return $" NO {tag.ToString().ToUpper()} "; }

                    XmlElementSyntax? summary = structure.ChildNodes()
                        .OfType<XmlElementSyntax>()
                        .FirstOrDefault(e => e.StartTag.Name.LocalName.Text == tag.ToString());

                    if (summary != null)
                    {
                        return CleanXmlSummary(summary.GetText().ToString(), tag);
                    }
                }
            }

            return $" NO {tag.ToString().ToUpper()} ";
        }

        private string CleanXmlSummary(string rawComment, XmlTag tag)
        {
            var lines = rawComment.Split('\n')
                                  .Select(line => line.Trim())
                                  .Where(line => !string.IsNullOrWhiteSpace(line))
                                  .Select(line =>
                                  {
                                      if (line.StartsWith("///"))
                                          line = line.Substring(3).Trim();
                                      line = line.Replace($"<{tag.ToString()}>", "")
                                                 .Replace($"</{tag.ToString()}>", "")
                                                 .Trim();
                                      return line;
                                  });

            return string.Join(" ", lines);
        }
    }

    public enum XmlTag {
        summary,
        returns

    }




}
