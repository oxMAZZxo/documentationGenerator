using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DocumentationGenerator.MVVM.View.SettingsTabViews
{
    /// <summary>
    /// Interaction logic for FontSettingsView.xaml
    /// </summary>
    public partial class UCFontSettingsView : UserControl
    {
        private const string numbers = "0123456789";

        public UCFontSettingsView()
        {
            InitializeComponent();

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(sender == null) { return; }
            e.Handled = false;

            TextBox textBox = (TextBox)sender;
            if(e.Text == "0" && (string.IsNullOrEmpty(textBox.Text) || string.IsNullOrWhiteSpace(textBox.Text)) || 
                (string.IsNullOrEmpty(e.Text) || string.IsNullOrWhiteSpace(e.Text))) 
            { 
                e.Handled = true; 
                return; 
            }

            bool valid = false;
            for(int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] == e.Text[0])
                {
                    valid = true;
                    break;
                }
            }

            if(!valid) { e.Handled = true; }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // Block space
            }
        }
    }
}
