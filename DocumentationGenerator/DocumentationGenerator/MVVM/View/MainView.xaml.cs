using DocumentationGenerator.MVVM.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

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
            //myRichTextBox.SelectionChanged += MyRichTextBox_SelectionChanged;
        }

        private void MyRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine(myRichTextBox.Selection.Text.ToString());
            //TextRange selection = new TextRange(myRichTextBox.Selection.Start, myRichTextBox.Selection.End);
            
            //if(myColorPicker.SelectedColor.HasValue == false) { return; }
            
            //selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(myColorPicker.SelectedColor.Value)); // or any brush

            ColorPicker c = new ColorPicker();
        }

        private void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            FilePopup.IsOpen = true;
        }

        
    }
}
