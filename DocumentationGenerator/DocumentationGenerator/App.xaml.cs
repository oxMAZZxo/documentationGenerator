using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.View;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Win32;
using PdfSharp.Fonts;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DocumentationGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainView? mainView;
        public static App? Instance { get; private set; }
        public bool IsShuttingDown { get; private set; }

        public App()
        {
            if(Instance == null)
            {
                Instance = this;
            }else 
            { 
                return; 
            }

            GlobalFontSettings.FontResolver = new SimpleFontResolver();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            mainView = new MainView();
            mainView.Show();

            this.MainWindow = mainView;
            MainWindow.Closing += MainWindowClosing;
        }

        private void MainWindowClosing(object? sender, CancelEventArgs e)
        {
            IsShuttingDown = true;
            SettingsModel.Instance?.SaveSettings();

        }
    }




}
