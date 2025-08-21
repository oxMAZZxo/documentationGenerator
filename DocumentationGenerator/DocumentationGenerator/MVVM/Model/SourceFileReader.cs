using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.IO;

namespace DocumentationGenerator.MVVM.Model
{
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

        public async Task ReadSourceDirectory(string directory)
        {
            List<string> filePaths = await Task.Run(() =>
                Directory.EnumerateFiles(
                    directory, "*.cs",
                    new EnumerationOptions { RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive, MatchType = MatchType.Simple }
                    ).ToList()
                );

            await ReadSourceFilesAsync(filePaths.ToArray());
        }

        /// <summary>
        /// Attempts to read multiple source files asyncronously and awaits for all source files to be read until it Parses all the Results into the object.
        /// </summary>
        /// <param name="fileNames">The source files to read.</param>
        /// <returns>Returns the async Task.</returns>
        public async Task ReadSourceFilesAsync(string[] fileNames)
        {
            IEnumerable<Task<ParsedSourceResults>>? tasks = fileNames.Select(path => ReadSourceFileAsync(path));

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
        private async Task<ParsedSourceResults> ReadSourceFileAsync(string sourceFile)
        {
            string rawCode = await File.ReadAllTextAsync(sourceFile);

            ParsedSourceResults results = await Task.Run(() => ParseSourceResults(rawCode));

            await Task.Run(() => results.HandleCustomTypes());
            
            return results;
        }

        private ParsedSourceResults ParseSourceResults(string rawCode)
        {
            ParsedSourceResults results = new ParsedSourceResults();
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(rawCode);

            SyntaxNode root = syntaxTree.GetRoot();
            IEnumerable<SyntaxNode> nodes = root.ChildNodes();


            foreach (SyntaxNode node in nodes)
            {
                switch (node)
                {
                    case ClassDeclarationSyntax classDec:

                        results.Classes.Add(HandleClassDeclaration(classDec));

                        break;

                    case EnumDeclarationSyntax enumDec:

                        results.Enums.Add(HandleEnumDeclaration(enumDec));

                        break;

                    case InterfaceDeclarationSyntax interfaceDec:
                        results.Interfaces.Add(HandleInterfaceDeclaration(interfaceDec));
                        break;

                    case StructDeclarationSyntax structDec:
                        results.Structs.Add(HandleStructDeclaration(structDec));
                        break;

                }
            }

            return results;
        }

        private StructDeclaration HandleStructDeclaration(StructDeclarationSyntax structDec)
        {
            string structName = structDec.Identifier.Text;
            string structDefinition = GetXML(structDec, XmlTag.summary);

            IEnumerable<PropertyDeclarationSyntax> properties = structDec.Members.OfType<PropertyDeclarationSyntax>();
            IEnumerable<FieldDeclarationSyntax> fields = structDec.Members.OfType<FieldDeclarationSyntax>();
            IEnumerable<MethodDeclarationSyntax> methods = structDec.Members.OfType<MethodDeclarationSyntax>();
            //IEnumerable<AccessorDeclarationSyntax> functions = structDec.Modifiers.OfType<AccessorDeclarationSyntax>();

            Declaration[]? newProperties = null;
            Declaration[]? newFields = null;
            Declaration[]? newMethods = null;
            int index = 0;

            if (properties.Count() > 0)
            {
                newProperties = new Declaration[properties.Count()];
                foreach (var property in properties)
                {
                    newProperties[index] = new Declaration(
                        property.Identifier.Text,
                        GetXML(property, XmlTag.summary),
                        property.Type.ToString(),
                        null,
                        IsPrimitiveType(property.Type.ToString())
                    );
                    index++;
                }
            }

            if (fields.Count() > 0)
            {
                index = 0;
                newFields = new Declaration[fields.Count()];
                foreach (var field in fields)
                {
                    foreach (var variable in field.Declaration.Variables)
                    {
                        newFields[index] = new Declaration(
                            variable.Identifier.Text,
                            GetXML(field, XmlTag.summary),
                            field.Declaration.Type.ToString(),
                            null,
                            IsPrimitiveType(field.Declaration.Type.ToString())
                        );
                        index++;
                    }
                }
            }

            if (methods.Count() > 0)
            {
                newMethods = new Declaration[methods.Count()];
                index = 0;
                foreach (var method in methods)
                {
                    newMethods[index] = new Declaration(
                        method.Identifier.Text,
                        GetXML(method, XmlTag.summary),
                        method.ReturnType.ToString(),
                        GetXML(method, XmlTag.returns),
                        IsPrimitiveType(method.ReturnType.ToString())
                    );
                    index++;
                }
            }

            return new StructDeclaration(structName, structDefinition, newProperties, newFields, newMethods);
        }

        private InterfaceDeclaration HandleInterfaceDeclaration(InterfaceDeclarationSyntax interfaceDec)
        {
            string interfaceName = interfaceDec.Identifier.Text;
            string interfaceDefinition = GetXML(interfaceDec, XmlTag.summary);

            IEnumerable<PropertyDeclarationSyntax> properties = interfaceDec.Members.OfType<PropertyDeclarationSyntax>();
            IEnumerable<MethodDeclarationSyntax> methods = interfaceDec.Members.OfType<MethodDeclarationSyntax>();

            Declaration[]? newProperties = null;
            Declaration[]? newMethods = null;
            int index = 0;

            if (properties.Count() > 0)
            {
                newProperties = new Declaration[properties.Count()];
                foreach (PropertyDeclarationSyntax property in properties)
                {
                    newProperties[index] = new Declaration(
                        property.Identifier.Text,
                        GetXML(property, XmlTag.summary),
                        property.Type.ToString(),
                        null,
                        IsPrimitiveType(property.Type.ToString())
                    );
                    index++;
                }
            }

            if (methods.Count() > 0)
            {
                newMethods = new Declaration[methods.Count()];
                index = 0;

                foreach (MethodDeclarationSyntax method in methods)
                {
                    newMethods[index] = new Declaration(
                        method.Identifier.Text,
                        GetXML(method, XmlTag.summary),
                        method.ReturnType.ToString(),
                        GetXML(method, XmlTag.returns),
                        IsPrimitiveType(method.ReturnType.ToString())
                    );
                    index++;
                }
            }

            return new InterfaceDeclaration(interfaceName, interfaceDefinition,newProperties, newMethods);
        }

        /// <summary>
        /// Creates a new Enum Declaration object with the data from the given Enum Declaration Syntax Node.
        /// </summary>
        /// <param name="enumDec">The Syntax Node to get the data from.</param>
        /// <returns>Returns an Enum Declaration containing all the members found in the given syntax, along with all their definitions read from the Trivia.</returns>
        private EnumDeclaration HandleEnumDeclaration(EnumDeclarationSyntax enumDec)
        {
            Declaration[] enumMembers = new Declaration[enumDec.Members.Count()];
            int index = 0;

            foreach (EnumMemberDeclarationSyntax member in enumDec.Members)
            {
                enumMembers[index] = new Declaration(member.Identifier.Text, GetXML(member, XmlTag.summary),null, null);
                index++;
            }

            return new EnumDeclaration(enumDec.Identifier.Text, GetXML(enumDec, XmlTag.summary), enumMembers);
        }

        /// <summary>
        /// Creates a new Class Declaration object with the data from the given Class Declaration Syntax Node.
        /// </summary>
        /// <param name="classDec">The Syntax Node to get the data from.</param>
        /// <returns>Returns a Class Declaration containing all the fields, properties, methods and functions found in the given syntax, along with all their definitions read from the Trivia.</returns>
        private ClassDeclaration HandleClassDeclaration(ClassDeclarationSyntax classDec)
        {
            string className = classDec.Identifier.Text;
            string classDefinition = GetXML(classDec, XmlTag.summary);

            IEnumerable<PropertyDeclarationSyntax> properties = classDec.Members.OfType<PropertyDeclarationSyntax>();
            IEnumerable<FieldDeclarationSyntax> fields = classDec.Members.OfType<FieldDeclarationSyntax>();
            IEnumerable<MethodDeclarationSyntax> methods = classDec.Members.OfType<MethodDeclarationSyntax>();

            Declaration[]? newProperties = null;
            Declaration[]? newFields = null;
            Declaration[]? newMethods = null;
            int index = 0;

            if (properties.Count() > 0)
            {
                newProperties = new Declaration[properties.Count()];
                foreach (PropertyDeclarationSyntax property in properties)
                {
                    newProperties[index] = new Declaration(property.Identifier.Text, 
                        GetXML(property, XmlTag.summary), property.Type.ToString(), null, IsPrimitiveType(property.Type.ToString()));
                    
                    index++;
                }
            }

            if (fields.Count() > 0)
            {
                index = 0;
                newFields = new Declaration[fields.Count()];

                foreach (FieldDeclarationSyntax field in fields)
                {
                    foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                    {
                        //tempOutput += $"  {field.Declaration.Type} {variable.Identifier.Text} - {GetXML(field, XmlTag.summary)}" + Environment.NewLine;
                        newFields[index] = new Declaration(variable.Identifier.Text, 
                            GetXML(field, XmlTag.summary), field.Declaration.Type.ToString(),null,IsPrimitiveType(field.Declaration.Type.ToString()));
                        index++;
                    }
                }

            }

            if (methods.Count() > 0)
            {
                newMethods = new Declaration[methods.Count()];

                index = 0;

                foreach (MethodDeclarationSyntax method in methods)
                {
                    newMethods[index] = new Declaration(method.Identifier.Text, GetXML(method, XmlTag.summary), 
                        method.ReturnType.ToString(), GetXML(method, XmlTag.returns),IsPrimitiveType(method.ReturnType.ToString()));
                    index++;
                }
            }

            return new ClassDeclaration(className, classDefinition, newMethods, newFields, newProperties);
        }

        /// <summary>
        /// Attempts to find the XML comment for the given syntax node.
        /// </summary>
        /// <param name="node">The node that may contain a XML comment trivia.</param>
        /// <param name="tag">The type of XML comment to look for.</param>
        /// <returns></returns>
        private string GetXML(SyntaxNode node, XmlTag tag)
        {
            SyntaxToken token = node.GetFirstToken();
            SyntaxTriviaList triviaList = token.LeadingTrivia;

            foreach (SyntaxTrivia trivia in triviaList)
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    SyntaxNode? structure = trivia.GetStructure();
                    if (structure == null) { return $" NO {tag.ToString().ToUpper()} "; }

                    XmlElementSyntax? summary = structure.ChildNodes()
                        .OfType<XmlElementSyntax>()
                        .FirstOrDefault(e => e.StartTag.Name.LocalName.Text == tag.ToString());

                    if (summary != null)
                    {
                        return CleanXML(summary.GetText().ToString(), tag);
                    }
                }
            }

            return $" NO {tag.ToString().ToUpper()} ";
        }

        /// <summary>
        /// Cleans the XML Comment by removing the comment characters (such as '/') and returns the declaration itself.
        /// </summary>
        /// <param name="rawComment">The XML comment.</param>
        /// <param name="tag">The type of XML (returns, summary, etc.)</param>
        /// <returns></returns>
        private string CleanXML(string rawComment, XmlTag tag)
        {
            var lines = rawComment.Split('\n')
                                  .Select(line => line.Trim())
                                  .Where(line => !string.IsNullOrWhiteSpace(line))
                                  .Select(line =>
                                  {
                                      if (line.StartsWith("///"))
                                          line = line.Substring(3).Trim();
                                      line = line.Replace($"<{tag.ToString()}>", "")
                                                 .Replace($"</{tag.ToString()}>", "")
                                                 .Trim();
                                      return line;
                                  });

            return string.Join(" ", lines);
        }

        /// <summary>
        /// Determines whether the given type in a string is a primitive.
        /// </summary>
        /// <param name="type">The type in a string format.</param>
        /// <returns>Returns true if is a primitive, otherwise false.</returns>
        private bool IsPrimitiveType(string type)
        {
            if(type == "int") { return true; }
            if(type == "bool") { return true; }
            if(type == "sbyte") { return true; }
            if(type == "int16") { return true; }
            if(type == "uint16") { return true; }
            if(type == "int32") { return true; }
            if(type == "uint32") { return true; }
            if(type == "int64") { return true; }
            if(type == "uint64") { return true; }
            if(type == "single") { return true; }
            if(type == "double") { return true; }
            if(type == "char") { return true; }
            if(type == "float") { return true; }
            if(type == "void") { return true; }

            return false;
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
            Classes = null;
            Enums = null;
            Interfaces = null;
            Structs = null;
        }
    }


    public enum XmlTag
    {
        summary,
        returns
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