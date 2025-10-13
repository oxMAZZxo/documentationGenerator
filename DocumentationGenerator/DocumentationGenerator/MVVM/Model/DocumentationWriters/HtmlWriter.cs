﻿using DocumentationGenerator.MVVM.Model.Declarations;
using DocumentationGenerator.MVVM.Model.DocumentInfo;
using System.Drawing;
using System.IO;


namespace DocumentationGenerator.MVVM.Model.DocumentationWriters
{
    public class HtmlWriter
    {
        public bool Write(ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docInfo)
        {
            if (string.IsNullOrEmpty(docInfo.SavePath) || string.IsNullOrWhiteSpace(docInfo.SavePath)) { return false; }

            DirectoryInfo outputPath = Directory.CreateDirectory(Path.Combine(docInfo.SavePath, $"{docInfo.ProjectName} Documentation"));

            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/helper.js"), Path.Combine(outputPath.FullName, "helper.js"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/docsHelpers.js"), Path.Combine(outputPath.FullName, "docsHelpers.js"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/homePageStyles.css"), Path.Combine(outputPath.FullName, "homePageStyles.css"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/sidebar.css"), Path.Combine(outputPath.FullName, "sidebar.css"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/docStyles.css"), Path.Combine(outputPath.FullName, "docStyles.css"), true);

            if (docInfo.GenerateInheritanceGraphs && docInfo.GlobalInheritanceGraph != null && docInfo.IndividualObjsGraphs != null)
            {
                SaveBitmaps(docInfo.GlobalInheritanceGraph, docInfo.IndividualObjsGraphs, outputPath);
            }

            string homepageSideBar = GenerateSideBar(classes, enums, interfaces, structs, docInfo);
            GenerateHomePage(outputPath.FullName, homepageSideBar, docInfo);

            string objSideBar = GenerateSideBarForObjs(classes, enums, interfaces, structs, docInfo);
            GenerateObjPages(outputPath.FullName, classes, enums, interfaces, structs, objSideBar, docInfo);
            return true;
        }

        private void SaveBitmaps(Bitmap globalInheritanceGraph, Dictionary<string, Bitmap> individualGraphs, DirectoryInfo outputPath)
        {
            globalInheritanceGraph.Save(Path.Combine(outputPath.FullName, "globalInheritanceGraph.png"));

            DirectoryInfo objsDirectory = Directory.CreateDirectory(Path.Combine(outputPath.FullName, "objGraphs"));

            foreach(string o in individualGraphs.Keys)
            {
                Bitmap bitmap = individualGraphs[o];
                bitmap.Save(Path.Combine(objsDirectory.FullName, $"{o}_Graph.png"));
            }
        }

        private void GenerateObjPages(string fullName, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, string sideBar, DocumentInformation docInfo)
        {
            DirectoryInfo objsDirectory = Directory.CreateDirectory(Path.Combine(fullName, "objs"));
            if (classes != null && classes.Length > 0)
            {
                foreach (ClassDeclaration current in classes)
                {
                    StreamWriter writer = new StreamWriter(File.Create(Path.Combine(objsDirectory.FullName, $"{current.Name}.html")));
                    string classOutput = GeneratePage(current.Name, current.Definition, sideBar, docInfo, current.Properties, current.Methods);
                    writer.Write(classOutput);
                    writer.Close();
                    writer.Dispose();
                }
            }

            if (interfaces != null && interfaces.Length > 0)
            {
                foreach (InterfaceDeclaration current in interfaces)
                {
                    StreamWriter writer = new StreamWriter(File.Create(Path.Combine(objsDirectory.FullName, $"{current.Name}.html")));
                    string classOutput = GeneratePage(current.Name, current.Definition, sideBar, docInfo, current.Properties, current.Methods);
                    writer.Write(classOutput);
                    writer.Close();
                    writer.Dispose();
                }
            }

            if (structs != null && structs.Length > 0)
            {
                foreach (StructDeclaration current in structs)
                {
                    StreamWriter writer = new StreamWriter(File.Create(Path.Combine(objsDirectory.FullName, $"{current.Name}.html")));
                    string classOutput = GeneratePage(current.Name, current.Definition, sideBar, docInfo, current.Properties, current.Methods);
                    writer.Write(classOutput);
                    writer.Close();
                    writer.Dispose();
                }
            }

            if (enums != null && enums.Length > 0)
            {
                foreach (EnumDeclaration current in enums)
                {
                    StreamWriter writer = new StreamWriter(File.Create(Path.Combine(objsDirectory.FullName, $"{current.Name}.html")));
                    string classOutput = GeneratePage(current.Name, current.Definition, sideBar, docInfo, null, null, current.EnumMembers);
                    writer.Write(classOutput);
                    writer.Close();
                    writer.Dispose();
                }
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
                            {GetGlobalRelationshipGraph(docInfo.GenerateInheritanceGraphs)}
                        </div>
                </body>

                </html>";


            streamWriter.Write(output);
            streamWriter.Close();
            streamWriter.Dispose();

            return filePath;
        }

        private string GetGlobalRelationshipGraph(bool generateHomePageGraph)
        {
            if (!generateHomePageGraph) { return ""; }
            return @$"<img src=""./globalInheritanceGraph.png"" alt=""Global Relationship Graph"">";
        }

        private string GeneratePage(string objectName, string? objectDefinition, string sidebar, DocumentInformation docInfo, Declaration[]? properties = null, Declaration[]? methods = null, Declaration[]? enumMembers = null)
        {
            string output = @$"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width,initial-scale=1"" />
    <title>Everybody Bleeds Documentation: {objectName}</title>

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
                <h1>{objectName} </h1>
                <p class=""lead"">{objectDefinition}</p>
                
            </header>

            {GetObjDiagram(objectName, docInfo)}

            <section class=""members"">
                {GenerateProperties(properties)}
                {GenerateMethods(methods)}
                {GenerateEnumMembers(enumMembers)}
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

        private object GetObjDiagram(string objectName,DocumentInformation docInfo)
        {
            if (docInfo.IndividualObjsGraphs != null && !docInfo.IndividualObjsGraphs.ContainsKey(objectName)) { return ""; }
            return @$"            <section class=""diagrams"">
                <div class=""diagram-placeholder""><img src=""../objGraphs/{objectName}_Graph.png"" alt=""Graph""></div>
            </section>";
        }

        private string GenerateEnumMembers(Declaration[]? enumMembers)
        {
            if (enumMembers == null || enumMembers.Length == 0) { return ""; }

            string output = $@"<div>
                    <h2>Enum Members</h2>

                    {GetAllEnumMembers(enumMembers)}
                </div>";

            return output;
        }

        private string GetAllEnumMembers(Declaration[] enumMembers)
        {
            string output = $@"";

            foreach (Declaration member in enumMembers)
            {
                output += $@"<article class=""member"">
                        <button class=""member-toggle"" onclick=""toggleMember(this)"">◈ {member.Name}</button>
                        <div class=""member-body"">
                            <p><strong>Signature:</strong> <code>{member.Name}</code></p>
                            <p>{member.Definition}</p>
                        </div>
                    </article>
";
            }

            return output;
        }

        private string GenerateMethods(Declaration[]? methods)
        {
            if (methods == null || methods.Length == 0) { return ""; }

            string output = $@"<div>
                    <h2>Methods</h2>

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
            if (parameters == null || parameters.Length == 0) { return ""; }
            string output = "";

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i == parameters.Length - 1)
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

        private string GenerateProperties(Declaration[]? properties)
        {
            if (properties == null || properties.Length == 0) { return ""; }

            string output = $@"<div>
                    <h2>Properties</h2>

                    {GetAllProperties(properties)}
                </div>";

            return output;
        }

        private string GetAllProperties(Declaration[] properties)
        {
            string output = $@"";

            foreach (Declaration property in properties)
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


        #region Sidebar For Home Page
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
        #endregion


        #region Sidebar For Object Pages
        private string GenerateSideBarForObjs(ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docinfo)
        {
            string output = "";
            output = @$"
                            <div class=""nav-section"">
                ";

            if (classes != null && classes.Length > 0)
            {
                output += WriteSidebarClassesForObjs(classes);
            }

            if (interfaces != null && interfaces.Length > 0)
            {
                output += WriteSidebarInterfacesForObjs(interfaces);
            }

            if (structs != null && structs.Length > 0)
            {
                output += WriteSidebarStructsForObjs(structs);
            }

            if (enums != null && enums.Length > 0)
            {
                output += WriteSidebarEnumsForObjs(enums);
            }


            output += @"
                </div>";

            return output;
        }

        private string WriteSidebarStructsForObjs(StructDeclaration[] structs)
        {
            string output = @$"<button class=""toggle-btn"" onclick=""toggleMenu('structs')"">Structs <span class=""arrow"">▼</span></button>
            <div id=""structs"" class=""nav-links"">
                    ";


            foreach (StructDeclaration current in structs)
            {
                output += @$"<a href=""./{current.Name}.html"">{current.Name}</a>";
            }


            output += @"       </div>
                ";

            return output;
        }

        private string WriteSidebarInterfacesForObjs(InterfaceDeclaration[] interfaces)
        {

            string output = @$"<button class=""toggle-btn"" onclick=""toggleMenu('interfaces')"">Interfaces <span class=""arrow"">▼</span></button>
            <div id=""interfaces"" class=""nav-links"">
                    ";


            foreach (InterfaceDeclaration current in interfaces)
            {
                output += @$"<a href=""./{current.Name}.html"">{current.Name}</a>";
            }


            output += @"       </div>
                ";

            return output;
        }

        private string WriteSidebarEnumsForObjs(EnumDeclaration[] enums)
        {
            string output = @$"<button class=""toggle-btn"" onclick=""toggleMenu('enums')"">Enums <span class=""arrow"">▼</span></button>
            <div id=""enums"" class=""nav-links"">
                    ";


            foreach (EnumDeclaration current in enums)
            {
                output += @$"<a href=""./{current.Name}.html"">{current.Name}</a>";
            }


            output += @"       </div>
                ";

            return output;
        }

        private string WriteSidebarClassesForObjs(ClassDeclaration[] classes)
        {
            string output = @$"<button class=""toggle-btn"" onclick=""toggleMenu('classes')"">Classes <span class=""arrow"">▼</span></button>
            <div id=""classes"" class=""nav-links"">
                    ";


            foreach (ClassDeclaration current in classes)
            {
                output += @$"<a href=""./{current.Name}.html"">{current.Name}</a>";
            }


            output += @"       </div>
                ";
            return output;
        }
        #endregion
    }
}
