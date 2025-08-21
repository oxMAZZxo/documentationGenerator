using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public System.Windows.Media.Color ClassDeclarationColour { get; set; }
        public System.Windows.Media.Color PrimitiveDeclarationColour { get; set; }
        public System.Windows.Media.Color EnumDeclarationColour { get; set; }
        public System.Windows.Media.Color InterfaceDeclarationColour { get; set; }
        public System.Windows.Media.Color StructDeclarationColour { get; set; }

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
            
        }
    }
}
