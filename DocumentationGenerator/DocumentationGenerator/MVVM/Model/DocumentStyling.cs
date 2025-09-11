using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DocumentationGenerator.MVVM.Model
{
    public class DocumentStyling
    {
        public DeclarationColours DeclarationColours { get; set; }
        public DeclarationFontStyles DeclarationFonts { get; set; }
        public bool GenerateTableOfContents { get; set; }
        public bool GeneratePageNumbers { get; set; }
        public bool GenerateRelationshipGraph { get; set; }
        public bool PrintBaseTypes { get; set; }

        public DocumentStyling(DeclarationColours declarationColours, DeclarationFontStyles declarationFonts, bool generateTableOfContents, bool generatePageNumbers, bool generateRelationshipGraph, bool printBaseTypes)
        {
            DeclarationColours = declarationColours;
            DeclarationFonts = declarationFonts;
            GenerateTableOfContents = generateTableOfContents;
            GeneratePageNumbers = generatePageNumbers;
            GenerateRelationshipGraph = generateRelationshipGraph;
            PrintBaseTypes = printBaseTypes;
        }
    }

    public class DeclarationFontStyles
    {
        public string FontFamilyName { get; }
        public FontDeclarationStyle ObjectDeclarationStyle { get; }
        public FontDeclarationStyle ObjectDefinitionStyle { get;}
        public FontDeclarationStyle MemberHeadingStyle { get;}
        public FontDeclarationStyle MemberStyle { get; }
        public FontDeclarationStyle MemberTypeStyle { get;}
        public FontDeclarationStyle MemberDefinitionStyle { get;}

        public DeclarationFontStyles(string fontFamilyName, FontDeclarationStyle objectDeclarationStyle, FontDeclarationStyle objectDefinitionStyle, FontDeclarationStyle memberHeadingStyle, FontDeclarationStyle memberTypeStyle, FontDeclarationStyle memberStyle ,FontDeclarationStyle memberDefinitionStyle)
        {
            FontFamilyName = fontFamilyName;
            ObjectDeclarationStyle = objectDeclarationStyle;
            ObjectDefinitionStyle = objectDefinitionStyle;
            MemberHeadingStyle = memberHeadingStyle;
            MemberStyle = memberStyle;
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
                OnPropertyChanged();
            }
        }

        public bool IsBold {
            get => isBold;
            set
            {
                isBold = value;

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

        public string GetValuesInStrings()
        {
            return $"{FontSize},{isBold},{IsItalic},{SpaceAfter}";
        }
    }
}
