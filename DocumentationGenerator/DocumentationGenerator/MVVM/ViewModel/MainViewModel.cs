using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.View;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DocumentationGenerator.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string output;
        private string fileName;
        private OpenFileDialog openFileDialog;
        private OpenFolderDialog openFolderDialog;
        private SourceFileReader sourceFileReader;
        private DocumentationWriter documentationWriter;
        private SettingsView settingsView;
        public string Output
        {
            get { return output; }
            set
            {
                output = value;
                OnPropertyChanged();
            }
        }

        public string FileName 
        { 
            get { return fileName; } 
            set 
            { 
                fileName = value; 
                OnPropertyChanged(); 
            } 
        }

        public ICommand LoadFileCommand { get; set; }
        public ICommand LoadDirectoryCommand { get; set; }
        public ICommand SaveDocsCommand { get; set; }
        public ICommand ClearDocsCommand { get; set; }
        public ICommand OpenSettingsMenuCommand { get; set; }
        public ICommand RefreshPreviewCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel(MainView mainView)
        {
            mainView.Closed += MainView_Closed;
            LoadFileCommand = new RelayCommand(LoadFile);
            SaveDocsCommand = new RelayCommand(SaveDocs);
            LoadDirectoryCommand = new RelayCommand(LoadDirectory);
            ClearDocsCommand = new RelayCommand(ClearDocs);
            OpenSettingsMenuCommand = new RelayCommand(OpenSettingsMenu);
            RefreshPreviewCommand = new RelayCommand(RefreshPreview);

            fileName = "";
            FileName = "This is where the file/s or directory loaded will be displayed.";
            output = "";
            Output = "An output preview will be shown here.";

            openFileDialog = new OpenFileDialog();
            openFolderDialog = new OpenFolderDialog();

            sourceFileReader = new SourceFileReader();
            documentationWriter = new DocumentationWriter();
            settingsView = new SettingsView();
            
        }

        private void RefreshPreview()
        {
            OnPropertyChanged("FileName");
        }

        private void MainView_Closed(object? sender, EventArgs e)
        {
            Debug.WriteLine($"Main View Closing");
            settingsView.Close();
        }

        private void OpenSettingsMenu()
        {
            settingsView.Show();
        }

        private void ClearDocs()
        {
            sourceFileReader.Clear();
        }

        private void SaveDocs()
        {
            bool? valid = openFolderDialog.ShowDialog();

            if (valid.HasValue && valid.Value == true)
            {
                if(SettingsModel.Instance == null) { MessageBox.Show($"Could not save document as the settings could not be retrieved. Settings Instance is null."); return; }
                
                DeclarationColours declarationColours = new DeclarationColours(SettingsModel.Instance.MigraDocClassDeclarationColour,
                    SettingsModel.Instance.MigraDocEnumDeclarationColour,SettingsModel.Instance.MigraDocPrimitiveDeclarationColour,
                    SettingsModel.Instance.MigraDocInterfaceDeclarationColour,SettingsModel.Instance.MigraDocStructDeclarationColour);
                
                documentationWriter.WriteDocumentation(openFolderDialog.FolderName, sourceFileReader.Classes.ToArray(),
                    sourceFileReader.Enums.ToArray(), sourceFileReader.Interfaces.ToArray(), sourceFileReader.Structs.ToArray(), declarationColours);
            }
        }

        private async void LoadFile()
        {
            bool? valid = openFileDialog.ShowDialog();

            if ((valid.HasValue && valid.Value == false) || valid.HasValue == false) { return; }

            await sourceFileReader.ReadSourceFilesAsync(openFileDialog.FileNames);
            
            FileName = $"File Name: {openFileDialog.SafeFileName}";
            Output = sourceFileReader.GetAllDeclarations();
        }

        private async void LoadDirectory()
        {
            bool? valid = openFolderDialog.ShowDialog();

            if ((valid.HasValue && valid.Value == false) || valid.HasValue == false) { return; }


            await sourceFileReader.ReadSourceDirectory(openFolderDialog.FolderName);
            FileName = $"Directory: {openFolderDialog.SafeFolderName}";
            Output = sourceFileReader.GetAllDeclarations();

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
