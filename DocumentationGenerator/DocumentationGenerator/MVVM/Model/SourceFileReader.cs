using DocumentationGenerator.MVVM.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.IO;

namespace DocumentationGenerator.MVVM.Model
{
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

        public async Task ReadSourceDirectory(string directory, ProgLanguage progLanguage)
        {
            List<string> filePaths = await Task.Run(() =>
                Directory.EnumerateFiles(
                    directory, "*.cs",
                    new EnumerationOptions { RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive, MatchType = MatchType.Simple }
                    ).ToList()
                );

            await ReadSourceFilesAsync(filePaths.ToArray(), progLanguage);
        }

        /// <summary>
        /// Attempts to read multiple source files asyncronously and awaits for all source files to be read until it Parses all the Results into the object.
        /// </summary>
        /// <param name="fileNames">The source files to read.</param>
        /// <returns>Returns the async Task.</returns>
        public async Task ReadSourceFilesAsync(string[] fileNames,ProgLanguage progLanguage)
        {
            IEnumerable<Task<ParsedSourceResults>>? tasks = fileNames.Select(path => ReadSourceFileAsync(path,progLanguage));

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
        private async Task<ParsedSourceResults> ReadSourceFileAsync(string sourceFile, ProgLanguage progLanguage)
        {
            string rawCode = await File.ReadAllTextAsync(sourceFile);

            ParsedSourceResults results = await Task.Run(() => ParseSourceResults(rawCode, progLanguage));

            await Task.Run(() => results.HandleCustomTypes());
            
            return results;
        }

        private ParsedSourceResults ParseSourceResults(string rawCode, ProgLanguage progLanguage)
        {
            ParsedSourceResults results;

            switch(progLanguage)
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
            if(Classes == null) { return ""; }

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

            if(Enums == null) { return ""; }

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

        public void HandleCustomTypes()
        {
            HandleClassCustomTypes();
            HandleStructCustomTypes();
            HandleInterfaceCustomTypes();
        }

        private void HandleClassCustomTypes()
        {
            for (int i = 0; i < Classes.Count; i++)
            {
                if (Classes[i].Fields != null)
                {
                    ClassDeclaration current = Classes[i];
                    current.Fields = HandleDeclarationType(current.Fields);
                    Classes[i] = current;
                }

                if (Classes[i].Methods != null)
                {
                    ClassDeclaration current = Classes[i];
                    current.Methods = HandleDeclarationType(current.Methods);
                    Classes[i] = current;
                }

                if (Classes[i].Properties != null)
                {
                    ClassDeclaration current = Classes[i];
                    current.Properties = HandleDeclarationType(current.Properties);
                    Classes[i] = current;
                }

            }
            
        }

        private Declaration[]? HandleDeclarationType(Declaration[]? declarations)
        {
            if (declarations == null) { return null; }
            for (int i = 0; i < declarations.Length; i++)
            {
                Declaration field = declarations[i];
                if (field.Type != null &&
                    field.IsTypePrimitive.HasValue &&
                    field.IsTypePrimitive.Value == false)
                {
                    field.WhatIsType = ObjectType.Class;
                    bool enumValid = FindTypeInCachedEnums(field.Type);
                    bool structValid = FindTypeInCachedStructs(field.Type);
                    bool interfaceValid = FindTypeInCachedInterfaces(field.Type);

                    if (enumValid)
                    {
                        field.WhatIsType = ObjectType.Enum;
                    }else if (structValid)
                    {
                        field.WhatIsType = ObjectType.Struct;
                    }else if(interfaceValid)
                    {
                        field.WhatIsType = ObjectType.Interface;
                    }

                        declarations[i] = field;
                }else if(field.Type != null && field.IsTypePrimitive.HasValue && field.IsTypePrimitive.Value == true)
                {
                    field.WhatIsType = ObjectType.Primitive;
                }
            }
            return declarations;
        }

        private bool FindTypeInCachedInterfaces(string type)
        {
            InterfaceDeclaration? interfaceDeclaration = null;
            try
            {
                interfaceDeclaration = Interfaces.First(current => current.Name == type);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }


            if (interfaceDeclaration.HasValue && interfaceDeclaration.Value.Name == type) { return true; }

            return false;
        }

        private bool FindTypeInCachedStructs(string type)
        {
            StructDeclaration? structDeclaration = null;
            try
            {
                structDeclaration = Structs.First(current => current.Name == type);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }

            if (structDeclaration.HasValue && structDeclaration.Value.Name == type) { return true; }

            return false;
        }

        private bool FindTypeInCachedEnums(string type)
        {
            EnumDeclaration? enumDeclaration = null;
            try
            {
                enumDeclaration = Enums.First(current => current.Name == type);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }


            if (enumDeclaration.HasValue && enumDeclaration.Value.Name == type) { return true; }

            return false;
        }

        private void HandleInterfaceCustomTypes()
        {
            for(int i = 0; i < Interfaces.Count; i++)
            {
                if (Interfaces[i].Properties != null)
                {
                    InterfaceDeclaration current = Interfaces[i];

                    current.Properties = HandleDeclarationType(Interfaces[i].Properties);

                    Interfaces[i] = current;
                }

                if (Interfaces[i].Methods != null)
                {
                    InterfaceDeclaration current = Interfaces[i];

                    current.Methods = HandleDeclarationType(Interfaces[i].Methods);

                    Interfaces[i] = current;
                }
            }
        }

        private void HandleStructCustomTypes()
        {
            for(int i = 0; i < Structs.Count ; i++)
            {
                if (Structs[i].Fields != null)
                {
                    StructDeclaration current = Structs[i];
                    current.Fields = HandleDeclarationType(current.Fields);
                    Structs[i] = current;
                }

                if (Structs[i].Methods != null)
                {
                    StructDeclaration current = Structs[i];
                    current.Methods = HandleDeclarationType(current.Methods);
                    Structs[i] = current;
                }

                if (Structs[i].Properties != null)
                {
                    StructDeclaration current = Structs[i];
                    current.Properties = HandleDeclarationType(current.Properties);
                    Structs[i] = current;
                }
            }
        }
    }
}