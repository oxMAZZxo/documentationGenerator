using DocumentationGenerator.MVVM.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents.Serialization;
using System.Windows.Media;

namespace DocumentationGenerator.MVVM.Model
{
    public class SettingsModel : IDisposable
    {
        public static SettingsModel? Instance { get; private set; }
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
        public  FontDeclarationStyle MemberTypeStyle { get; set; }
        public FontDeclarationStyle MemberDefinitionStyle { get; set; }
        public string SelectedFont { get; set; }
        public bool GenerateTableOfContents { get; set; }
        public bool GeneratePageNumbers { get; set; }

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

            LoadSettings();
        }

        private void LoadSettings()
        {
            string[] lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "settings.txt"));

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
        }


        public void SaveSettings()
        {
            StreamWriter writer = new StreamWriter(Path.Combine(AppContext.BaseDirectory, "settings.txt"),false);

            writer.WriteLine("Colours (C,I,S,E,P):");
            writer.WriteLine(ClassDeclarationColour.ToString().Replace("#",""));
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

            writer.Close();
            writer.Dispose();
        }

        ~SettingsModel()
        {
            Dispose();
        }

        public void SetMigraDocClassDeclarationColour(byte r, byte g, byte b)
        {
            MigraDocClassDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255,r, g, b);
        }

        public void SetMigraDocPrimitiveDeclarationColour(byte r, byte g, byte b)
        {
            MigraDocPrimitiveDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255, r, g, b);    
        }

        public void SetMigraDocInterfaceDeclarationColour(byte r, byte g, byte b)
        {
            MigraDocInterfaceDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255, r, g,b);
        }

        public void SetMigraDocEnumDeclarationColour(byte r, byte g, byte b)
        {
            MigraDocEnumDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255,r,g,b);
        }

        public void SetMigraDocStructDeclarationColour(byte r, byte g, byte b)
        {
            MigraDocStructDeclarationColour = new MigraDoc.DocumentObjectModel.Color(255, r, g, b);
        }

        public void Dispose()
        {
            Instance = null;
        }
    }
}
