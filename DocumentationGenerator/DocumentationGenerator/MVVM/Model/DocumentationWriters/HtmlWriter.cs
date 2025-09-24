using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            GenerateHomePage(outputPath.FullName, classes, enums, interfaces, structs, docInfo);
            GenerateObjPages(outputPath.FullName, classes, enums, interfaces, structs, docInfo);
            if (docInfo.GenerateRelationshipGraph)
            {
                // string graphPath = GenerateRelationshipGraph(classes, interfaces, documentStyling.DeclarationColours, path);
            }

            Directory.CreateDirectory(Path.Combine(outputPath.FullName, "objs"));

            return true;
        }

        private void GenerateObjPages(string fullName, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docInfo)
        {

        }

        private string GenerateHomePage(string outputPath, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs, DocumentInformation docInfo)
        {
            string filePath = Path.Combine(outputPath, "index.html");

            File.Create(filePath).Close();
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/helper.js"), Path.Combine(outputPath, "helper.js"), true);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "HTML DOC Templates/homePageStyles.css"), Path.Combine(outputPath, "homePageStyles.css"), true);
            StreamWriter streamWriter = new StreamWriter(filePath, false);


            streamWriter.Write(@$"<!DOCTYPE html>
                <html lang=""en"">

                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>{filePath}</title>
                    <link rel=""stylesheet"" href=""./homePageStyles.css"">
                    <script src=""./helper.js""></script>
                </head>
                <body>");

            GenerateSideBar(streamWriter, classes, enums, interfaces, structs);

            streamWriter.Write(@$"<div class=""content"">
                        <h1>{docInfo.ProjectName}</h1>
                        <p>{docInfo.ProjectDescription}</p>
                    </div>
                </body>
                ");

            streamWriter.Write("<html>");
            streamWriter.Close();
            streamWriter.Dispose();

            return filePath;
        }

        private void GenerateSideBar(StreamWriter streamWriter, ClassDeclaration[]? classes, EnumDeclaration[]? enums, InterfaceDeclaration[]? interfaces, StructDeclaration[]? structs)
        {
            streamWriter.Write(@"
            <div class=""sidebar"">
                <a href=""./""><h2>My Project</h2></a>
                    <div class=""nav-section"">
                ");

            if (classes != null && classes.Length > 0)
            {
                WriteSiderbarClasses(streamWriter, classes);
            }

            if (interfaces != null && interfaces.Length > 0)
            {
                WriteSiderbarInterfaces(streamWriter, interfaces);
            }

            if (structs != null && structs.Length > 0)
            {
                WriteSiderbarStructs(streamWriter, structs);
            }

            if (enums != null && enums.Length > 0)
            {
                WriteSiderbarEnums(streamWriter, enums);
            }


            streamWriter.Write(@"
                </div>
            </div>");


        }

        private void WriteSiderbarStructs(StreamWriter writer, StructDeclaration[] structs)
        {
            writer.Write(@$"<button onclick=""toggleMenu('structs')"">Structs ▼</button>
                    <div id=""structs"" class=""nav-links"">
                    ");


            foreach (StructDeclaration current in structs)
            {
                writer.Write(@$"<a href=""./objs/{current.Name}.html"">{current.Name}</a>");
            }


            writer.Write(@"       </div>
                ");
        }

        private void WriteSiderbarInterfaces(StreamWriter writer, InterfaceDeclaration[] interfaces)
        {
            writer.Write(@$"<button onclick=""toggleMenu('interaces')"">Interfaces ▼</button>
                    <div id=""interaces"" class=""nav-links"">
                    ");


            foreach (InterfaceDeclaration current in interfaces)
            {
                writer.Write(@$"<a href=""./objs/{current.Name}.html"">{current.Name}</a>");
            }


            writer.Write(@"       </div>
                ");
        }

        private void WriteSiderbarEnums(StreamWriter writer, EnumDeclaration[] enums)
        {
            writer.Write(@$"<button onclick=""toggleMenu('enums')"">Enums ▼</button>
                    <div id=""enums"" class=""nav-links"">
                    ");


            foreach (EnumDeclaration current in enums)
            {
                writer.Write(@$"<a href=""./objs/{current.Name}.html"">{current.Name}</a>");
            }


            writer.Write(@"       </div>
                ");
        }

        private void WriteSiderbarClasses(StreamWriter writer, ClassDeclaration[] classes)
        {
            writer.Write(@$"<button onclick=""toggleMenu('classes')"">Classes ▼</button>
                    <div id=""classes"" class=""nav-links"">
                    ");


            foreach (ClassDeclaration current in classes)
            {
                writer.Write(@$"<a href=""./objs/{current.Name}.html"">{current.Name}</a>");
            }


            writer.Write(@"       </div>
                ");
        }
    }
}
