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
        private UserControl currentUserControl;
        private UserControl colourSettingsTabView;
        private UserControl fontSettingsTabView;
        private UserControl generalSettingsTabView;


        public Color ClassDeclarationColour
        {
            get { return SettingsModel.Instance.ClassDeclarationColour; }
            set
            {
                SettingsModel.Instance.SetMigraDocClassDeclarationColour(value.R, value.G, value.B);
                SettingsModel.Instance.ClassDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color PrimitiveDeclarationColour
        {
            get { return SettingsModel.Instance.PrimitiveDeclarationColour; }
            set
            {
                SettingsModel.Instance.SetMigraDocPrimitiveDeclarationColour(value.R, value.G, value.B);
                SettingsModel.Instance.PrimitiveDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color EnumDeclarationColour
        {
            get { return SettingsModel.Instance.EnumDeclarationColour; }
            set
            {
                SettingsModel.Instance.SetMigraDocEnumDeclarationColour(value.R, value.G, value.B);
                SettingsModel.Instance.EnumDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color InterfaceDeclarationColour
        {
            get { return SettingsModel.Instance.InterfaceDeclarationColour; }
            set
            {
                SettingsModel.Instance.SetMigraDocInterfaceDeclarationColour(value.R, value.G, value.B);
                SettingsModel.Instance.InterfaceDeclarationColour = value;
                OnPropertyChanged();
            }
        }

        public Color StructDeclarationColour
        {
            get { return SettingsModel.Instance.StructDeclarationColour; }
            set
            {
                SettingsModel.Instance.SetMigraDocStructDeclarationColour(value.R, value.G, value.B);
                SettingsModel.Instance.StructDeclarationColour = value;
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

        public ObservableCollection<string> Fonts { get => SettingsModel.Instance.Fonts; }

        public string SelectedFont
        {
            get => SettingsModel.Instance.SelectedFont;
            set
            {
                SettingsModel.Instance.SelectedFont = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle ObjectDeclarationStyle
        {
            get => SettingsModel.Instance.ObjectDeclarationStyle;
            set
            {
                SettingsModel.Instance.ObjectDeclarationStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle ObjectDefinitionStyle
        {
            get => SettingsModel.Instance.ObjectDefinitionStyle;
            set
            {
                SettingsModel.Instance.ObjectDefinitionStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberHeadingStyle
        {
            get => SettingsModel.Instance.MemberHeadingStyle;
            set
            {
                SettingsModel.Instance.MemberHeadingStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberStyle
        {
            get => SettingsModel.Instance.MemberStyle;
            set
            {
                SettingsModel.Instance.MemberStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberTypeStyle
        {
            get => SettingsModel.Instance.MemberTypeStyle;
            set
            {
                SettingsModel.Instance.MemberTypeStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberDefinitionStyle
        {
            get => SettingsModel.Instance.MemberDefinitionStyle;
            set
            {
                SettingsModel.Instance.MemberDefinitionStyle = value;
                OnPropertyChanged();
            }
        }

        public bool GenerateTableOfContents
        {
            get => SettingsModel.Instance.GenerateTableOfContents;
            set
            {
                SettingsModel.Instance.GenerateTableOfContents = value;
                OnPropertyChanged();
            }
        }

        public bool GeneratePageNumbers
        {
            get => SettingsModel.Instance.GeneratePageNumbers;
            set
            {
                SettingsModel.Instance.GeneratePageNumbers = value;
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

            ClassDeclarationColour = SettingsModel.Instance.ClassDeclarationColour;
            PrimitiveDeclarationColour = SettingsModel.Instance.PrimitiveDeclarationColour;
            EnumDeclarationColour = SettingsModel.Instance.EnumDeclarationColour;
            InterfaceDeclarationColour = SettingsModel.Instance.InterfaceDeclarationColour;
            StructDeclarationColour = SettingsModel.Instance.StructDeclarationColour;

            ObjectDeclarationStyle = SettingsModel.Instance.ObjectDeclarationStyle;

            ObjectDefinitionStyle = SettingsModel.Instance.ObjectDefinitionStyle;

            MemberHeadingStyle = SettingsModel.Instance.MemberHeadingStyle;

            MemberStyle = SettingsModel.Instance.MemberStyle;

            MemberTypeStyle = SettingsModel.Instance.MemberTypeStyle;

            MemberDefinitionStyle = SettingsModel.Instance.MemberDefinitionStyle;

            SelectedFont = SettingsModel.Instance.SelectedFont;

            GeneratePageNumbers = SettingsModel.Instance.GeneratePageNumbers;
            GenerateTableOfContents = SettingsModel.Instance.GenerateTableOfContents;

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
            if (App.Instance == null || sender == null) { return; }
            if (!App.Instance.IsShuttingDown)
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
