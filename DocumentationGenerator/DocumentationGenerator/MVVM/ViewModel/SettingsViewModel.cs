using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.View;
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
                Settings.SetMigraDocClassDeclarationColour(value.R, value.G, value.B);
                Settings.ClassDeclarationColour = value;
                classDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color PrimitiveDeclarationColour
        {
            get { return primitiveDeclarationColour; }
            set
            {
                Settings.SetMigraDocPrimitiveDeclarationColour(value.R, value.G, value.B);
                Settings.PrimitiveDeclarationColour = value;
                primitiveDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color EnumDeclarationColour
        {
            get { return enumDeclarationColour; }
            set
            {
                Settings.SetMigraDocEnumDeclarationColour(value.R, value.G, value.B);
                Settings.EnumDeclarationColour = value;
                enumDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color InterfaceDeclarationColour
        {
            get { return interfaceDeclarationColour; }
            set
            {
                Settings.SetMigraDocInterfaceDeclarationColour(value.R, value.G, value.B);
                Settings.InterfaceDeclarationColour = value;
                interfaceDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color StructDeclarationColour
        {
            get { return structDeclarationColour; }
            set
            {
                Settings.SetMigraDocStructDeclarationColour(value.R, value.G, value.B);
                Settings.StructDeclarationColour = value;
                structDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsViewModel(View.SettingsView settingsView)
        {
            Settings = new SettingsModel();
            settingsView.Closing += SettingsViewClosing;
            ClassDeclarationColour = Color.FromRgb(173, 216, 230); // LightBlue
            PrimitiveDeclarationColour = Color.FromRgb(0, 0, 255); // Blue
            EnumDeclarationColour = Color.FromRgb(255, 165, 0);    // Orange
            InterfaceDeclarationColour = Color.FromRgb(0, 128, 128); // Teal
            StructDeclarationColour = Color.FromRgb(0, 255, 255); // Cyan  
        }

        private void SettingsViewClosing(object? sender, CancelEventArgs e)
        {
            if(App.Instance == null || sender == null) { return; }
            if(App.Instance.IsShuttingDown)
            {
                Debug.WriteLine($"Settings is closing");
                //Settings.Save();
                Settings.Dispose();
            }else
            {
                e.Cancel = true;
                SettingsView view = (SettingsView)sender;
                view.Hide();
                
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
