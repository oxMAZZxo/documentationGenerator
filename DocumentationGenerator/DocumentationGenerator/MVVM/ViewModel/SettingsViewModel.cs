using DocumentationGenerator.MVVM.Helpers;
using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.View;
using DocumentationGenerator.MVVM.View.SettingsTabViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        private FontDeclarationStyle objectDeclarationStyle;
        private FontDeclarationStyle objectDefinitionStyle;
        private FontDeclarationStyle memberHeadingStyle;
        private FontDeclarationStyle memberStyle;
        private FontDeclarationStyle memberTypeStyle;
        private FontDeclarationStyle memberDefinitionStyle;
        private string selectedFont;

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

        public ObservableCollection<string> Fonts { get; } = new ObservableCollection<string> { "Arial", "Arial Black", "Courier New" };

        public string SelectedFont
        {
            get => selectedFont;
            set
            {
                selectedFont = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle ObjectDeclarationStyle
        {
            get => objectDeclarationStyle;
            set
            {
                objectDeclarationStyle = value;
                Debug.WriteLine("Object Declaration Style Changed");
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle ObjectDefinitionStyle
        {
            get => objectDefinitionStyle;
            set
            {
                objectDefinitionStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberHeadingStyle
        {
            get => memberHeadingStyle;
            set
            {
                memberHeadingStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberStyle
        {
            get => memberStyle;
            set
            {
                memberStyle = value;
                OnPropertyChanged();
            }
        } 
        
        public FontDeclarationStyle MemberTypeStyle
        {
            get => memberTypeStyle;
            set
            {
                memberTypeStyle = value;
                OnPropertyChanged();
            }
        }

        public FontDeclarationStyle MemberDefinitionStyle
        {
            get => memberDefinitionStyle;
            set
            {
                memberDefinitionStyle = value;
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

            objectDeclarationStyle = new FontDeclarationStyle("20",false,true,"5");
            ObjectDeclarationStyle = objectDeclarationStyle;

            objectDefinitionStyle = new FontDeclarationStyle("18",true,false,"20");
            ObjectDefinitionStyle = objectDefinitionStyle;

            memberHeadingStyle = new FontDeclarationStyle("16",false,true,"2");
            MemberHeadingStyle = memberHeadingStyle;

            memberStyle = new FontDeclarationStyle("14",false,false,"1");
            MemberStyle = memberStyle;

            memberTypeStyle = new FontDeclarationStyle("14", false, true, "1");
            MemberTypeStyle = memberTypeStyle;

            memberDefinitionStyle = new FontDeclarationStyle("14", true, false, "1");
            MemberDefinitionStyle = memberDefinitionStyle;

            selectedFont = Fonts[0];
            SelectedFont = selectedFont;
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
