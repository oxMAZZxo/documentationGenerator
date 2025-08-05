using DocumentationGenerator.MVVM.ViewModel;
using System.Diagnostics;
using System.Windows;

namespace DocumentationGenerator.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            FilePopup.IsOpen = true;
        }
    }
}
