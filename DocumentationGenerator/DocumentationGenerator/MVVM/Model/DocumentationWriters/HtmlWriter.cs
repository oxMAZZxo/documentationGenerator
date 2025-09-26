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
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/helper.js"), Path.Combine(outputPath.FullName, "docHelpers.js"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/homePageStyles.css"), Path.Combine(outputPath.FullName, "homePageStyles.css"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/homePageStyles.css"), Path.Combine(outputPath.FullName, "sidebar.css"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/homePageStyles.css"), Path.Combine(outputPath.FullName, "docStyles.css"), true);
            GenerateHomePage(outputPath.FullName, classes, enums, interfaces, structs, docInfo);
            GenerateObjPages(outputPath.FullName, classes, enums, interfaces, structs, docInfo);

            if (docInfo.GenerateRelationshipGraph)
            {
                // string graphPath = GenerateRelationshipGraph(classes, interfaces, documentStyling.DeclarationColours, path);
            }
            return true;
        }

        private void GenerateObjPages(string fullName, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docInfo)
        {
            DirectoryInfo objsDirectory = Directory.CreateDirectory(Path.Combine(fullName, "objs"));
            if (classes != null && classes.Length > 0)
            {
                //GenerateClasses(objsDirectory.FullName, classes, docInfo);
            }
        }

        private string GenerateHomePage(string outputPath, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docInfo)
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
                
                {GenerateSideBar(classes, enums, interfaces, structs,docInfo)} 

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

        private void GenerateClasses(string outputPath,ClassDeclaration[] classes, DocumentInformation docInfo)
        {
            foreach (ClassDeclaration current in classes)
            {
                string classOutput = "";
                StreamWriter streamWriter = new StreamWriter(File.Create(Path.Combine(outputPath, $"{current.Name}.html")));
                classOutput = GenerateClassTopBody(current);
            }
        }

        private string GenerateClassTopBody(ClassDeclaration current)
        {
            //string output = @$"";
            throw new NotImplementedException();
        }

        private string GenerateSideBar(ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docinfo)
        {
            string output = "";
            output = @$"<aside class=""sidebar"">
                            <a href=""./"">
                                <h2>{docinfo.ProjectName}</h2>
                            </a>
                            <div class=""nav-section"">
                ";

            if (classes != null && classes.Length > 0)
            {
                output += WriteSiderbarClasses(classes);
            }

            if (interfaces != null && interfaces.Length > 0)
            {
                output += WriteSiderbarInterfaces(interfaces);
            }

            if (structs != null && structs.Length > 0)
            {
                output += WriteSiderbarStructs(structs);
            }

            if (enums != null && enums.Length > 0)
            {
                output += WriteSiderbarEnums(enums);
            }


            output += @"
                </div>
            </aside>";

            return output;
        }

        private string WriteSiderbarStructs(StructDeclaration[] structs)
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

        private string WriteSiderbarInterfaces(InterfaceDeclaration[] interfaces)
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

        private string WriteSiderbarEnums(EnumDeclaration[] enums)
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

        private string WriteSiderbarClasses(ClassDeclaration[] classes)
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
