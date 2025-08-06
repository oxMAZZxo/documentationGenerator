using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.IO;

namespace DocumentationGenerator.MVVM.Helpers
{
    public static class RichTextBoxHelper
    {
        public static readonly DependencyProperty BoundDocumentProperty =
            DependencyProperty.RegisterAttached(
                "BoundDocument",
                typeof(string),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundDocumentChanged));

        public static string GetBoundDocument(DependencyObject obj) => (string)obj.GetValue(BoundDocumentProperty);

        public static void SetBoundDocument(DependencyObject obj, string value) => obj.SetValue(BoundDocumentProperty, value);

        private static void OnBoundDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox richTextBox)
            {
                string newText = e.NewValue as string ?? "";

                // Prevent recursive updates
                richTextBox.TextChanged -= RichTextBox_TextChanged;

                // Load string into RichTextBox
                richTextBox.Document.Blocks.Clear();
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(newText)))
                {
                    var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                    range.Load(stream, DataFormats.Rtf);
                }

                richTextBox.TextChanged += RichTextBox_TextChanged;
            }
        }

        private static void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

                using (var stream = new MemoryStream())
                {
                    range.Save(stream, DataFormats.Rtf);
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream))
                    {
                        SetBoundDocument(richTextBox, reader.ReadToEnd());
                    }
                }
            }
        }
    }

}
