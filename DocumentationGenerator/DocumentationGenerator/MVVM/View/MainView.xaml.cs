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
            ShowDefaultPreviewMessage();
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

        private void ShowDefaultPreviewMessage()
        {
            if (SettingsModel.Instance == null) { System.Windows.MessageBox.Show("Cannot Display Preview since the settings have not been initialised."); return; }

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

            Run run = new Run("This is where the source results will be shown.")
            {
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = brush
            };

            paragraph.Inlines.Add(run);
            myRichTextBox.Document.Blocks.Add(paragraph);
        }


        private void UpdatePreviewRichTextBox(ParsedSourceResults results, DeclarationColours colours)
        {
            myRichTextBox.Document.Blocks.Clear();

            // Helper local function for consistent Run creation
            Run MakeRun(string text, double size, bool bold, Color? colour = null, bool italic = false)
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

            void AddDefinition(Paragraph p, string def)
            {
                p.Inlines.Add(MakeRun($"Definition: {def}", 18, false, null, true));
            }

            // CLASS
            if (results.Classes.Count > 0)
            {
                var c = results.Classes[0];
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Class ", 20, true));
                p.Inlines.Add(MakeRun(c.Name, 20, true, SettingsModel.Instance?.ClassDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, c.Definition ?? "NO SUMMARY");
                myRichTextBox.Document.Blocks.Add(defP);

                if (c.Properties != null && c.Properties.Length > 0)
                {
                    var prop = c.Properties[0];
                    var propP = new Paragraph();
                    propP.Inlines.Add(MakeRun("Property: ", 16, true));
                    propP.Inlines.Add(MakeRun($"{prop.Type} ", 14, true, GetTypeColour(prop, colours)));
                    propP.Inlines.Add(MakeRun($"{prop.Name} - ", 14, false));
                    propP.Inlines.Add(MakeRun(prop.Definition ?? "NO SUMMARY", 14, false, null, true));
                    myRichTextBox.Document.Blocks.Add(propP);
                }

                if (c.Fields != null && c.Fields.Length > 0)
                {
                    var field = c.Fields[0];
                    var fieldP = new Paragraph();
                    fieldP.Inlines.Add(MakeRun("Field: ", 16, true));
                    fieldP.Inlines.Add(MakeRun($"{field.Type} ", 14, true, GetTypeColour(field, colours)));
                    fieldP.Inlines.Add(MakeRun($"{field.Name} - ", 14, false));
                    fieldP.Inlines.Add(MakeRun(field.Definition ?? "NO SUMMARY", 14, false, null, true));
                    myRichTextBox.Document.Blocks.Add(fieldP);
                }

                if (c.Methods != null && c.Methods.Length > 0)
                {
                    var m = c.Methods[0];
                    var methodP = new Paragraph();
                    methodP.Inlines.Add(MakeRun("Method: ", 16, true));
                    methodP.Inlines.Add(MakeRun($"{m.Type} ", 14, true, GetTypeColour(m, colours)));
                    methodP.Inlines.Add(MakeRun($"{m.Name} - ", 14, false));
                    methodP.Inlines.Add(MakeRun(m.Definition ?? "NO SUMMARY", 14, false, null, true));
                    methodP.Inlines.Add(MakeRun($" - {m.ReturnDefinition ?? ""}", 14, false, null, true));
                    myRichTextBox.Document.Blocks.Add(methodP);
                }
            }

            // INTERFACE
            if (results.Interfaces.Count > 0)
            {
                var i = results.Interfaces[0];
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Interface ", 20, true));
                p.Inlines.Add(MakeRun(i.Name, 20, true, SettingsModel.Instance?.InterfaceDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, i.Definition ?? "NO SUMMARY");
                myRichTextBox.Document.Blocks.Add(defP);

                if (i.Properties != null && i.Properties.Length > 0)
                {
                    var prop = i.Properties[0];
                    var propP = new Paragraph();
                    propP.Inlines.Add(MakeRun("Property: ", 16, true));
                    propP.Inlines.Add(MakeRun($"{prop.Type} ", 14, true, GetTypeColour(prop, colours)));
                    propP.Inlines.Add(MakeRun($"{prop.Name} - ", 14, false));
                    propP.Inlines.Add(MakeRun(prop.Definition ?? "NO SUMMARY", 14, false, null, true));
                    myRichTextBox.Document.Blocks.Add(propP);
                }

                if (i.Methods != null && i.Methods.Length > 0)
                {
                    var m = i.Methods[0];
                    var methodP = new Paragraph();
                    methodP.Inlines.Add(MakeRun("Method: ", 16, true));
                    methodP.Inlines.Add(MakeRun($"{m.Type} ", 14, true, GetTypeColour(m, colours)));
                    methodP.Inlines.Add(MakeRun($"{m.Name} - ", 14, false));
                    methodP.Inlines.Add(MakeRun(m.Definition ?? "NO SUMMARY", 14, false, null, true));
                    methodP.Inlines.Add(MakeRun($" - {m.ReturnDefinition ?? ""}", 14, false, null, true));
                    myRichTextBox.Document.Blocks.Add(methodP);
                }
            }

            // ENUM
            if (results.Enums.Count > 0)
            {
                var e = results.Enums[0];
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Enum ", 20, true));
                p.Inlines.Add(MakeRun(e.Name, 20, true, SettingsModel.Instance?.EnumDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, e.Definition ?? "NO SUMMARY");
                myRichTextBox.Document.Blocks.Add(defP);

                if (e.EnumMembers != null && e.EnumMembers.Length > 0)
                {
                    var mem = e.EnumMembers[0];
                    var memP = new Paragraph();
                    memP.Inlines.Add(MakeRun("Member: ", 16, true));
                    memP.Inlines.Add(MakeRun($"{mem.Name} - ", 14, false));
                    memP.Inlines.Add(MakeRun(mem.Definition ?? "NO SUMMARY", 14, false, null, true));
                    myRichTextBox.Document.Blocks.Add(memP);
                }
            }

            // STRUCT
            if (results.Structs.Count > 0)
            {
                var s = results.Structs[0];
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Struct ", 20, true));
                p.Inlines.Add(MakeRun(s.Name, 20, true, SettingsModel.Instance?.StructDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, s.Definition ?? "NO SUMMARY");
                myRichTextBox.Document.Blocks.Add(defP);

                if (s.Properties != null && s.Properties.Length > 0)
                {
                    var prop = s.Properties[0];
                    var propP = new Paragraph();
                    propP.Inlines.Add(MakeRun("Property: ", 16, true));
                    propP.Inlines.Add(MakeRun($"{prop.Type} ", 14, true, GetTypeColour(prop, colours)));
                    propP.Inlines.Add(MakeRun($"{prop.Name} - ", 14, false));
                    propP.Inlines.Add(MakeRun(prop.Definition ?? "NO SUMMARY", 14, false, null, true));
                    myRichTextBox.Document.Blocks.Add(propP);
                }

                if (s.Fields != null && s.Fields.Length > 0)
                {
                    var field = s.Fields[0];
                    var fieldP = new Paragraph();
                    fieldP.Inlines.Add(MakeRun("Field: ", 16, true));
                    fieldP.Inlines.Add(MakeRun($"{field.Type} ", 14, true, GetTypeColour(field, colours)));
                    fieldP.Inlines.Add(MakeRun($"{field.Name} - ", 14, false));
                    fieldP.Inlines.Add(MakeRun(field.Definition ?? "NO SUMMARY", 14, false, null, true));
                    myRichTextBox.Document.Blocks.Add(fieldP);
                }

                if (s.Methods != null && s.Methods.Length > 0)
                {
                    var m = s.Methods[0];
                    var methodP = new Paragraph();
                    methodP.Inlines.Add(MakeRun("Method: ", 16, true));
                    methodP.Inlines.Add(MakeRun($"{m.Type} ", 14, true, GetTypeColour(m, colours)));
                    methodP.Inlines.Add(MakeRun($"{m.Name} - ", 14, false));
                    methodP.Inlines.Add(MakeRun(m.Definition ?? "NO SUMMARY", 14, false, null, true));
                    methodP.Inlines.Add(MakeRun($" - {m.ReturnDefinition ?? ""}", 14, false, null, true));
                    myRichTextBox.Document.Blocks.Add(methodP);
                }
            }
        }

        // Helper to pick colour based on ObjectType

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



        private void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            FilePopup.IsOpen = true;
        }
    }
}
