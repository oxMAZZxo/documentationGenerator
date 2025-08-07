using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace DocumentationGenerator.MVVM.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsModel Settings { get; private set; }
        private Color classDeclarationColour;
        private Color primitiveDeclarationColour;
        private Color enumDeclarationColour;
        private Color interfaceDeclarationColour;
        private Color structDeclarationColour;

        public Color ClassDeclarationColour
        {
            get { return classDeclarationColour; }
            set
            {
                Settings.SetClassDeclarationColour(value.R, value.G, value.B);
                classDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color PrimitiveDeclarationColour
        {
            get { return primitiveDeclarationColour; }
            set
            {
                Settings.SetPrimitiveDeclarationColour(value.R, value.G, value.B);
                primitiveDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color EnumDeclarationColour
        {
            get { return enumDeclarationColour; }
            set
            {
                Settings.SetEnumDeclarationColour(value.R, value.G, value.B);
                enumDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color InterfaceDeclarationColour
        {
            get { return interfaceDeclarationColour; }
            set
            {
                Settings.SetInterfaceDeclarationColour(value.R, value.G, value.B);
                interfaceDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color StructDeclarationColour
        {
            get { return structDeclarationColour; }
            set
            {
                Settings.SetStructDeclarationColour(value.R, value.G, value.B);
                structDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            Settings = new SettingsModel();

            ClassDeclarationColour = Color.FromRgb(173, 216, 230); // LightBlue
            PrimitiveDeclarationColour = Color.FromRgb(0, 0, 255); // Blue
            EnumDeclarationColour = Color.FromRgb(255, 165, 0);    // Orange
            InterfaceDeclarationColour = Color.FromRgb(0, 128, 128); // Teal
            StructDeclarationColour = Color.FromRgb(0, 255, 255); // Cyan  
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine($"Property with name {propertyName} has been changed");
        }
    }
}
