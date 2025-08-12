using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.View;
using DocumentationGenerator.MVVM.View.SettingsTabViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private UserControl currentUserControl;
        private UserControl colourSettingsTabView;
        private UserControl fontSettingsTabView;
        private UserControl generalSettingsTabView;

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

        public UserControl CurrentUserControl
        {
            get
            {
                return currentUserControl;
            }
            set
            {
                currentUserControl = value;
                OnPropertyChanged();
            }
        }

        public bool IsColourButtonChecked { get; set; }
        public bool IsFontButtonChecked { get; set; }
        public bool IsGeneralButtonChecked { get; set; }

        public ICommand ShowColourSettingsCommand { get; set; }
        public ICommand ShowFontSettingsCommand { get; set; }
        public ICommand ShowGeneralSettingsCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsViewModel(SettingsView settingsView)
        {
            Settings = new SettingsModel();
            settingsView.Closing += SettingsViewClosing;

            colourSettingsTabView = new UCColourSettingsView();
            fontSettingsTabView = new UCFontSettingsView();
            generalSettingsTabView = new UCGeneralSettingsView();

            currentUserControl = generalSettingsTabView;
            CurrentUserControl = generalSettingsTabView;
            IsGeneralButtonChecked = true;
            OnPropertyChanged("IsGeneralButtonChecked");

            ShowColourSettingsCommand = new RelayCommand(ShowColourSettings);
            ShowFontSettingsCommand = new RelayCommand(ShowFontSettings);
            ShowGeneralSettingsCommand = new RelayCommand(ShowGeneralSettings);

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

        private void ShowColourSettings()
        {
            CurrentUserControl = colourSettingsTabView;
            IsGeneralButtonChecked = false;
            IsFontButtonChecked = false;
            OnPropertyChanged("IsFontButtonChecked");
            OnPropertyChanged("IsGeneralButtonChecked");
        }

        private void ShowFontSettings()
        {
            CurrentUserControl = fontSettingsTabView;
            IsGeneralButtonChecked = false;
            IsColourButtonChecked = false;
            OnPropertyChanged("IsColourButtonChecked");
            OnPropertyChanged("IsGeneralButtonChecked");
        }

        private void ShowGeneralSettings()
        {
            CurrentUserControl = generalSettingsTabView;
            IsFontButtonChecked = false;
            IsColourButtonChecked = false;
            OnPropertyChanged("IsColourButtonChecked");
            OnPropertyChanged("IsFontButtonChecked");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
