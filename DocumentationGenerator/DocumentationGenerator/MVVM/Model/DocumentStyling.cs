using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationGenerator.MVVM.Model
{
    public class DocumentStyling
    {
        public DeclarationColours DeclarationColours { get; set; }
        public DeclarationFontStyles DeclarationFonts { get; set; }
        public bool GenerateTableOfContents { get; set; }
        public bool GeneratePageNumbers { get; set; }

        public DocumentStyling(DeclarationColours declarationColours, DeclarationFontStyles declarationFonts, bool generateTableOfContents, bool generatePageNumbers)
        {
            DeclarationColours = declarationColours;
            DeclarationFonts = declarationFonts;
            GenerateTableOfContents = generateTableOfContents;
            GeneratePageNumbers = generatePageNumbers;
        }
    }

    public class DeclarationFontStyles
    {
        public string FontFamilyName { get; set; }
        public FontDeclarationStyle ObjectDeclarationStyle { get; set; }
        public FontDeclarationStyle ObjectDefinitionStyle { get; set; }
        public FontDeclarationStyle MemberHeadingStyle { get; set; }
        public FontDeclarationStyle MemberTypeStyle { get; set; }
        public FontDeclarationStyle MemberDefinitionStyle { get; set; }

        public DeclarationFontStyles(string fontFamilyName, FontDeclarationStyle objectDeclarationStyle, FontDeclarationStyle objectDefinitionStyle, FontDeclarationStyle memberHeadingStyle, FontDeclarationStyle memberTypeStyle, FontDeclarationStyle memberDefinitionStyle)
        {
            FontFamilyName = fontFamilyName;
            ObjectDeclarationStyle = objectDeclarationStyle;
            ObjectDefinitionStyle = objectDefinitionStyle;
            MemberHeadingStyle = memberHeadingStyle;
            MemberTypeStyle = memberTypeStyle;
            MemberDefinitionStyle = memberDefinitionStyle;
        }
    }

    public class FontDeclarationStyle : INotifyPropertyChanged
    {
        private string fontSizeString = "";
        private bool isItalic;
        private bool isBold;
        private string spaceAfterString = "";

        public string FontSizeString { 
            get => fontSizeString; 
            set 
            {
                fontSizeString = value;
                SetFontSize();
                OnPropertyChanged();
            } 
        }

        public int FontSize { get; private set; }

        public bool IsItalic { 
            get => isItalic; 
            set
            {
                isItalic = value;
                Debug.WriteLine("IsItalic changed");
                OnPropertyChanged();
            }
        }

        public bool IsBold {
            get => isBold;
            set
            {
                isBold = value;

                Debug.WriteLine("IsBold changed");
                OnPropertyChanged();
            }
        }

        public string SpaceAfterString
        {
            get => spaceAfterString;
            set
            {
                spaceAfterString = value;
                SetSpaceAfter();
                OnPropertyChanged();
            }
        }

        public int SpaceAfter { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public FontDeclarationStyle(string fontSizeString, bool isItalic, bool isBold, string spaceAfterString)
        {
            FontSizeString = fontSizeString;
            SpaceAfterString = spaceAfterString;
            IsItalic = isItalic;
            IsBold = isBold;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void SetSpaceAfter()
        {
            SpaceAfter = Convert.ToInt32(SpaceAfterString);
        }

        private void SetFontSize()
        {
            FontSize = Convert.ToInt32(FontSizeString);
        }
    }
}
