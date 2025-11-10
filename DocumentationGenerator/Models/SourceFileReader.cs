using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using DocumentationGenerator.Helpers;
using DocumentationGenerator.Models.Declarations;
using DocumentationGenerator.Models.LanguageParsers;
using Microsoft.CodeAnalysis.CSharp;

namespace DocumentationGenerator.Models;

public enum ProgLanguage
{
    CSharp,
    VisualBasic,
    CPP
}

public class SourceFileReader : IDisposable
{
    public List<ClassDeclaration> Classes { get; private set; }
    public List<EnumDeclaration> Enums { get; private set; }
    public List<InterfaceDeclaration> Interfaces { get; private set; }
    public List<StructDeclaration> Structs { get; private set; }
    public bool HasData { get; private set; }

    public SourceFileReader()
    {
        Classes = new List<ClassDeclaration>();
        Enums = new List<EnumDeclaration>();
        Interfaces = new List<InterfaceDeclaration>();
        Structs = new List<StructDeclaration>();
    }

    ~SourceFileReader()
    {
        Dispose();
    }

    public async Task ReadSourceDirectory(IStorageFolder directory, ProgLanguage progLanguage)
    {
        string searchPattern = "*";
        switch (progLanguage)
        {
            case ProgLanguage.CSharp:
                searchPattern = "*.cs";
                break;
        }

        List<IStorageFile> files = await Utilities.EnumerateAllFilesAsync(directory,searchPattern);
        await ReadSourceFilesAsync(files, progLanguage);
    }

    /// Attempts to read multiple source files asyncronously and awaits for all source files to be read until it Parses all the Results into the object.
    /// </summary>
    /// <param name="fileNames">The source files to read.</param>
    /// <returns>Returns the async Task.</returns>
    public async Task ReadSourceFilesAsync(List<IStorageFile> fileNames, ProgLanguage progLanguage)
    {
        IEnumerable<Task<ParsedSourceResults>>? tasks = fileNames.Select(path => ReadSourceFileAsync(path, progLanguage));

        ParsedSourceResults[] results = await Task.WhenAll(tasks);

        foreach (ParsedSourceResults parsedSourceResults in results)
        {
            Classes.AddRange(parsedSourceResults.Classes);
            Enums.AddRange(parsedSourceResults.Enums);
            Interfaces.AddRange(parsedSourceResults.Interfaces);
            Structs.AddRange(parsedSourceResults.Structs);
        }

        if (Classes.Count > 0 || Enums.Count > 0 || Structs.Count > 0 || Interfaces.Count > 0)
        {
            HasData = true;
        }
    }

    /// <summary>
    /// Reads a given source file for Syntax Node to extract into Declaration along with their Definitions if any.
    /// </summary>
    /// <param name="sourceFile">The source file to read.</param>
    /// <returns>Returns the Parsed Source Results which may contain Class, Enum, etc. Declarations.</returns>
    private async Task<ParsedSourceResults> ReadSourceFileAsync(IStorageFile sourceFile, ProgLanguage progLanguage)
    {
        if(sourceFile.Name == "NPC.cs")
        {
            Debug.WriteLine($"Reading NPC.cs file");
        }
        Stream? stream = await sourceFile.OpenReadAsync();
        StreamReader streamReader = new StreamReader(stream);
        string rawCode = await streamReader.ReadToEndAsync();
        ParsedSourceResults results = ParseSourceResults(rawCode, progLanguage);

        await Task.Run(() => results.HandleCustomTypes());

        streamReader.Close();
        streamReader.Dispose();
        stream.Close();
        await stream.DisposeAsync();

        return results;
    }

    private ParsedSourceResults ParseSourceResults(string rawCode, ProgLanguage progLanguage)
    {
        ParsedSourceResults results;
        switch (progLanguage)
        {
            //case ProgLanguage.VisualBasic: syntaxTree = VisualBasicSyntaxTree.ParseText(rawCode); break;
            default: results = CSharpDeclarationsParser.ReadAllDeclarations(CSharpSyntaxTree.ParseText(rawCode)); break;
        }
        return results;
    }

    /// <summary>
    /// Clears all declarations cached.
    /// </summary>
    public void Clear()
    {
        Classes.Clear();
        Enums.Clear();
        Interfaces.Clear();
        Structs.Clear();
        HasData = false;
    }

    /// <summary>
    /// Gets all the declaration read from the source files combining them into a string. The only formatting it applies is tab spacing and new lines.
    /// </summary>
    /// <returns>Returns a string with all declarations.</returns>
    public string GetAllDeclarations()
    {
        string output = "";

        output += GetClassDeclarations();
        output += GetEnumDeclarations();
        output += GetInterfaceDeclarations();

        return output;
    }

    /// <summary>
    /// Assembles all Class Declarations from the sources.  The only formatting it applies is tab spacing and new lines.
    /// </summary>
    /// <returns>Returns a string with all class declarations and their fields, properties and method/functions.</returns>
    public string GetClassDeclarations()
    {
        string output = "";
        if (Classes == null) { return ""; }

        foreach (ClassDeclaration classDeclaration in Classes)
        {
            output += $"Class: {classDeclaration.Name} - {classDeclaration.Definition}\n";
            if (classDeclaration.Properties != null)
            {
                output += $"    Properties:\n";
                foreach (Declaration declaration in classDeclaration.Properties)
                {
                    output += $"    {declaration.Type} {declaration.Name} - {declaration.Definition}\n";
                }

            }

            if (classDeclaration.Fields != null)
            {
                output += $"    Fields:\n";
                foreach (Declaration declaration in classDeclaration.Fields)
                {
                    output += $"    {declaration.Type} {declaration.Name} - {declaration.Definition}\n";
                }

            }

            if (classDeclaration.Methods != null)
            {
                output += $"    Methods:\n";
                foreach (Declaration declaration in classDeclaration.Methods)
                {
                    output += $"    {declaration.Type} {declaration.Name} - {declaration.Definition} - {declaration.ReturnDefinition}\n";
                }

            }
        }

        return output;
    }

    public string GetInterfaceDeclarations()
    {
        string output = "";

        if (Interfaces == null) return "";

        foreach (InterfaceDeclaration interfaceDeclaration in Interfaces)
        {
            output += $"Interface: {interfaceDeclaration.Name} - {interfaceDeclaration.Definition}\n";

            if (interfaceDeclaration.Properties != null)
            {
                output += $"    Properties:\n";
                foreach (Declaration declaration in interfaceDeclaration.Properties)
                {
                    output += $"    {declaration.Type} {declaration.Name} - {declaration.Definition}\n";
                }
            }

            if (interfaceDeclaration.Methods != null)
            {
                output += $"    Methods:\n";
                foreach (Declaration declaration in interfaceDeclaration.Methods)
                {
                    output += $"    {declaration.Type} {declaration.Name} - {declaration.Definition} - {declaration.ReturnDefinition}\n";
                }
            }
        }

        return output;
    }

    /// <summary>
    /// Assembles all Enum Declarations from the sources.  The only formatting it applies is tab spacing and new lines.
    /// </summary>
    /// <returns>Returns a string with all enum declarations and their members.</returns>
    public string GetEnumDeclarations()
    {
        string output = "";

        if (Enums == null) { return ""; }

        foreach (EnumDeclaration enumDeclaration in Enums)
        {
            output += $"Enum: {enumDeclaration.Name} - {enumDeclaration.Definition}\n   Members:\n";

            foreach (Declaration declaration in enumDeclaration.EnumMembers)
            {
                output += $"    {declaration.Name} - {declaration.Definition}\n";
            }
        }

        return output;
    }

    public void Dispose()
    {
        Clear();
    }
}


public class ParsedSourceResults
{
    public string FilePath { get; set; }
    public List<ClassDeclaration> Classes { get; set; }
    public List<EnumDeclaration> Enums { get; set; }
    public List<InterfaceDeclaration> Interfaces { get; set; }
    public List<StructDeclaration> Structs { get; set; }

    public ParsedSourceResults()
    {
        FilePath = "";
        Classes = new List<ClassDeclaration>();
        Enums = new List<EnumDeclaration>();
        Interfaces = new List<InterfaceDeclaration>();
        Structs = new List<StructDeclaration>();
    }

    public void Add(ParsedSourceResults results)
    {
        if (results == null) { return; }
        
        if (results.Classes != null && results.Classes.Count > 0)
        {
            Classes.AddRange(results.Classes);
        }
        if (results.Interfaces != null && results.Interfaces.Count > 0)
        {
            Interfaces.AddRange(results.Interfaces);
        }
        if (results.Structs != null && results.Structs.Count > 0)
        {
            Structs.AddRange(results.Structs);
        }
        if(results.Enums != null && results.Enums.Count > 0)
        {
            Enums.AddRange(results.Enums);
        }
    }

    public void HandleCustomTypes()
    {
        var enumNames = new HashSet<string>(Enums.Select(e => e.Name));
        var structNames = new HashSet<string>(Structs.Select(s => s.Name));
        var interfaceNames = new HashSet<string>(Interfaces.Select(i => i.Name));

        HandleClassCustomTypes(enumNames, structNames, interfaceNames);
        HandleStructCustomTypes(enumNames, structNames, interfaceNames);
        HandleInterfaceCustomTypes(enumNames, structNames, interfaceNames);
    }

    private void HandleClassCustomTypes(HashSet<string> enumNames, HashSet<string> structNames, HashSet<string> interfaceNames)
    {
        for (int i = 0; i < Classes.Count; i++)
        {
            if (Classes[i].Fields != null)
            {
                ClassDeclaration current = Classes[i];
                current.Fields = HandleDeclarationType(current.Fields, enumNames, structNames, interfaceNames);
                Classes[i] = current;
            }

            if (Classes[i].Methods != null)
            {
                ClassDeclaration current = Classes[i];
                current.Methods = HandleDeclarationType(current.Methods, enumNames, structNames, interfaceNames);
                Classes[i] = current;
            }

            if (Classes[i].Properties != null)
            {
                ClassDeclaration current = Classes[i];
                current.Properties = HandleDeclarationType(current.Properties, enumNames, structNames, interfaceNames);
                Classes[i] = current;
            }

        }
    }

    private Declaration[]? HandleDeclarationType(Declaration[]? declarations, HashSet<string> enumNames, HashSet<string> structNames, HashSet<string> interfaceNames)
    {
        if (declarations == null) { return null; }
        for (int i = 0; i < declarations.Length; i++)
        {
            Declaration field = declarations[i];
            if (field.Type != null &&
                field.IsTypePrimitive.HasValue &&
                field.IsTypePrimitive.Value == false)
            {
                field.WhatIsType = ObjectType.Class; //default

                if (enumNames.Contains(field.Type))
                {
                    field.WhatIsType = ObjectType.Enum;
                }
                else if (structNames.Contains(field.Type))
                {
                    field.WhatIsType = ObjectType.Struct;
                }
                else if (interfaceNames.Contains(field.Type))
                {
                    field.WhatIsType = ObjectType.Interface;
                }

                declarations[i] = field;
            }
            else if (field.Type != null && field.IsTypePrimitive.HasValue && field.IsTypePrimitive.Value == true)
            {
                field.WhatIsType = ObjectType.Primitive;
            }
        }
        return declarations;
    }

    private void HandleInterfaceCustomTypes(HashSet<string> enumNames, HashSet<string> structNames, HashSet<string> interfaceNames)
    {
        for (int i = 0; i < Interfaces.Count; i++)
        {
            if (Interfaces[i].Properties != null)
            {
                InterfaceDeclaration current = Interfaces[i];

                current.Properties = HandleDeclarationType(Interfaces[i].Properties, enumNames, structNames, interfaceNames);

                Interfaces[i] = current;
            }

            if (Interfaces[i].Methods != null)
            {
                InterfaceDeclaration current = Interfaces[i];

                current.Methods = HandleDeclarationType(Interfaces[i].Methods, enumNames, structNames, interfaceNames);

                Interfaces[i] = current;
            }
        }
    }

    private void HandleStructCustomTypes(HashSet<string> enumNames, HashSet<string> structNames, HashSet<string> interfaceNames)
    {
        for (int i = 0; i < Structs.Count; i++)
        {
            if (Structs[i].Fields != null)
            {
                StructDeclaration current = Structs[i];
                current.Fields = HandleDeclarationType(current.Fields, enumNames, structNames, interfaceNames);
                Structs[i] = current;
            }

            if (Structs[i].Methods != null)
            {
                StructDeclaration current = Structs[i];
                current.Methods = HandleDeclarationType(current.Methods, enumNames, structNames, interfaceNames);
                Structs[i] = current;
            }

            if (Structs[i].Properties != null)
            {
                StructDeclaration current = Structs[i];
                current.Properties = HandleDeclarationType(current.Properties, enumNames, structNames, interfaceNames);
                Structs[i] = current;
            }
        }
    }
}