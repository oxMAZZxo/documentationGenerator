using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.View;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine($"Property with name {propertyName} has been changed");
        }

        public MainViewModel()
        {
            LoadFileCommand = new RelayCommand(LoadFile);
            SaveDocsCommand = new RelayCommand(SaveDocs);
            LoadDirectoryCommand = new RelayCommand(LoadDirectory);
            ClearDocsCommand = new RelayCommand(ClearDocs);
            OpenSettingsMenuCommand = new RelayCommand(OpenSettingsMenu);
            output = "";
            fileName = "";
            FileName = "This is where the file name you selected will be shown";
            Output = "This is where the Documentation output will be shown";
            openFileDialog = new OpenFileDialog();
            openFolderDialog = new OpenFolderDialog();

            sourceFileReader = new SourceFileReader();
            documentationWriter = new DocumentationWriter();
            settingsView = new SettingsView();
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
                
                DeclarationColours declarationColours = new DeclarationColours(SettingsModel.Instance.ClassDeclarationColour,
                    SettingsModel.Instance.EnumDeclarationColour,SettingsModel.Instance.PrimitiveDeclarationColour,
                    SettingsModel.Instance.InterfaceDeclarationColour,SettingsModel.Instance.StructDeclarationColour);
                
                documentationWriter.WriteDocumentation(openFolderDialog.FolderName, sourceFileReader.Classes.ToArray(),
                    sourceFileReader.Enums.ToArray(), sourceFileReader.Interfaces.ToArray(), sourceFileReader.Structs.ToArray(), declarationColours);
            }
        }

        private async void LoadFile()
        {
            bool? valid = openFileDialog.ShowDialog();

            if ((valid.HasValue && valid.Value == false) || valid.HasValue == false) { return; }

            await sourceFileReader.ReadSourceFilesAsync(openFileDialog.FileNames);
            
            FileName = openFileDialog.FileName;
            Output = sourceFileReader.GetAllDeclarations();
        }

        private async void LoadDirectory()
        {
            bool? valid = openFolderDialog.ShowDialog();

            if ((valid.HasValue && valid.Value == false) || valid.HasValue == false) { return; }


            await sourceFileReader.ReadSourceDirectory(openFolderDialog.FolderName);
            FileName = openFolderDialog.FolderName;

            Output = sourceFileReader.GetAllDeclarations();
        }
    }
}
