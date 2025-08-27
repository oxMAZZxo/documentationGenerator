using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.View;
using DocumentationGenerator.MVVM.View.SettingsTabViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DocumentationGenerator.MVVM.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsModel Settings { get; private set; }
        private UserControl currentUserControl;
        private UserControl colourSettingsTabView;
        private UserControl fontSettingsTabView;
        private UserControl generalSettingsTabView;


        public Color ClassDeclarationColour
        {
            get { return Settings.ClassDeclarationColour; }
            set
            {
                Settings.SetMigraDocClassDeclarationColour(value.R, value.G, value.B);
                Settings.ClassDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color PrimitiveDeclarationColour
        {
            get { return Settings.PrimitiveDeclarationColour; }
            set
            {
                Settings.SetMigraDocPrimitiveDeclarationColour(value.R, value.G, value.B);
                Settings.PrimitiveDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color EnumDeclarationColour
        {
            get { return Settings.EnumDeclarationColour; }
            set
            {
                Settings.SetMigraDocEnumDeclarationColour(value.R, value.G, value.B);
                Settings.EnumDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color InterfaceDeclarationColour
        {
            get { return Settings.InterfaceDeclarationColour; }
            set
            {
                Settings.SetMigraDocInterfaceDeclarationColour(value.R, value.G, value.B);
                Settings.InterfaceDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color StructDeclarationColour
        {
            get { return Settings.StructDeclarationColour; }
            set
            {
                Settings.SetMigraDocStructDeclarationColour(value.R, value.G, value.B);
                Settings.StructDeclarationColour = value;
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

        public ObservableCollection<string> Fonts { get; } = new ObservableCollection<string> { "Arial", "Arial Black", "Courier New" };

        public string SelectedFont
        {
            get => Settings.SelectedFont;
            set
            {
                Settings.SelectedFont = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle ObjectDeclarationStyle
        {
            get => Settings.ObjectDeclarationStyle;
            set
            {
                Settings.ObjectDeclarationStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle ObjectDefinitionStyle
        {
            get => Settings.ObjectDefinitionStyle;
            set
            {
                Settings.ObjectDefinitionStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberHeadingStyle
        {
            get => Settings.MemberHeadingStyle;
            set
            {
                Settings.MemberHeadingStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberStyle
        {
            get => Settings.MemberStyle;
            set
            {
                Settings.MemberStyle = value;
                OnPropertyChanged();
            }
        } 
        
        public FontDeclarationStyle MemberTypeStyle
        {
            get => Settings.MemberTypeStyle;
            set
            {
                Settings.MemberTypeStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberDefinitionStyle
        {
            get => Settings.MemberDefinitionStyle;
            set
            {
                Settings.MemberDefinitionStyle = value;
                OnPropertyChanged();
            }
        }

        public bool GenerateTableOfContents
        {
            get => Settings.GenerateTableOfContents;
            set
            {
                Settings.GenerateTableOfContents = value;
                OnPropertyChanged();
            }
        }

        public bool GeneratePageNumbers
        {
            get => Settings.GeneratePageNumbers;
            set
            {
                Settings.GeneratePageNumbers = value;
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

            ClassDeclarationColour = Settings.ClassDeclarationColour;
            PrimitiveDeclarationColour = Settings.PrimitiveDeclarationColour;
            EnumDeclarationColour = Settings.EnumDeclarationColour;
            InterfaceDeclarationColour = Settings.InterfaceDeclarationColour;
            StructDeclarationColour = Settings.StructDeclarationColour;

            ObjectDeclarationStyle = Settings.ObjectDeclarationStyle;

            ObjectDefinitionStyle = Settings.ObjectDefinitionStyle;

            MemberHeadingStyle = Settings.MemberHeadingStyle;

            MemberStyle = Settings.MemberStyle;

            MemberTypeStyle = Settings.MemberTypeStyle;

            MemberDefinitionStyle = Settings.MemberDefinitionStyle;

            SelectedFont = Settings.SelectedFont;

            GeneratePageNumbers = Settings.GeneratePageNumbers;
            GenerateTableOfContents = Settings.GenerateTableOfContents;

            ObjectDeclarationStyle.PropertyChanged += DeclarationStylePropertyChanged;
            ObjectDefinitionStyle.PropertyChanged += DeclarationStylePropertyChanged;
            MemberHeadingStyle.PropertyChanged += DeclarationStylePropertyChanged;
            MemberStyle.PropertyChanged += DeclarationStylePropertyChanged;
            MemberTypeStyle.PropertyChanged += DeclarationStylePropertyChanged;
            MemberDefinitionStyle.PropertyChanged += DeclarationStylePropertyChanged;

        }

        private void DeclarationStylePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        private void SettingsViewClosing(object? sender, CancelEventArgs e)
        {
            if(App.Instance == null || sender == null) { return; }
            if(!App.Instance.IsShuttingDown)
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
