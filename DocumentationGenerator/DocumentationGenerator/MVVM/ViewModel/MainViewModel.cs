using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;

using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        private Color classDeclarationColour;
        private Color primitiveDeclarationColour;
        private Color enumDeclarationColour;
        private Color interfaceDeclarationColour;

        public Color ClassDeclarationColour
        {
            get { return classDeclarationColour; }
            set
            {
                classDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color PrimitiveDeclarationColour
        {
            get { return primitiveDeclarationColour; }
            set
            {
                primitiveDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color EnumDeclarationColour
        {
            get { return enumDeclarationColour; }
            set
            {
                enumDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color InterfaceDeclarationColour
        {
            get { return interfaceDeclarationColour; }
            set
            {
                interfaceDeclarationColour = value;
                OnPropertyChanged();
            }
        }


        public string Output
        {
            get { return output; }
            set
            {
                output = value;
                OnPropertyChanged();
            }
        }

        public string FileName { get { return fileName; } set { fileName = value; OnPropertyChanged(); } }

        public ICommand LoadFileCommand { get; set; }
        public ICommand LoadDirectoryCommand { get; set; }
        public ICommand SaveDocsCommand { get; set; }
        public ICommand ClearDocsCommand { get; set; }

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
            output = "";
            fileName = "";
            FileName = "This is where the file name you selected will be shown";
            Output = "This is where the Documentation output will be shown";
            openFileDialog = new OpenFileDialog();
            openFolderDialog = new OpenFolderDialog();

            sourceFileReader = new SourceFileReader();
            documentationWriter = new DocumentationWriter();

            ClassDeclarationColour = Color.FromRgb(173, 216, 230); // LightBlue
            PrimitiveDeclarationColour = Color.FromRgb(0, 0, 255); // Blue
            EnumDeclarationColour = Color.FromRgb(255, 165, 0);    // Orange
            InterfaceDeclarationColour = Color.FromRgb(0, 128, 128); // Teal
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
                DeclarationColours declarationColours = new DeclarationColours(new MigraDoc.DocumentObjectModel.Color(classDeclarationColour.R,classDeclarationColour.G,classDeclarationColour.B), new MigraDoc.DocumentObjectModel.Color(enumDeclarationColour.R, enumDeclarationColour.G, enumDeclarationColour.B), new MigraDoc.DocumentObjectModel.Color(primitiveDeclarationColour.R, primitiveDeclarationColour.G, primitiveDeclarationColour.B), new MigraDoc.DocumentObjectModel.Color(interfaceDeclarationColour.R, interfaceDeclarationColour.G, interfaceDeclarationColour.B));
                
                documentationWriter.WriteDocumentation(openFolderDialog.FolderName, sourceFileReader.Classes.ToArray(), sourceFileReader.Enums.ToArray(),declarationColours);
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
