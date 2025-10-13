using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.Model.DocumentInfo;
using DocumentationGenerator.MVVM.View;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DocumentationGenerator.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string fileName;
        private OpenFileDialog openFileDialog;
        private OpenFolderDialog openFolderDialog;
        private SaveFileDialog saveFileDialog;
        private SourceFileReader sourceFileReader;
        private DocumentationWriter documentationWriter;
        private SettingsView settingsView;
        private MainView view;

        public string FileName 
        { 
            get { return fileName; } 
            set 
            { 
                fileName = value; 
                OnPropertyChanged(); 
            } 
        }

        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public ICommand LoadFileCommand { get; set; }
        public ICommand LoadDirectoryCommand { get; set; }
        public ICommand ExportPDFDocumentationCommand { get; set; }
        public ICommand ExportHTMLDocumentationCommand { get; set; }
        public ICommand ClearDocsCommand { get; set; }
        public ICommand OpenSettingsMenuCommand { get; set; }
        public ICommand ExitAppCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if(sourceFileReader != null && sourceFileReader.HasData)
            {
                ParsedSourceResults parsedSourceResults = new ParsedSourceResults
                {
                    Classes = sourceFileReader.Classes,
                    Enums = sourceFileReader.Enums,
                    Structs = sourceFileReader.Structs,
                    Interfaces = sourceFileReader.Interfaces
                };

                view.UpdateRichTextBox(parsedSourceResults);
            }
        }

        public MainViewModel(MainView mainView)
        {
            mainView.Closed += MainView_Closed;
            view = mainView;
            
            LoadFileCommand = new RelayCommand(LoadFile);
            ExportPDFDocumentationCommand = new RelayCommand(ExportPDFDocumentation);
            ExportHTMLDocumentationCommand = new RelayCommand(ExportHTMLDocumentation);
            LoadDirectoryCommand = new RelayCommand(LoadDirectory);
            ClearDocsCommand = new RelayCommand(ClearDocs);
            OpenSettingsMenuCommand = new RelayCommand(OpenSettingsMenu);
            ExitAppCommand = new RelayCommand(ExitApp);

            fileName = "";
            FileName = "The name of the file/directory loaded will be displayed here.";

            InitDialogs();
            sourceFileReader = new SourceFileReader();
            documentationWriter = new DocumentationWriter();

            settingsView = new SettingsView();

            SettingsViewModel settingsViewModel = (SettingsViewModel)settingsView.DataContext;
            settingsViewModel.PropertyChanged += SettingsViewModelPropertyChanged;

            view.ShowDefaultPreviewMessage();
        }



        private void ExitApp()
        {
            view.Close();
        }

        private void InitDialogs()
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a file(s) to load.";
            openFileDialog.Filter = "C# Files (*.cs)|*.cs|Visual Basic Files (*.vb)|*.vb|C++ Files (*.cpp)|*.cpp";
            openFileDialog.DefaultExt = ".cs";
            openFileDialog.Multiselect = true;

            openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Title = "Select a folder which contains source files to load.";
            openFolderDialog.Multiselect = false;
            
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save the Documentation as a PDF at the desired Location";
            saveFileDialog.DefaultExt = ".pdf";
            saveFileDialog.Filter = "PDF (*.pdf)|*.pdf";
            saveFileDialog.AddExtension = true;
        }

        private void SettingsViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedFont") { view.ChangeRichTextBoxFont(); return; }

            if (sourceFileReader.HasData)
            {
                view.UpdateRichTextBox(GetAllSourceResults());
            }
            else
            {
                view.ShowDefaultPreviewMessage();
            }
        }

        private void MainView_Closed(object? sender, EventArgs e)
        {
            settingsView.Close();
        }

        private void OpenSettingsMenu()
        {
            settingsView.ShowDialog();
        }

        private void ClearDocs()
        {
            if(sourceFileReader == null || !sourceFileReader.HasData) { return; }
            
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear all the loaded source files?","Warning!",MessageBoxButton.YesNo);
            if(result == MessageBoxResult.No) { return; }

            sourceFileReader.Clear();

            view.ShowDefaultPreviewMessage();
            FileName = "The name of the file/directory loaded will be displayed here.";
        }
        
        private void ExportHTMLDocumentation()
        {
            if (!sourceFileReader.HasData)
            {
                MessageBox.Show("You cannot export to PDF, since no source data has been loaded");
                return;
            }

            bool? valid = openFolderDialog.ShowDialog();
            
            if((valid.HasValue && valid.Value == false) || !valid.HasValue) { return; }

            if (SettingsModel.Instance == null)
            {
                MessageBox.Show($"Could not save document as the settings could not be retrieved. Settings Instance is null.");
                return;
            }

            DeclarationColours declarationColours = new DeclarationColours(SettingsModel.Instance.MigraDocClassDeclarationColour,
                    SettingsModel.Instance.MigraDocEnumDeclarationColour, SettingsModel.Instance.MigraDocPrimitiveDeclarationColour,
                    SettingsModel.Instance.MigraDocInterfaceDeclarationColour, SettingsModel.Instance.MigraDocStructDeclarationColour);

                DeclarationFontStyles declarationFontStyles = new DeclarationFontStyles(SettingsModel.Instance.SelectedFont, SettingsModel.Instance.ObjectDeclarationStyle,
                    SettingsModel.Instance.ObjectDefinitionStyle, SettingsModel.Instance.MemberHeadingStyle, SettingsModel.Instance.MemberTypeStyle,
                    SettingsModel.Instance.MemberStyle, SettingsModel.Instance.MemberDefinitionStyle);

                DocumentInformation docInfo = new DocumentInformation(openFolderDialog.FolderName, declarationColours, declarationFontStyles,
                    SettingsModel.Instance.GenerateTableOfContents, SettingsModel.Instance.GeneratePageNumbers,
                    SettingsModel.Instance.AddDocumentRelationshipGraph, SettingsModel.Instance.PrintBaseTypesToDocument, ProjectName, ProjectDescription);

            documentationWriter.WriteDocumentation(DocumentationType.HTML, sourceFileReader.Classes.ToArray(),
                    sourceFileReader.Enums.ToArray(), sourceFileReader.Interfaces.ToArray(), sourceFileReader.Structs.ToArray(), docInfo);

            Process.Start("explorer.exe", openFolderDialog.FolderName);
        }

        private void ExportPDFDocumentation()
        {
            if (!sourceFileReader.HasData)
            {
                MessageBox.Show("You cannot export to PDF, since no source data has been loaded");
                return;
            }

            bool? valid = saveFileDialog.ShowDialog();

            if ((valid.HasValue && valid.Value == false) || !valid.HasValue) { return; }

            if (SettingsModel.Instance == null) 
            { 
                MessageBox.Show($"Could not save document as the settings could not be retrieved. Settings Instance is null."); 
                return; 
            }

                DeclarationColours declarationColours = new DeclarationColours(SettingsModel.Instance.MigraDocClassDeclarationColour,
                    SettingsModel.Instance.MigraDocEnumDeclarationColour, SettingsModel.Instance.MigraDocPrimitiveDeclarationColour,
                    SettingsModel.Instance.MigraDocInterfaceDeclarationColour, SettingsModel.Instance.MigraDocStructDeclarationColour);

                DeclarationFontStyles declarationFontStyles = new DeclarationFontStyles(SettingsModel.Instance.SelectedFont, SettingsModel.Instance.ObjectDeclarationStyle,
                    SettingsModel.Instance.ObjectDefinitionStyle, SettingsModel.Instance.MemberHeadingStyle, SettingsModel.Instance.MemberTypeStyle,
                    SettingsModel.Instance.MemberStyle, SettingsModel.Instance.MemberDefinitionStyle);

                DocumentInformation documentInfo = new DocumentInformation(saveFileDialog.FileName, declarationColours, declarationFontStyles,
                    SettingsModel.Instance.GenerateTableOfContents, SettingsModel.Instance.GeneratePageNumbers,
                    SettingsModel.Instance.AddDocumentRelationshipGraph, SettingsModel.Instance.PrintBaseTypesToDocument,ProjectName,ProjectDescription);

                documentationWriter.WriteDocumentation(DocumentationType.PDF, sourceFileReader.Classes.ToArray(),
                    sourceFileReader.Enums.ToArray(), sourceFileReader.Interfaces.ToArray(), sourceFileReader.Structs.ToArray(), documentInfo);

                Process.Start("explorer.exe", openFolderDialog.FolderName);

            
        }

        private void LoadFile()
        {
            if (!CheckForExistingData()) { return; }

            bool? valid = openFileDialog.ShowDialog();

            if ((valid.HasValue && valid.Value == false) || valid.HasValue == false) { return; }

            FileName = "Reading File, PLEASE WAIT...";

            LoadFileAsync();
        }

        private async void LoadFileAsync()
        {
            ProgLanguage progLanguage = ProgLanguage.CSharp;

            //switch (openFileDialog.FilterIndex)
            //{
            //    //case 2: progLanguage = ProgLanguage.VisualBasic; break;
            //    //case 3: progLanguage = ProgLanguage.CPP; break;
            //}

            await sourceFileReader.ReadSourceFilesAsync(openFileDialog.FileNames,progLanguage);

            FileName = $"File Name: {openFileDialog.SafeFileName}";
        }

        private void LoadDirectory()
        {
            if(!CheckForExistingData()) { return; }

            bool? valid = openFolderDialog.ShowDialog();

            if ((valid.HasValue && valid.Value == false) || valid.HasValue == false) { return; }

            FileName = "Reading Directory, PLEASE WAIT...";

            LoadDirectoryAsync();

        }

        private bool CheckForExistingData()
        {
            if (sourceFileReader.HasData)
            {
                MessageBoxResult result = MessageBox.Show($"You already has source data loaded. Would you like to clear the existing data? \nClick Yes to add data, \nClick No to clear data, \nClick Cancel to not perform any operation.", "Caption", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.No: ClearDocs(); break;
                    case MessageBoxResult.Cancel: return false;
                }
            }
            return true;
        }

        private async void LoadDirectoryAsync()
        {
            await sourceFileReader.ReadSourceDirectory(openFolderDialog.FolderName,ProgLanguage.CSharp);

            FileName = $"Directory: {openFolderDialog.SafeFolderName}";

        }

        public ParsedSourceResults GetAllSourceResults()
        {
            ParsedSourceResults result = new ParsedSourceResults();

            result.Enums = sourceFileReader.Enums;
            result.Interfaces = sourceFileReader.Interfaces;
            result.Structs = sourceFileReader.Structs;
            result.Classes = sourceFileReader.Classes;

            return result;
        }
    }
}
