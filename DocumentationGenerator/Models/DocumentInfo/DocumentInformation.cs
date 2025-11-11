using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace DocumentationGenerator.Models.DocumentInfo;

public class DocumentInformation : IDisposable
{
    public DeclarationColours DeclarationColours { get; set; }
    public DeclarationFontStyles DeclarationFonts { get; set; }
    public bool GenerateTableOfContents { get; set; }
    public bool GeneratePageNumbers { get; set; }
    public bool GenerateInheritanceGraphs { get; set; }
    public bool PrintBaseTypes { get; set; }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
    public Bitmap? GlobalInheritanceGraph { get; set; }
    public Dictionary<string, Bitmap>? IndividualObjsGraphs { get; set; }
    public IStorageItem SavePath { get; }
    public IStorageFolder? GraphFolder { get; set; }

    public
    DocumentInformation(IStorageItem savePath, DeclarationColours declarationColours, DeclarationFontStyles declarationFonts, bool generateTableOfContents, bool generatePageNumbers, bool generateRelationshipGraph, bool printBaseTypes, string projectName, string projectDescription)
    {
        SavePath = savePath;
        DeclarationColours = declarationColours;
        DeclarationFonts = declarationFonts;
        GenerateTableOfContents = generateTableOfContents;
        GeneratePageNumbers = generatePageNumbers;
        GenerateInheritanceGraphs = generateRelationshipGraph;
        PrintBaseTypes = printBaseTypes;
        ProjectName = projectName;
        ProjectDescription = projectDescription;
    }

    public void Dispose()
    {
        if (IndividualObjsGraphs != null)
        {
            IndividualObjsGraphs.Clear();
        }

        if (GlobalInheritanceGraph != null)
        {
            GlobalInheritanceGraph.Dispose();
        }
    }
}

public class DeclarationFontStyles
{
    public string FontFamilyName { get; }
    public FontDeclarationStyle ObjectDeclarationStyle { get; }
    public FontDeclarationStyle ObjectDefinitionStyle { get; }
    public FontDeclarationStyle MemberHeadingStyle { get; }
    public FontDeclarationStyle MemberStyle { get; }
    public FontDeclarationStyle MemberTypeStyle { get; }
    public FontDeclarationStyle MemberDefinitionStyle { get; }

    public DeclarationFontStyles(string fontFamilyName, FontDeclarationStyle objectDeclarationStyle, FontDeclarationStyle objectDefinitionStyle, FontDeclarationStyle memberHeadingStyle, FontDeclarationStyle memberTypeStyle, FontDeclarationStyle memberStyle, FontDeclarationStyle memberDefinitionStyle)
    {
        FontFamilyName = fontFamilyName;
        ObjectDeclarationStyle = objectDeclarationStyle;
        ObjectDefinitionStyle = objectDefinitionStyle;
        MemberHeadingStyle = memberHeadingStyle;
        MemberStyle = memberStyle;
        MemberTypeStyle = memberTypeStyle;
        MemberDefinitionStyle = memberDefinitionStyle;
    }
}

public class FontDeclarationStyle : INotifyPropertyChanged
{
    private string fontSizeString = "";
    private bool isItalic;
    private bool isBold;
    private string spaceAfterString = "";

    public string FontSizeString
    {
        get => fontSizeString;
        set
        {
            fontSizeString = value;
            SetFontSize();
            OnPropertyChanged();
        }
    }

    public int FontSize { get; private set; }

    public bool IsItalic
    {
        get => isItalic;
        set
        {
            isItalic = value;
            OnPropertyChanged();
        }
    }

    public bool IsBold
    {
        get => isBold;
        set
        {
            isBold = value;

            OnPropertyChanged();
        }
    }

    public string SpaceAfterString
    {
        get => spaceAfterString;
        set
        {
            spaceAfterString = value;
            SetSpaceAfter();
            OnPropertyChanged();
        }
    }

    public int SpaceAfter { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public FontDeclarationStyle(string fontSizeString, bool isItalic, bool isBold, string spaceAfterString)
    {
        FontSizeString = fontSizeString;
        SpaceAfterString = spaceAfterString;
        IsItalic = isItalic;
        IsBold = isBold;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private void SetSpaceAfter()
    {
        if(string.IsNullOrEmpty(SpaceAfterString) || string.IsNullOrWhiteSpace(SpaceAfterString))
        {
            SpaceAfter = 1;
            return;
        }
        SpaceAfter = Convert.ToInt32(SpaceAfterString);
    }

    private void SetFontSize()
    {
        if(string.IsNullOrEmpty(FontSizeString) || string.IsNullOrWhiteSpace(FontSizeString))
        {
            FontSize = 1;
            return;
        }
        FontSize = Convert.ToInt32(FontSizeString);
    }

    public string GetValuesInStrings()
    {
        return $"{FontSize},{isBold},{IsItalic},{SpaceAfter}";
    }
}