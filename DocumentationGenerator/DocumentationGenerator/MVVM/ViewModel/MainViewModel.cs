using DocumentationGenerator.MVVM.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentationGenerator.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string output;
        private string fileName;
        private OpenFileDialog openFileDialog;

        public string Output
        {
            get { return output; }
            set
            {
                output = value;
                OnPropertyChanged();
            }
        }

        public string FileName {  get { return fileName; } set { fileName = value; OnPropertyChanged(); } }

        public ICommand LoadFileCommand { get; set; }


        public MainViewModel()
        {
            LoadFileCommand = new RelayCommand(LoadFile);
            output = "";
            fileName = "";
            FileName = "This is where the file name you selected will be shown";
            Output = "This is where the Documentation output will be shown";
            openFileDialog = new OpenFileDialog();
        }

        private void LoadFile()
        {
            bool? valid = openFileDialog.ShowDialog();

            if(valid.HasValue && valid.Value == true)
            {
                ReadSourceFileAsync(openFileDialog.FileName);
                FileName = openFileDialog.SafeFileName;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ReadSourceFileAsync(string sourceFile)
        {
            string rawCode = await File.ReadAllTextAsync(sourceFile);

            string tempOutput = "";

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(rawCode);

            SyntaxNode root = syntaxTree.GetRoot();
            IEnumerable<SyntaxNode> nodes = root.ChildNodes();
            foreach (SyntaxNode node in nodes)
            {
                bool alteration = false;
                switch (node)
                {
                    case ClassDeclarationSyntax classDec:
                        tempOutput += HandleClassDeclaration(classDec);
                        alteration = true;
                        break;

                    case EnumDeclarationSyntax enumDec:
                        tempOutput += HandleEnumDeclaration(enumDec);
                        alteration = true;
                        break;

                }
                if(alteration)
                {
                    tempOutput += Environment.NewLine + "========================" + Environment.NewLine;
                }
            }

            Output = tempOutput;
        }

        private string HandleEnumDeclaration(EnumDeclarationSyntax enumDec)
        {
            string tempOutput = "";
            tempOutput = $"Enum: {enumDec.Identifier.Text}" + Environment.NewLine;
            foreach (EnumMemberDeclarationSyntax member in enumDec.Members)
            {
                tempOutput += $"    {member.Identifier.Text} - {GetXML(member, XmlTag.summary)}" + Environment.NewLine;
            }

            return tempOutput;
        }

        private string HandleClassDeclaration(ClassDeclarationSyntax classDec)
        {
            string tempOutput = "";
            tempOutput = $"Class: {classDec.Identifier.Text} - {GetXML(classDec, XmlTag.summary)}" + Environment.NewLine;

            IEnumerable<PropertyDeclarationSyntax> properties = classDec.Members.OfType<PropertyDeclarationSyntax>();
            if (properties.Count() > 0) { tempOutput += "Properties: " + Environment.NewLine; }

            foreach (PropertyDeclarationSyntax property in properties)
            {
                tempOutput += $"  {property.Type} {property.Identifier.Text} - {GetXML(property, XmlTag.summary)}" + Environment.NewLine;
            }

            IEnumerable<FieldDeclarationSyntax> fields = classDec.Members.OfType<FieldDeclarationSyntax>();
            if (fields.Count() > 0) { tempOutput += "Fields: " + Environment.NewLine; }

            foreach (FieldDeclarationSyntax field in fields)
            {
                foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                {
                    tempOutput += $"  {field.Declaration.Type} {variable.Identifier.Text} - {GetXML(field, XmlTag.summary)}" + Environment.NewLine;
                }
            }

            IEnumerable<MethodDeclarationSyntax> methods = classDec.Members.OfType<MethodDeclarationSyntax>();
            if (methods.Count() > 0) { tempOutput += "Methods: " + Environment.NewLine; }

            foreach (MethodDeclarationSyntax method in methods)
            {
                tempOutput += $"  {method.Identifier} {method.ParameterList}, Returns {method.ReturnType} - {GetXML(method, XmlTag.summary)} - {GetXML(method, XmlTag.returns)}" + Environment.NewLine;
            }

            return tempOutput;
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

    public enum XmlTag
    {
        summary,
        returns

    }
}
