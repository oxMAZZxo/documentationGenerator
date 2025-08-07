using System;
using System.Diagnostics;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocumentationGenerator.MVVM.Model
{
    public class SourceFileReader : IDisposable
    {
        public List<ClassDeclaration> Classes { get; private set; }
        public List<EnumDeclaration> Enums { get; private set; }

        public SourceFileReader()
        {
            Classes = new List<ClassDeclaration>();
            Enums = new List<EnumDeclaration>();
        }

        ~SourceFileReader()
        {
            Dispose();
        }

        public async Task ReadSourceDirectory(string directory)
        {
            IEnumerable<string> filePaths = Directory.EnumerateFiles(directory, "*.cs", new EnumerationOptions { RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive, MatchType = MatchType.Simple });

            IEnumerable<Task<ParsedSourceResults>>? tasks = filePaths.Select(path => ReadSourceFileAsync(path));

            ParsedSourceResults[] results = await Task.WhenAll(tasks);

            foreach (ParsedSourceResults parsedSourceResults in results)
            {
                Classes.AddRange(parsedSourceResults.Classes);
                Enums.AddRange(parsedSourceResults.Enums);
            }
        }

        public async Task ReadSourceFilesAsync(string[] fileNames)
        {
            IEnumerable<Task<ParsedSourceResults>>? tasks = fileNames.Select(path => ReadSourceFileAsync(path));

            ParsedSourceResults[] results = await Task.WhenAll(tasks);

            foreach (ParsedSourceResults parsedSourceResults in results)
            {
                Classes.AddRange(parsedSourceResults.Classes);
                Enums.AddRange(parsedSourceResults.Enums);
            }
        }

        private async Task<ParsedSourceResults> ReadSourceFileAsync(string sourceFile)
        {
            string rawCode = await File.ReadAllTextAsync(sourceFile);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(rawCode);

            SyntaxNode root = syntaxTree.GetRoot();
            IEnumerable<SyntaxNode> nodes = root.ChildNodes();

            ParsedSourceResults results = new ParsedSourceResults();


            foreach (SyntaxNode node in nodes)
            {
                switch (node)
                {
                    case ClassDeclarationSyntax classDec:

                        results.Classes.Add(HandleClassDeclaration(classDec));

                        break;

                    case EnumDeclarationSyntax enumDec:

                        results.Enums.Add(HandleEnumDeclaration(enumDec));

                        break;

                }
            }

            return results;
        }

        private EnumDeclaration HandleEnumDeclaration(EnumDeclarationSyntax enumDec)
        {
            Declaration[] enumMembers = new Declaration[enumDec.Members.Count()];
            int index = 0;

            foreach (EnumMemberDeclarationSyntax member in enumDec.Members)
            {
                enumMembers[index] = new Declaration(member.Identifier.Text, GetXML(member, XmlTag.summary),null, null);
                index++;
            }

            return new EnumDeclaration(enumDec.Identifier.Text, GetXML(enumDec, XmlTag.summary), enumMembers);
        }

        private ClassDeclaration HandleClassDeclaration(ClassDeclarationSyntax classDec)
        {
            string className = classDec.Identifier.Text;
            string classDefinition = GetXML(classDec, XmlTag.summary);

            IEnumerable<PropertyDeclarationSyntax> properties = classDec.Members.OfType<PropertyDeclarationSyntax>();
            IEnumerable<FieldDeclarationSyntax> fields = classDec.Members.OfType<FieldDeclarationSyntax>();
            IEnumerable<MethodDeclarationSyntax> methods = classDec.Members.OfType<MethodDeclarationSyntax>();

            Declaration[]? newProperties = null;
            Declaration[]? newFields = null;
            Declaration[]? newMethods = null;
            int index = 0;

            if (properties.Count() > 0)
            {
                newProperties = new Declaration[properties.Count()];
                foreach (PropertyDeclarationSyntax property in properties)
                {
                    newProperties[index] = new Declaration(property.Identifier.Text, 
                        GetXML(property, XmlTag.summary), property.Type.ToString(), null, IsPrimitiveType(property.Type.ToString()));
                    
                    index++;
                }
            }

            if (fields.Count() > 0)
            {
                index = 0;
                newFields = new Declaration[fields.Count()];

                foreach (FieldDeclarationSyntax field in fields)
                {
                    foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                    {
                        //tempOutput += $"  {field.Declaration.Type} {variable.Identifier.Text} - {GetXML(field, XmlTag.summary)}" + Environment.NewLine;
                        newFields[index] = new Declaration(variable.Identifier.Text, 
                            GetXML(field, XmlTag.summary), field.Declaration.Type.ToString(),null,IsPrimitiveType(field.Declaration.Type.ToString()));
                        index++;
                    }
                }

            }

            if (methods.Count() > 0)
            {
                newMethods = new Declaration[methods.Count()];

                index = 0;

                foreach (MethodDeclarationSyntax method in methods)
                {
                    newMethods[index] = new Declaration(method.Identifier.Text, GetXML(method, XmlTag.summary), 
                        method.ReturnType.ToString(), GetXML(method, XmlTag.returns),IsPrimitiveType(method.ReturnType.ToString()));
                    index++;
                }
            }

            return new ClassDeclaration(className, classDefinition, newMethods, newFields, newProperties);
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
                        return CleanXML(summary.GetText().ToString(), tag);
                    }
                }
            }

            return $" NO {tag.ToString().ToUpper()} ";
        }

        private string CleanXML(string rawComment, XmlTag tag)
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

        private bool IsPrimitiveType(string type)
        {
            if(type == "int") { return true; }
            if(type == "bool") { return true; }
            if(type == "sbyte") { return true; }
            if(type == "int16") { return true; }
            if(type == "uint16") { return true; }
            if(type == "int32") { return true; }
            if(type == "uint32") { return true; }
            if(type == "int64") { return true; }
            if(type == "uint64") { return true; }
            if(type == "single") { return true; }
            if(type == "double") { return true; }
            if(type == "char") { return true; }
            if(type == "float") { return true; }
            if(type == "void") { return true; }

            return false;
        }


        public void Clear()
        {
            Classes.Clear();
            Enums.Clear();
        }

        public string GetAllDeclarations()
        {
            string output = "";

            output += GetClassDeclarations();
            output += GetEnumDeclarations();
            return output;
        }

        public string GetClassDeclarations()
        {
            string output = "";
            if(Classes == null) { return ""; }

            foreach (ClassDeclaration classDeclaration in Classes)
            {
                output += $"Class: {classDeclaration.Name} - {classDeclaration.Definition}\n";
                if (classDeclaration.Properties != null)
                {
                    output += $"    Properties:\n";
                    foreach (Declaration declaration in classDeclaration.Properties)
                    {
                        output += $"    {declaration.Type} {declaration.Name} - {declaration.Definition}\n";
                    }

                }

                if (classDeclaration.Fields != null)
                {
                    output += $"    Fields:\n";
                    foreach (Declaration declaration in classDeclaration.Fields)
                    {
                        output += $"    {declaration.Type} {declaration.Name} - {declaration.Definition}\n";
                    }

                }

                if (classDeclaration.Methods != null)
                {
                    output += $"    Methods:\n";
                    foreach (Declaration declaration in classDeclaration.Methods)
                    {
                        output += $"    {declaration.Type} {declaration.Name} - {declaration.Definition} - {declaration.ReturnDefinition}\n";
                    }

                }
            }

            return output;
        }

        public string GetEnumDeclarations()
        {
            string output = "";

            if(Enums == null) { return ""; }

            foreach (EnumDeclaration enumDeclaration in Enums)
            {
                output += $"Enum: {enumDeclaration.Name} - {enumDeclaration.Definition}\n   Members:\n";
                
                foreach (Declaration declaration in enumDeclaration.EnumMembers)
                {
                    output += $"    {declaration.Name} - {declaration.Definition}\n";
                }
            }

            return output;
        }

        public void Dispose()
        {
            Clear();
            Classes = null;
            Enums = null;
        }
    }


    public enum XmlTag
    {
        summary,
        returns
    }

    public class ParsedSourceResults
    {
        public string FilePath { get; set; }
        public List<ClassDeclaration> Classes { get; set; }
        public List<EnumDeclaration> Enums { get; set; }

        public ParsedSourceResults()
        {
            FilePath = "";
            Classes = new List<ClassDeclaration>();
            Enums = new List<EnumDeclaration>();
        }
    }
}