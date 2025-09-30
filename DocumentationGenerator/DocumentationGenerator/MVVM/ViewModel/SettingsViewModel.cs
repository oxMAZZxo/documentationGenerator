using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.Model.DocumentInfo;
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
        private SettingsTabs selectedTab;

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

        public SettingsTabs SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                OnPropertyChanged();
                ChangeUserControl();
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
            
        public bool AddDocumentRelationshipGraph
        {
            get => SettingsModel.Instance.AddDocumentRelationshipGraph;

            set
            {
                SettingsModel.Instance.AddDocumentRelationshipGraph = value;
                OnPropertyChanged();
            }
        }

        public bool PrintBaseTypesToDocument
        {
            get => SettingsModel.Instance.PrintBaseTypesToDocument;
            set
            {
                SettingsModel.Instance.PrintBaseTypesToDocument = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsViewModel(SettingsView settingsView)
        {
            settingsView.Closing += SettingsViewClosing;

            colourSettingsTabView = new UCColourSettingsView();
            fontSettingsTabView = new UCFontSettingsView();
            generalSettingsTabView = new UCGeneralSettingsView();

            SelectedTab = SettingsTabs.General;

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
            AddDocumentRelationshipGraph = SettingsModel.Instance.AddDocumentRelationshipGraph;
            PrintBaseTypesToDocument = SettingsModel.Instance.PrintBaseTypesToDocument;

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

        private void ChangeUserControl()
        {
            switch (SelectedTab)
            {
                case SettingsTabs.General:
                    CurrentUserControl = generalSettingsTabView;
                    break;
                case SettingsTabs.Colours:
                    CurrentUserControl = colourSettingsTabView;
                    break;
                case SettingsTabs.Fonts:
                    CurrentUserControl = fontSettingsTabView;
                    break;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum SettingsTabs
    {
        General,
        Colours,
        Fonts
    }
}
