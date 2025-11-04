using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DocumentationGenerator.Helpers;
using DocumentationGenerator.Models;
using DocumentationGenerator.Views;
using PdfSharp.Snippets.Font;

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

    private void ExportToHTML()
    {
        throw new NotImplementedException();
    }

    private void ExportToPDF()
    {
        throw new NotImplementedException();
    }

    private async void LoadDirectory()
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(owner);
        if (topLevel == null) { return; }
        IReadOnlyList<IStorageFolder> folders = await topLevel.StorageProvider.OpenFolderPickerAsync(directoryPickerOpenOptions);

        await sourceFileReader.ReadSourceDirectory(folders[0], ProgLanguage.CSharp);
        FileName = folders[0].Name;
        Output = sourceFileReader.GetAllDeclarations();
    }

    private async void LoadFile()
    {
        TopLevel? toplevel = TopLevel.GetTopLevel(owner);
        if (toplevel == null) { return; }
        IReadOnlyList<IStorageFile>? files = await toplevel.StorageProvider.OpenFilePickerAsync(filePickerOpenOptions);

        await sourceFileReader.ReadSourceFilesAsync(files.ToList(), ProgLanguage.CSharp);

        FileName = files[0].Name;
        Output = sourceFileReader.GetAllDeclarations();
    }

    private void OnSettingsButtonClicked()
    {
        settingsView.ShowDialog(owner);
    }

}
