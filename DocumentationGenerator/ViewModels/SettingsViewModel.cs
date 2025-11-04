using System;
using System.Diagnostics;
using Avalonia.Controls;
using DocumentationGenerator.Views.SettingsViews;

namespace DocumentationGenerator.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private UserControl currentUserControl;
    private SettingsTab selectedTab;
    private UCGeneralSettings UC_GeneralSettings;
    private UCColourSettings UC_ColourSettings;
    private UCFontSettings UC_FontSettings;


    public UserControl CurrentUserControl
    {
        get => currentUserControl;
        set
        {
            currentUserControl = value;
            OnPropertyChanged(nameof(CurrentUserControl));
        }
    }
    public SettingsTab SelectedTab
    {
        get => selectedTab;
        set
        {
            selectedTab = value;
            ChangeUC();
            OnPropertyChanged(nameof(SelectedTab));
        }
    }
    public bool GeneratePageNumbers { get; set; }
    public bool GenerateTableOfContents { get; set; }
    public bool AddDocumentRelationshipGraph { get; set; }
    public bool PrintBaseTypesToDocument { get; set; }
    

    public SettingsViewModel()
    {
        UC_GeneralSettings = new UCGeneralSettings();
        UC_ColourSettings = new UCColourSettings();
        UC_FontSettings = new UCFontSettings();
        SelectedTab = SettingsTab.Colours;
        
    }

    private void ChangeUC()
    {
        switch (selectedTab)
        {
            case SettingsTab.General:
                CurrentUserControl = UC_GeneralSettings;
                break;
            case SettingsTab.Colours:
                CurrentUserControl = UC_ColourSettings;
                break;

            case SettingsTab.Fonts:
                CurrentUserControl = UC_FontSettings;
                break;
        }
    }
}

public enum SettingsTab
{
    General,
    Colours,
    Fonts
}