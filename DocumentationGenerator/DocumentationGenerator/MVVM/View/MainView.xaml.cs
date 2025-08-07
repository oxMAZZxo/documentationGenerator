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
            // Example: Procedurally add formatted text to a WPF RichTextBox

            // Clear the RichTextBox first if needed
            myRichTextBox.Document.Blocks.Clear();

            // Create a paragraph to hold our runs
            Paragraph paragraph = new Paragraph();

            // Example list of strings
            var words = new List<string> { "OK", "Warning", "Error" };

            // Loop through words and format each one differently
            foreach (var word in words)
            {
                Run run = new Run(word + " "); // add space for separation

                // Change color based on value
                switch (word)
                {
                    case "OK":
                        run.Foreground = Brushes.Green;
                        break;
                    case "Warning":
                        run.Foreground = Brushes.Orange;
                        run.FontWeight = FontWeights.Bold;
                        break;
                    case "Error":
                        run.Foreground = Brushes.Red;
                        run.TextDecorations = TextDecorations.Underline;
                        break;
                }

                // Add this run to the paragraph
                paragraph.Inlines.Add(run);
            }

            // Add the paragraph to the RichTextBox's document
            myRichTextBox.Document.Blocks.Add(paragraph);

        }

        private void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            FilePopup.IsOpen = true;
        }

        
    }
}
