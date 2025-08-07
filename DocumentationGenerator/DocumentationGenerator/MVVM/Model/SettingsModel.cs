using MigraDoc.DocumentObjectModel;
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
        public Color ClassDeclarationColour { get; private set; }
        public Color PrimitiveDeclarationColour { get; private set; }
        public Color EnumDeclarationColour { get; private set; }
        public Color InterfaceDeclarationColour { get; private set; }
        public Color StructDeclarationColour { get; private set; }

        public SettingsModel()
        {
            if(Instance != null)
            {
                return;
            }

            if(Instance == null && Instance != this)
            {
                Instance = this;
            }
        }

        ~SettingsModel()
        {
            Dispose();
        }

        public void SetClassDeclarationColour(byte r, byte g, byte b)
        {
            ClassDeclarationColour = new Color(255,r, g, b);
        }

        public void SetPrimitiveDeclarationColour(byte r, byte g, byte b)
        {
            PrimitiveDeclarationColour = new Color(255, r, g, b);    
        }

        public void SetInterfaceDeclarationColour(byte r, byte g, byte b)
        {
            InterfaceDeclarationColour = new Color(255, r, g,b);
        }

        public void SetEnumDeclarationColour(byte r, byte g, byte b)
        {
            EnumDeclarationColour = new Color(255,r,g,b);
        }

        public void SetStructDeclarationColour(byte r, byte g, byte b)
        {
            StructDeclarationColour = new Color(255, r, g, b);
        }

        public void Dispose()
        {
            
        }
    }
}
