using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DocumentationGenerator.Helpers;
using DocumentationGenerator.Models;
using DocumentationGenerator.Models.DocumentInfo;
using DocumentationGenerator.Views;

namespace DocumentationGenerator.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private Window owner;
    private string fileName;
    private FilePickerOpenOptions filePickerOpenOptions;
    private FolderPickerOpenOptions directoryPickerOpenOptions;
    private FilePickerSaveOptions filePickerSaveOptions;
    private SourceFileReader sourceFileReader;
    private DocumentationWriter documentationWriter;
    private SettingsWindowView settingsView;
    private string output;

    public string FileName
    {
        get { return fileName; }
        set
        {
            fileName = value;
            OnPropertyChanged();
        }
    }

    public string Output
    {
        get => output;
        set
        {
            output = value;
            OnPropertyChanged(nameof(Output));
        }
    }

    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }

    public ICommand LoadFileCommand { get; set; }
    public ICommand LoadDirectoryCommand { get; set; }
    public ICommand ExportPDFDocumentationCommand { get; set; }
    public ICommand ExportHTMLDocumentationCommand { get; set; }
    public ICommand ClearDocsCommand { get; set; }
    public ICommand SettingsButtonCommand { get; set; }
    public ICommand ExitAppCommand { get; set; }

    public MainWindowViewModel(Window owner)
    {
        this.owner = owner;
        owner.Closing += OnAppShuttingDown;
        settingsView = new SettingsWindowView();
        LoadFileCommand = new RelayCommand(LoadFile);
        LoadDirectoryCommand = new RelayCommand(LoadDirectory);
        ExportPDFDocumentationCommand = new RelayCommand(ExportToPDF);
        ExportHTMLDocumentationCommand = new RelayCommand(ExportToHTML);
        ClearDocsCommand = new RelayCommand(ClearDocs);
        ExitAppCommand = new RelayCommand(Exit);
        SettingsButtonCommand = new RelayCommand(OnSettingsButtonClicked);

        sourceFileReader = new SourceFileReader();
        documentationWriter = new DocumentationWriter();
        InitDialogs();

        FileName = "Loaded File(s) / Directory name will be displayed here.";
        Output = "Preview will be displayed here.";
    }

    private void OnAppShuttingDown(object? sender, WindowClosingEventArgs e)
    {
        SettingsModel.Instance.SaveSettings();
    }

    private void InitDialogs()
    {
        filePickerOpenOptions = new FilePickerOpenOptions();
        filePickerOpenOptions.AllowMultiple = true;
        filePickerOpenOptions.Title = "Select a file(s) to load.";

        directoryPickerOpenOptions = new FolderPickerOpenOptions();
        directoryPickerOpenOptions.Title = "Select a folder which contains source files to load."; ;
        directoryPickerOpenOptions.AllowMultiple = false;

        filePickerSaveOptions = new FilePickerSaveOptions();
        filePickerSaveOptions.Title = "Save the Documentation as a PDF at the desired Location";
    }

    private void Exit()
    {
        Environment.Exit(0);
    }

    private void ClearDocs()
    {
        sourceFileReader.Clear();
    }

    private async void ExportToHTML()
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(owner);
        if (topLevel == null) { return; }
        IReadOnlyList<IStorageFolder> folders = await topLevel.StorageProvider.OpenFolderPickerAsync(directoryPickerOpenOptions);
        if (folders.Count < 1) { return; }
        
        DeclarationColours declarationColours = new DeclarationColours(SettingsModel.Instance.MigraDocClassDeclarationColour,
                   SettingsModel.Instance.MigraDocEnumDeclarationColour, SettingsModel.Instance.MigraDocPrimitiveDeclarationColour,
                   SettingsModel.Instance.MigraDocInterfaceDeclarationColour, SettingsModel.Instance.MigraDocStructDeclarationColour);

        DeclarationFontStyles declarationFontStyles = new DeclarationFontStyles(SettingsModel.Instance.SelectedFont, SettingsModel.Instance.ObjectDeclarationStyle,
            SettingsModel.Instance.ObjectDefinitionStyle, SettingsModel.Instance.MemberHeadingStyle, SettingsModel.Instance.MemberTypeStyle,
            SettingsModel.Instance.MemberStyle, SettingsModel.Instance.MemberDefinitionStyle);

        DocumentInformation docInfo = new DocumentInformation(folders[0], declarationColours, declarationFontStyles,
            SettingsModel.Instance.GenerateTableOfContents, SettingsModel.Instance.GeneratePageNumbers,
            SettingsModel.Instance.AddDocumentRelationshipGraph, SettingsModel.Instance.PrintBaseTypesToDocument, ProjectName, ProjectDescription);

        await documentationWriter.WriteDocumentation(DocumentationType.HTML, sourceFileReader.Classes.ToArray(),
                sourceFileReader.Enums.ToArray(), sourceFileReader.Interfaces.ToArray(), sourceFileReader.Structs.ToArray(), docInfo);
    }

    private void ExportToPDF()
    {
        throw new NotImplementedException();
    }

    private async void LoadDirectory()
    {
        if (App.Instance == null || App.Instance.TopLevel == null) { return; }
        IReadOnlyList<IStorageFolder> folders = await App.Instance.TopLevel.StorageProvider.OpenFolderPickerAsync(directoryPickerOpenOptions);

        await sourceFileReader.ReadSourceDirectory(folders[0], ProgLanguage.CSharp);
        FileName = folders[0].Name;
        Output = sourceFileReader.GetAllDeclarations();
    }

    private async void LoadFile()
    {
        
        if (App.Instance == null || App.Instance.TopLevel == null) { return; }
        IReadOnlyList<IStorageFile>? files = await App.Instance.TopLevel.StorageProvider.OpenFilePickerAsync(filePickerOpenOptions);

        await sourceFileReader.ReadSourceFilesAsync(files.ToList(), ProgLanguage.CSharp);

        FileName = files[0].Name;
        Output = sourceFileReader.GetAllDeclarations();
    }

    private void OnSettingsButtonClicked()
    {
        settingsView.ShowDialog(owner);
    }

}
