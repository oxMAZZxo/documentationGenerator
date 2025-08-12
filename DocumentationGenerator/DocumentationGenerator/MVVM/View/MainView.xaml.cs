using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace DocumentationGenerator.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private MainViewModel viewModel;

        public MainView()
        {
            InitializeComponent();

            viewModel = new MainViewModel(this);
            this.DataContext = viewModel;

            viewModel.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FileName")
            {
                UpdateRichTextBox();
            }
        }

        private void UpdateRichTextBox()
        {
            if (SettingsModel.Instance == null) { System.Windows.MessageBox.Show("Cannot Display Preview since the settings have not been initialised."); return; }

            ParsedSourceResults parsedSourceResults = viewModel.GetAllSourceResults();
            DeclarationColours declarationColours = new DeclarationColours(SettingsModel.Instance.MigraDocClassDeclarationColour,
                    SettingsModel.Instance.MigraDocEnumDeclarationColour, SettingsModel.Instance.MigraDocPrimitiveDeclarationColour,
                    SettingsModel.Instance.MigraDocInterfaceDeclarationColour, SettingsModel.Instance.MigraDocStructDeclarationColour);

             UpdatePreviewRichTextBox(parsedSourceResults,declarationColours);
        }

        public void ShowDefaultPreviewMessage()
        {
            if (SettingsModel.Instance == null) { Debug.WriteLine($"Settings Model IS NULL"); return; }
            myRichTextBox.Document.Blocks.Clear();

            // Create a paragraph for the message
            Paragraph paragraph = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(20)
            };

            Color color = SettingsModel.Instance.PrimitiveDeclarationColour;
            var brush = new SolidColorBrush(color);
            brush.Freeze();

            Run run = new Run("The source that is loaded will be displayed here.")
            {
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = brush
            };

            paragraph.Inlines.Add(run);
            myRichTextBox.Document.Blocks.Add(paragraph);
        }

        /// <summary>
        /// Helper local function for consistent Run creation.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="bold">Indicates whether the text is bold.</param>
        /// <param name="colour">The colour of the text.</param>
        /// <param name="italic">Indicates whether the text is italic.</param>
        /// <returns></returns>
        private Run MakeRun(string text, double size, bool bold, Color? colour = null, bool italic = false)
        {
            var run = new Run(text)
            {
                FontSize = size,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                FontStyle = italic ? FontStyles.Italic : FontStyles.Normal
            };
            if (colour.HasValue)
            {
                var brush = new SolidColorBrush(colour.Value);
                brush.Freeze();
                run.Foreground = brush;
            }
            return run;
        }

        private void AddDefinition(Paragraph p, string def)
        {
            p.Inlines.Add(MakeRun($"Definition: {def}", 18, false, null, true));
        }

        private void UpdatePreviewRichTextBox(ParsedSourceResults results, DeclarationColours colours)
        {
            myRichTextBox.Document.Blocks.Clear();

            WriteClassesPreview(results.Classes, colours);
            WriteInterfacesPreview(results.Interfaces, colours);
            WriteEnumsPreview(results.Enums, colours);
            WriteStructsPreview(results.Structs, colours);
        }

        private void WriteClassesPreview(List<ClassDeclaration> classes, DeclarationColours colours)
        {
            foreach (ClassDeclaration currentClass in classes)
            {
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Class ", 20, true));
                p.Inlines.Add(MakeRun(currentClass.Name, 20, true, SettingsModel.Instance?.ClassDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, currentClass.Definition ?? "NO SUMMARY");
                myRichTextBox.Document.Blocks.Add(defP);

                WriteProperties(currentClass.Properties, colours);

                WriteFields(currentClass.Fields, colours);

                WriteMethods(currentClass.Methods, colours);
            }
        }

        private void WriteInterfacesPreview(List<InterfaceDeclaration> interfaces, DeclarationColours colours)
        {
            foreach (InterfaceDeclaration currentInterface in interfaces)
            {
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Interface ", 20, true));
                p.Inlines.Add(MakeRun(currentInterface.Name, 20, true, SettingsModel.Instance?.InterfaceDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, currentInterface.Definition ?? "NO SUMMARY");
                myRichTextBox.Document.Blocks.Add(defP);

                WriteProperties(currentInterface.Properties, colours);
                WriteMethods(currentInterface.Methods, colours);

            }
        }

        private void WriteEnumsPreview(List<EnumDeclaration> enums, DeclarationColours colours)
        {
            foreach (EnumDeclaration currentEnum in enums)
            {
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Enum ", 20, true));
                p.Inlines.Add(MakeRun(currentEnum.Name, 20, true, SettingsModel.Instance?.EnumDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, currentEnum.Definition ?? "NO SUMMARY");
                myRichTextBox.Document.Blocks.Add(defP);

                WriteEnumMembers(currentEnum.EnumMembers);
            }
        }

        private void WriteStructsPreview(List<StructDeclaration> structs, DeclarationColours colours)
        {
            foreach (StructDeclaration currentStruct in structs)
            {
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Struct ", 20, true));
                p.Inlines.Add(MakeRun(currentStruct.Name, 20, true, SettingsModel.Instance?.StructDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, currentStruct.Definition ?? "NO SUMMARY");
                myRichTextBox.Document.Blocks.Add(defP);

                WriteProperties(currentStruct.Properties, colours);
                WriteFields(currentStruct.Fields, colours);
                WriteMethods(currentStruct.Methods, colours);
            }
        }

        /// <summary>
        /// Helper to pick colour based on ObjectType
        /// </summary>
        /// <param name="decl"></param>
        /// <param name="colours"></param>
        /// <returns></returns>
        private Color? GetTypeColour(Declaration decl, DeclarationColours colours)
        {
            if (SettingsModel.Instance == null)
            {
                return null;
            }

            Color col = SettingsModel.Instance.PrimitiveDeclarationColour;

            if (decl.IsTypePrimitive.HasValue && decl.IsTypePrimitive.Value == false)
            {
                switch (decl.WhatIsType)
                {
                    case ObjectType.Class: col = SettingsModel.Instance.ClassDeclarationColour; break;
                    case ObjectType.Enum: col = SettingsModel.Instance.EnumDeclarationColour; break;
                    case ObjectType.Interface: col = SettingsModel.Instance.InterfaceDeclarationColour; break;
                    case ObjectType.Struct: col = SettingsModel.Instance.StructDeclarationColour; break;
                }
            }
            
            return col;
        }

        private void WriteProperties(Declaration[]? properties, DeclarationColours colours)
        {
            if (properties == null || properties.Length == 0) return;

            foreach (var prop in properties)
            {
                var propP = new Paragraph();
                propP.Inlines.Add(MakeRun("Property: ", 16, true));
                propP.Inlines.Add(MakeRun($"{prop.Type} ", 14, true, GetTypeColour(prop, colours)));
                propP.Inlines.Add(MakeRun($"{prop.Name} - ", 14, false));
                propP.Inlines.Add(MakeRun(prop.Definition ?? "NO SUMMARY", 14, false, null, true));
                myRichTextBox.Document.Blocks.Add(propP);
            }
        }

        private void WriteFields(Declaration[]? fields, DeclarationColours colours)
        {
            if (fields == null || fields.Length == 0) return;

            foreach (var field in fields)
            {
                var fieldP = new Paragraph();
                fieldP.Inlines.Add(MakeRun("Field: ", 16, true));
                fieldP.Inlines.Add(MakeRun($"{field.Type} ", 14, true, GetTypeColour(field, colours)));
                fieldP.Inlines.Add(MakeRun($"{field.Name} - ", 14, false));
                fieldP.Inlines.Add(MakeRun(field.Definition ?? "NO SUMMARY", 14, false, null, true));
                myRichTextBox.Document.Blocks.Add(fieldP);
            }
        }

        private void WriteMethods(Declaration[]? methods, DeclarationColours colours)
        {
            if (methods == null || methods.Length == 0) return;

            foreach (var m in methods)
            {
                var methodP = new Paragraph();
                methodP.Inlines.Add(MakeRun("Method: ", 16, true));
                methodP.Inlines.Add(MakeRun($"{m.Type} ", 14, true, GetTypeColour(m, colours)));
                methodP.Inlines.Add(MakeRun($"{m.Name} - ", 14, false));
                methodP.Inlines.Add(MakeRun(m.Definition ?? "NO SUMMARY", 14, false, null, true));
                if (!string.IsNullOrEmpty(m.ReturnDefinition))
                {
                    methodP.Inlines.Add(MakeRun($" - {m.ReturnDefinition}", 14, false, null, true));
                }
                myRichTextBox.Document.Blocks.Add(methodP);
            }
        }

        private void WriteEnumMembers(Declaration[]? members)
        {
            if (members == null || members.Length == 0) return;

            foreach (var mem in members)
            {
                var memP = new Paragraph();
                memP.Inlines.Add(MakeRun("Member: ", 16, true));
                memP.Inlines.Add(MakeRun($"{mem.Name} - ", 14, false));
                memP.Inlines.Add(MakeRun(mem.Definition ?? "NO SUMMARY", 14, false, null, true));
                myRichTextBox.Document.Blocks.Add(memP);
            }
        }

        private void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            FilePopup.IsOpen = true;
        }
    }
}
