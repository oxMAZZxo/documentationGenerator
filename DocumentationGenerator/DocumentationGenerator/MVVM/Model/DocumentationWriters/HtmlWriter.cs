using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationGenerator.MVVM.Model.DocumentationWriters
{
    public class HtmlWriter
    {
        public bool Write(string path, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docInfo)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) { return false; }

            DirectoryInfo outputPath = Directory.CreateDirectory(Path.Combine(path, $"{docInfo.ProjectName} Documentation"));

            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/helper.js"), Path.Combine(outputPath.FullName, "helper.js"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/docsHelpers.js"), Path.Combine(outputPath.FullName, "docsHelpers.js"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/homePageStyles.css"), Path.Combine(outputPath.FullName, "homePageStyles.css"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/sidebar.css"), Path.Combine(outputPath.FullName, "sidebar.css"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/docStyles.css"), Path.Combine(outputPath.FullName, "docStyles.css"), true);
            string sidebar = GenerateSideBar(classes, enums, interfaces, structs,docInfo);
            GenerateHomePage(outputPath.FullName, sidebar, docInfo);
            GenerateObjPages(outputPath.FullName, classes, enums, interfaces, structs, sidebar,docInfo);

            if (docInfo.GenerateRelationshipGraph)
            {
                // string graphPath = GenerateRelationshipGraph(classes, interfaces, documentStyling.DeclarationColours, path);
            }
            return true;
        }

        private void GenerateObjPages(string fullName, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, string sideBar ,DocumentInformation docInfo)
        {
            DirectoryInfo objsDirectory = Directory.CreateDirectory(Path.Combine(fullName, "objs"));
            if (classes != null && classes.Length > 0)
            {
                GenerateClasses(objsDirectory.FullName, classes, sideBar, docInfo);
            }
        }

        private string GenerateHomePage(string outputPath, string sidebar, DocumentInformation docInfo)
        {
            string filePath = Path.Combine(outputPath, "index.html");

            File.Create(filePath).Close();
            StreamWriter streamWriter = new StreamWriter(filePath, false);

            string output = @$"<!DOCTYPE html>
                <html lang=""en"">

                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>{docInfo.ProjectName} Documentation</title>
                    <link rel=""stylesheet"" href=""./homePageStyles.css"">
                    <link rel=""stylesheet"" href=""./sidebar.css"">
                    <script src=""./helper.js"" defer></script>
                </head>

                <body> 
                

                    <aside class=""sidebar"">
                <a href=""./"">
                    <h2>{docInfo.ProjectName}</h2>
                </a>

                {sidebar}
            </aside> 

                <!-- Main Content -->
                    <div class=""content"">
                        <h1>{docInfo.ProjectName}</h1>
                        <p>{docInfo.ProjectDescription}</p>
                    </div>
                </body>

                </html>";


            streamWriter.Write(output);
            streamWriter.Close();
            streamWriter.Dispose();

            return filePath;
        }

        private void GenerateClasses(string outputPath,ClassDeclaration[] classes,string sidebar ,DocumentInformation docInfo)
        {
            foreach (ClassDeclaration current in classes)
            {
                StreamWriter writer = new StreamWriter(File.Create(Path.Combine(outputPath, $"{current.Name}.html")));
                string classOutput = GenerateClassPage(current, sidebar,docInfo);
                writer.Write(classOutput);
                writer.Close();
                writer.Dispose();
            }
        }

        private string GenerateClassPage(ClassDeclaration current, string sidebar, DocumentInformation docInfo)
        {
            string output = @$"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
    <title>Everybody Bleeds Documentation: {current.Name}</title>

    <link rel=""stylesheet"" href=""../docStyles.css"">
    <link rel=""stylesheet"" href=""../sidebar.css"">
    <script src=""../docsHelpers.js"" defer></script>
</head>

<body>
    <div class=""page"">
        <aside class=""sidebar"">
            <a href=""../"" class=""brand"">
                <h2>{docInfo.ProjectName}</h2>
            </a>

            {sidebar}
        </aside>
        

        <main class=""content"">
            <header class=""doc-header"">
                <h1>{current.Name} </h1>
                <p class=""lead"">{current.Definition}</p>
            </header>

            <section class=""diagrams"">
                <div class=""diagram-placeholder""><img src="""" alt=""Diagram""></div>
            </section>

            <section class=""members"">
                {GenerateVariables(current.Properties)}
                {GenerateMethods(current.Methods)}
            </section>

            <footer class=""doc-footer"">
                <small>Generated from: <code>Source</code></small>
            </footer>
        </main>
    </div>
</body>

</html>";


            return output;
        }

        private string GenerateMethods(Declaration[]? methods)
        {
            if (methods == null || methods.Length == 0) { return ""; }

            string output = $@"<div>
                    <h2>Properties</h2>

                    {GetAllMethods(methods)}
                </div>";

            return output;
        }

        private string GetAllMethods(Declaration[] methods)
        {
            string output = $@"";

            foreach (Declaration method in methods)
            {
                output += $@"<article class=""member"">
                        <button class=""member-toggle"" onclick=""toggleMember(this)"">◈ {method.Name}</button>
                        <div class=""member-body"">
                            <p><strong>Signature:</strong> <code>{method.Type} {method.Name}({GetMethodParameters(method.Parameters)})</code></p>
                            <p>{method.Definition}</p>
                        </div>
                    </article>
";
            }

            return output;
        }

        private string GetMethodParameters(string[]? parameters)
        {
            if(parameters == null || parameters.Length == 0) { return ""; }
            string output = "";

            for(int i = 0; i < parameters.Length; i++)
            {
                if(i == parameters.Length -1)
                {
                    output += parameters[i];
                }
                else
                {
                    output += parameters[i] + ", ";
                }
            }

            return output;
        }

        private string GenerateVariables(Declaration[]? properties)
        {
            if(properties == null || properties.Length == 0) { return ""; }

            string output = $@"<div>
                    <h2>Properties</h2>

                    {GetAllProperties(properties)}
                </div>";

            return output;
        }

        private string GetAllProperties(Declaration[] properties)
        {
            string output = $@"";

            foreach(Declaration property in properties)
            {
                output += $@"<article class=""member"">
                        <button class=""member-toggle"" onclick=""toggleMember(this)"">◈ {property.Name}</button>
                        <div class=""member-body"">
                            <p><strong>Signature:</strong> <code>{property.Type} {property.Name}</code></p>
                            <p>{property.Definition}</p>
                        </div>
                    </article>
";
            }

            return output;
        }

        private string GenerateSideBar(ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docinfo)
        {
            string output = "";
            output = @$"
                            <div class=""nav-section"">
                ";

            if (classes != null && classes.Length > 0)
            {
                output += WriteSidebarClasses(classes);
            }

            if (interfaces != null && interfaces.Length > 0)
            {
                output += WriteSidebarInterfaces(interfaces);
            }

            if (structs != null && structs.Length > 0)
            {
                output += WriteSidebarStructs(structs);
            }

            if (enums != null && enums.Length > 0)
            {
                output += WriteSidebarEnums(enums);
            }


            output += @"
                </div>";

            return output;
        }

        private string WriteSidebarStructs(StructDeclaration[] structs)
        {
            string output = @$"<button class=""toggle-btn"" onclick=""toggleMenu('structs')"">Structs <span class=""arrow"">▼</span></button>
            <div id=""structs"" class=""nav-links"">
                    ";


            foreach (StructDeclaration current in structs)
            {
                output += @$"<a href=""./objs/{current.Name}.html"">{current.Name}</a>";
            }


            output += @"       </div>
                ";

            return output;
        }

        private string WriteSidebarInterfaces(InterfaceDeclaration[] interfaces)
        {

            string output = @$"<button class=""toggle-btn"" onclick=""toggleMenu('interfaces')"">Interfaces <span class=""arrow"">▼</span></button>
            <div id=""interfaces"" class=""nav-links"">
                    ";


            foreach (InterfaceDeclaration current in interfaces)
            {
                output += @$"<a href=""./objs/{current.Name}.html"">{current.Name}</a>";
            }


            output += @"       </div>
                ";

            return output;
        }

        private string WriteSidebarEnums(EnumDeclaration[] enums)
        {
            string output = @$"<button class=""toggle-btn"" onclick=""toggleMenu('enums')"">Enums <span class=""arrow"">▼</span></button>
            <div id=""enums"" class=""nav-links"">
                    ";


            foreach (EnumDeclaration current in enums)
            {
                output += @$"<a href=""./objs/{current.Name}.html"">{current.Name}</a>";
            }


            output += @"       </div>
                ";

            return output;
        }

        private string WriteSidebarClasses(ClassDeclaration[] classes)
        {
            string output = @$"<button class=""toggle-btn"" onclick=""toggleMenu('classes')"">Classes <span class=""arrow"">▼</span></button>
            <div id=""classes"" class=""nav-links"">
                    ";


            foreach (ClassDeclaration current in classes)
            {
                output += @$"<a href=""./objs/{current.Name}.html"">{current.Name}</a>";
            }


            output += @"       </div>
                ";
            return output;
        }
    }
}
