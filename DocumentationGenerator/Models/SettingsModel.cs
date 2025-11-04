using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media;
using DocumentationGenerator.Helpers;
using DocumentationGenerator.Models.DocumentInfo;

namespace DocumentationGenerator.Models;


public class SettingsModel
{
    public static SettingsModel Instance { get; private set; }
    public MigraDoc.DocumentObjectModel.Color MigraDocClassDeclarationColour { get; private set; }
    public MigraDoc.DocumentObjectModel.Color MigraDocPrimitiveDeclarationColour { get; private set; }
    public MigraDoc.DocumentObjectModel.Color MigraDocEnumDeclarationColour { get; private set; }
    public MigraDoc.DocumentObjectModel.Color MigraDocInterfaceDeclarationColour { get; private set; }
    public MigraDoc.DocumentObjectModel.Color MigraDocStructDeclarationColour { get; private set; }

    public Color ClassDeclarationColour { get; set; }
    public Color PrimitiveDeclarationColour { get; set; }
    public Color EnumDeclarationColour { get; set; }
    public Color InterfaceDeclarationColour { get; set; }
    public Color StructDeclarationColour { get; set; }

    public FontDeclarationStyle ObjectDeclarationStyle { get; set; }
    public FontDeclarationStyle ObjectDefinitionStyle { get; set; }
    public FontDeclarationStyle MemberHeadingStyle { get; set; }
    public FontDeclarationStyle MemberStyle { get; set; }
    public FontDeclarationStyle MemberTypeStyle { get; set; }
    public FontDeclarationStyle MemberDefinitionStyle { get; set; }
    public string SelectedFont { get; set; }
    public bool GenerateTableOfContents { get; set; }
    public bool GeneratePageNumbers { get; set; }
    public bool AddDocumentRelationshipGraph { get; set; }
    public bool PrintBaseTypesToDocument { get; set; }
    public ObservableCollection<string> Fonts { get; } = new ObservableCollection<string> { "Arial", "Arial Black", "Courier New" };

    public SettingsModel()
    {
        if (Instance != null)
        {
            return;
        }

        if (Instance == null && Instance != this)
        {
            Instance = this;
        }

        if (TryLoadSettings() == false)
        {
            AssignDefaultValues();
        }
    }

    private void AssignDefaultValues()
    {
        ClassDeclarationColour = Color.FromRgb(0, 200, 100);
        PrimitiveDeclarationColour = Color.FromRgb(0, 0, 255);
        EnumDeclarationColour = Color.FromRgb(107, 142, 35);
        InterfaceDeclarationColour = Color.FromRgb(0, 128, 128);
        StructDeclarationColour = Color.FromRgb(184, 134, 11);

        SelectedFont = Fonts[0];
        GeneratePageNumbers = true;
        GenerateTableOfContents = true;
        AddDocumentRelationshipGraph = true;
        PrintBaseTypesToDocument = true;

        ObjectDeclarationStyle = new FontDeclarationStyle("20", false, true, "5");
        ObjectDefinitionStyle = new FontDeclarationStyle("18", true, false, "20");
        MemberHeadingStyle = new FontDeclarationStyle("16", false, true, "2");
        MemberTypeStyle = new FontDeclarationStyle("14", false, true, "1");
        MemberStyle = new FontDeclarationStyle("14", false, false, "1");
        MemberDefinitionStyle = new FontDeclarationStyle("14", true, false, "1");
    }

    private bool TryLoadSettings()
    {
        string[] lines;
        try
        {
            lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "settings.txt"));
        }
        catch (DirectoryNotFoundException)
        {
            File.Create(Path.Combine(AppContext.BaseDirectory, "settings.txt")).Close();
            return false;
        }
        catch (FileNotFoundException)
        {
            File.Create(Path.Combine(AppContext.BaseDirectory, "settings.txt")).Close();
            return false;
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"The program was not able load the saved settings preferences due to an IOException.\n Exception Message:{ex.Message}");
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            Debug.WriteLine($"The program could not load the saved settings file because of unauthorised access.");
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"The program could not load the saved settings file due to a random error.\n Exception Message:{ex.Message}");
            return false;
        }

        try
        {

            ClassDeclarationColour = Color.FromArgb(Utilities.HexToByte(lines[1].Substring(0, 2)), Utilities.HexToByte(lines[1].Substring(2, 2)),
                Utilities.HexToByte(lines[1].Substring(4, 2)), Utilities.HexToByte(lines[1].Substring(6, 2)));
            InterfaceDeclarationColour = Color.FromArgb(Utilities.HexToByte(lines[2].Substring(0, 2)), Utilities.HexToByte(lines[2].Substring(2, 2)),
                Utilities.HexToByte(lines[2].Substring(4, 2)), Utilities.HexToByte(lines[2].Substring(6, 2)));
            StructDeclarationColour = Color.FromArgb(Utilities.HexToByte(lines[3].Substring(0, 2)), Utilities.HexToByte(lines[3].Substring(2, 2)),
                Utilities.HexToByte(lines[3].Substring(4, 2)), Utilities.HexToByte(lines[3].Substring(6, 2)));
            EnumDeclarationColour = Color.FromArgb(Utilities.HexToByte(lines[4].Substring(0, 2)), Utilities.HexToByte(lines[4].Substring(2, 2)),
                Utilities.HexToByte(lines[4].Substring(4, 2)), Utilities.HexToByte(lines[4].Substring(6, 2)));
            PrimitiveDeclarationColour = Color.FromArgb(Utilities.HexToByte(lines[5].Substring(0, 2)), Utilities.HexToByte(lines[5].Substring(2, 2)),
                Utilities.HexToByte(lines[5].Substring(4, 2)), Utilities.HexToByte(lines[5].Substring(6, 2)));

            string[] values = lines[7].Split(",");
            ObjectDeclarationStyle = new FontDeclarationStyle(values[0], Convert.ToBoolean(values[1]), Convert.ToBoolean(values[2]), values[3]);

            values = lines[8].Split(",");
            ObjectDefinitionStyle = new FontDeclarationStyle(values[0], Convert.ToBoolean(values[1]), Convert.ToBoolean(values[2]), values[3]);

            values = lines[9].Split(",");
            MemberHeadingStyle = new FontDeclarationStyle(values[0], Convert.ToBoolean(values[1]), Convert.ToBoolean(values[2]), values[3]);

            values = lines[10].Split(",");
            MemberTypeStyle = new FontDeclarationStyle(values[0], Convert.ToBoolean(values[1]), Convert.ToBoolean(values[2]), values[3]);

            values = lines[11].Split(",");
            MemberStyle = new FontDeclarationStyle(values[0], Convert.ToBoolean(values[1]), Convert.ToBoolean(values[2]), values[3]);

            values = lines[12].Split(",");
            MemberDefinitionStyle = new FontDeclarationStyle(values[0], Convert.ToBoolean(values[1]), Convert.ToBoolean(values[2]), values[3]);

            SelectedFont = lines[13];

            GeneratePageNumbers = Convert.ToBoolean(lines[14]);

            GenerateTableOfContents = Convert.ToBoolean(lines[15]);

            AddDocumentRelationshipGraph = Convert.ToBoolean(lines[16]);

            PrintBaseTypesToDocument = Convert.ToBoolean(lines[17]);

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Settings file was fully loaded but could not extract data.\nException Message:{ex.Message}");
            return false;
        }

        return true;
    }


    public void SaveSettings()
    {
        StreamWriter writer = new StreamWriter(Path.Combine(AppContext.BaseDirectory, "settings.txt"), false);

        writer.WriteLine("Colours (C,I,S,E,P):");
        writer.WriteLine(ClassDeclarationColour.ToString().Replace("#", ""));
        writer.WriteLine(InterfaceDeclarationColour.ToString().Replace("#", ""));
        writer.WriteLine(StructDeclarationColour.ToString().Replace("#", ""));
        writer.WriteLine(EnumDeclarationColour.ToString().Replace("#", ""));
        writer.WriteLine(PrimitiveDeclarationColour.ToString().Replace("#", ""));

        writer.WriteLine("FontDeclarationStyle (ObjDec,ObjDef,MemberHead,MemberType,MemberStyle,MemberDef):");
        writer.WriteLine(ObjectDeclarationStyle.GetValuesInStrings());
        writer.WriteLine(ObjectDefinitionStyle.GetValuesInStrings());
        writer.WriteLine(MemberHeadingStyle.GetValuesInStrings());
        writer.WriteLine(MemberTypeStyle.GetValuesInStrings());
        writer.WriteLine(MemberStyle.GetValuesInStrings());
        writer.WriteLine(MemberDefinitionStyle.GetValuesInStrings());

        writer.WriteLine(SelectedFont);
        writer.WriteLine(GeneratePageNumbers.ToString());
        writer.WriteLine(GenerateTableOfContents.ToString());
        writer.WriteLine(AddDocumentRelationshipGraph.ToString());
        writer.WriteLine(PrintBaseTypesToDocument.ToString());

        writer.Close();
        writer.Dispose();
    }

    public void SetMigraDocClassDeclarationColour(byte r, byte g, byte b)
    {
        // MigraDocClassDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255, r, g, b);
    }

    public void SetMigraDocPrimitiveDeclarationColour(byte r, byte g, byte b)
    {
        // MigraDocPrimitiveDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255, r, g, b);
    }

    public void SetMigraDocInterfaceDeclarationColour(byte r, byte g, byte b)
    {
        // MigraDocInterfaceDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255, r, g, b);
    }

    public void SetMigraDocEnumDeclarationColour(byte r, byte g, byte b)
    {
        // MigraDocEnumDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255, r, g, b);
    }

    public void SetMigraDocStructDeclarationColour(byte r, byte g, byte b)
    {
        // MigraDocStructDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255, r, g, b);
    }
}