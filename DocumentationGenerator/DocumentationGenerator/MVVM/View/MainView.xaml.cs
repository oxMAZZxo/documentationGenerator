using DocumentationGenerator.MVVM.Model;
using DocumentationGenerator.MVVM.Model.Declarations;
using DocumentationGenerator.MVVM.Model.DocumentInformation;
using DocumentationGenerator.MVVM.ViewModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace DocumentationGenerator.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {

        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(this);
            SourceInitialized += MainView_SourceInitialized;
        }

        private void MainView_SourceInitialized(object? sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        }

        public void ChangeRichTextBoxFont()
        {
            if(SettingsModel.Instance == null) { return; }

            myRichTextBox.FontFamily = new FontFamily(SettingsModel.Instance.SelectedFont);
        }

        public void UpdateRichTextBox(ParsedSourceResults parsedSourceResults)
        {
            if (SettingsModel.Instance == null) { MessageBox.Show("Cannot Display Preview since the settings have not been initialised."); return; }

            DeclarationColours declarationColours = new DeclarationColours(SettingsModel.Instance.MigraDocClassDeclarationColour,
                    SettingsModel.Instance.MigraDocEnumDeclarationColour, SettingsModel.Instance.MigraDocPrimitiveDeclarationColour,
                    SettingsModel.Instance.MigraDocInterfaceDeclarationColour, SettingsModel.Instance.MigraDocStructDeclarationColour);

            myRichTextBox.Document.Blocks.Clear();

            WriteClassesPreview(parsedSourceResults.Classes, declarationColours);
            WriteInterfacesPreview(parsedSourceResults.Interfaces, declarationColours);
            WriteEnumsPreview(parsedSourceResults.Enums, declarationColours);
            WriteStructsPreview(parsedSourceResults.Structs, declarationColours);
        }

        public void ShowDefaultPreviewMessage()
        {
            if (SettingsModel.Instance == null)
            {
                System.Windows.MessageBox.Show($"Settings have not been initialised. The preview will not been shown.");
                return;
            }

            myRichTextBox.Document.Blocks.Clear();
            ChangeRichTextBoxFont();
            // Fake "Class" heading
            var classP = new Paragraph();
            classP.Inlines.Add(MakeRun("Class ", SettingsModel.Instance.ObjectDeclarationStyle.FontSize, SettingsModel.Instance.ObjectDeclarationStyle.IsBold, 
                Color.FromRgb(0,0,0), SettingsModel.Instance.ObjectDeclarationStyle.IsItalic));
            classP.Inlines.Add(MakeRun("ExampleClass", SettingsModel.Instance.ObjectDeclarationStyle.FontSize, 
                SettingsModel.Instance.ObjectDeclarationStyle.IsBold, SettingsModel.Instance.ClassDeclarationColour, 
                SettingsModel.Instance.ObjectDeclarationStyle.IsItalic));

            myRichTextBox.Document.Blocks.Add(classP);

            // Fake definition
            var defP = new Paragraph();
            AddDefinition(defP, "This is how the an Objects definition will be displayed.", SettingsModel.Instance.ObjectDefinitionStyle);
            myRichTextBox.Document.Blocks.Add(defP);

            // Fake property
            var propP = new Paragraph();
            propP.Inlines.Add(MakeRun("Property: ", SettingsModel.Instance.MemberHeadingStyle.FontSize, SettingsModel.Instance.MemberHeadingStyle.IsBold,
                Color.FromRgb(0,0,0), SettingsModel.Instance.MemberHeadingStyle.IsItalic));
            propP.Inlines.Add(MakeRun("string ", SettingsModel.Instance.MemberTypeStyle.FontSize, SettingsModel.Instance.MemberTypeStyle.IsBold, 
                SettingsModel.Instance.ClassDeclarationColour, SettingsModel.Instance.MemberTypeStyle.IsItalic));
            propP.Inlines.Add(MakeRun("ExampleProperty - ", SettingsModel.Instance.MemberStyle.FontSize, SettingsModel.Instance.MemberStyle.IsBold, 
                Color.FromRgb(1,1,1), SettingsModel.Instance.MemberStyle.IsItalic));
            propP.Inlines.Add(MakeRun("A sample property.", SettingsModel.Instance.MemberDefinitionStyle.FontSize, SettingsModel.Instance.MemberDefinitionStyle.IsBold, 
                Color.FromRgb(1,1,1), SettingsModel.Instance.MemberDefinitionStyle.IsItalic));
            myRichTextBox.Document.Blocks.Add(propP);

            // Fake field
            var fieldP = new Paragraph();
            fieldP.Inlines.Add(MakeRun("Field: ", SettingsModel.Instance.MemberHeadingStyle.FontSize, SettingsModel.Instance.MemberHeadingStyle.IsBold,
                Color.FromRgb(0, 0, 0), SettingsModel.Instance.MemberHeadingStyle.IsItalic));
            fieldP.Inlines.Add(MakeRun("int ", SettingsModel.Instance.MemberTypeStyle.FontSize, SettingsModel.Instance.MemberTypeStyle.IsBold,
                SettingsModel.Instance.PrimitiveDeclarationColour, SettingsModel.Instance.MemberTypeStyle.IsItalic));
            fieldP.Inlines.Add(MakeRun("_exampleField - ", SettingsModel.Instance.MemberStyle.FontSize, SettingsModel.Instance.MemberStyle.IsBold,
                Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberStyle.IsItalic));
            fieldP.Inlines.Add(MakeRun("A sample field.", SettingsModel.Instance.MemberDefinitionStyle.FontSize, SettingsModel.Instance.MemberDefinitionStyle.IsBold,
                Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberDefinitionStyle.IsItalic));
            myRichTextBox.Document.Blocks.Add(fieldP);

            // Fake method
            var methodP = new Paragraph();
            methodP.Inlines.Add(MakeRun("Method: ", SettingsModel.Instance.MemberHeadingStyle.FontSize, SettingsModel.Instance.MemberHeadingStyle.IsBold,
                Color.FromRgb(0, 0, 0), SettingsModel.Instance.MemberHeadingStyle.IsItalic));
            methodP.Inlines.Add(MakeRun("void ", SettingsModel.Instance.MemberTypeStyle.FontSize, SettingsModel.Instance.MemberTypeStyle.IsBold,
                SettingsModel.Instance.PrimitiveDeclarationColour, SettingsModel.Instance.MemberTypeStyle.IsItalic));
            methodP.Inlines.Add(MakeRun("ExampleMethod - ", SettingsModel.Instance.MemberStyle.FontSize, SettingsModel.Instance.MemberStyle.IsBold,
                Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberStyle.IsItalic));
            methodP.Inlines.Add(MakeRun("A sample method.", SettingsModel.Instance.MemberDefinitionStyle.FontSize, SettingsModel.Instance.MemberDefinitionStyle.IsBold,
                Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberDefinitionStyle.IsItalic));
            methodP.Inlines.Add(MakeRun(" - Does nothing.", SettingsModel.Instance.MemberDefinitionStyle.FontSize, SettingsModel.Instance.MemberDefinitionStyle.IsBold,
                Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberDefinitionStyle.IsItalic));
            myRichTextBox.Document.Blocks.Add(methodP);
        }

        /// <summary>
        /// Helper local function for consistent Run creation.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="bold">Indicates whether the text is bold.</param>
        /// <param name="colour">The colour of the text.</param>
        /// <param name="italic">Indicates whether the text is italic.</param>
        /// <returns></returns>
        private Run MakeRun(string text, double size, bool bold, Color? colour = null, bool italic = false)
        {
            var run = new Run(text)
            {
                FontSize = size,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                FontStyle = italic ? FontStyles.Italic : FontStyles.Normal
            };
            if (colour.HasValue)
            {
                var brush = new SolidColorBrush(colour.Value);
                brush.Freeze();
                run.Foreground = brush;
            }
            return run;
        }

        private void AddDefinition(Paragraph p, string def, FontDeclarationStyle style)
        {
            p.Inlines.Add(MakeRun($"Definition: {def}", style.FontSize, style.IsBold, Color.FromRgb(0,0,0), style.IsItalic));
        }

        private void WriteClassesPreview(List<ClassDeclaration> classes, DeclarationColours colours)
        {
            if(SettingsModel.Instance == null) { return; }

            foreach (ClassDeclaration currentClass in classes)
            {
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Class ", SettingsModel.Instance.ObjectDeclarationStyle.FontSize, SettingsModel.Instance.ObjectDeclarationStyle.IsBold,
                Color.FromRgb(0, 0, 0), SettingsModel.Instance.ObjectDeclarationStyle.IsItalic));
                p.Inlines.Add(MakeRun(currentClass.Name, SettingsModel.Instance.ObjectDeclarationStyle.FontSize,
                SettingsModel.Instance.ObjectDeclarationStyle.IsBold, SettingsModel.Instance.ClassDeclarationColour,
                SettingsModel.Instance.ObjectDeclarationStyle.IsItalic));
                if (currentClass.BaseTypes != null && currentClass.BaseTypes.Length > 0)
                {
                    WriteClassInheritancesAndInterfaces(currentClass, p, colours);
                }
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, currentClass.Definition ?? "NO SUMMARY", SettingsModel.Instance.ObjectDefinitionStyle);
                myRichTextBox.Document.Blocks.Add(defP);

                WriteProperties(currentClass.Properties, colours);

                WriteFields(currentClass.Fields, colours);

                WriteMethods(currentClass.Methods, colours);
            }
        }

        private void WriteClassInheritancesAndInterfaces(ClassDeclaration currentClass, Paragraph paragraph, DeclarationColours colours)
        {
            if (currentClass.BaseTypes[0][0] == 'I')
            {
                WriteInterfaceImplementations(false, currentClass, paragraph, colours);
                return;
            }
            else
            {
                paragraph.Inlines.Add(MakeRun("\nInherits ", SettingsModel.Instance.ObjectDefinitionStyle.FontSize,
                    SettingsModel.Instance.ObjectDefinitionStyle.IsBold, Color.FromRgb(0,0,0),
                    SettingsModel.Instance.ObjectDefinitionStyle.IsItalic));
                paragraph.Inlines.Add(MakeRun(currentClass.BaseTypes[0], SettingsModel.Instance.ObjectDeclarationStyle.FontSize,
                SettingsModel.Instance.ObjectDeclarationStyle.IsBold, SettingsModel.Instance.ClassDeclarationColour,
                SettingsModel.Instance.ObjectDeclarationStyle.IsItalic));
            }

            if (currentClass.BaseTypes.Length > 1)
            {
                WriteInterfaceImplementations(true, currentClass, paragraph, colours);
            }
        }

        private void WriteInterfaceImplementations(bool skipFirst, ClassDeclaration currentClass, Paragraph paragraph, DeclarationColours colours)
        {
            string interfaces = "";
            if (!skipFirst)
            {
                interfaces = $"{currentClass.BaseTypes[0]}";
                interfaces += ", ";
            }
            if (currentClass.BaseTypes.Length > 1)
            {
                for (int i = 1; i < currentClass.BaseTypes.Length; i++)
                {
                    if (i == currentClass.BaseTypes.Length - 1)
                    {
                        interfaces += $"{currentClass.BaseTypes[i]}";
                    }
                    else
                    {
                        interfaces += $"{currentClass.BaseTypes[i]}, ";
                    }
                }
            }

            paragraph.Inlines.Add(MakeRun("\nImplements ", SettingsModel.Instance.ObjectDefinitionStyle.FontSize,
                    SettingsModel.Instance.ObjectDefinitionStyle.IsBold, Color.FromRgb(0, 0, 0),
                    SettingsModel.Instance.ObjectDefinitionStyle.IsItalic));
            paragraph.Inlines.Add(MakeRun(interfaces, SettingsModel.Instance.ObjectDeclarationStyle.FontSize,
            SettingsModel.Instance.ObjectDeclarationStyle.IsBold, SettingsModel.Instance.InterfaceDeclarationColour,
            SettingsModel.Instance.ObjectDeclarationStyle.IsItalic));
        }

        private void WriteInterfacesPreview(List<InterfaceDeclaration> interfaces, DeclarationColours colours)
        {
            if(SettingsModel.Instance == null) { return; }

            foreach (InterfaceDeclaration currentInterface in interfaces)
            {
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Interface ", 20, true));
                p.Inlines.Add(MakeRun(currentInterface.Name, 20, true, SettingsModel.Instance?.InterfaceDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, currentInterface.Definition ?? "NO SUMMARY", SettingsModel.Instance.ObjectDefinitionStyle);
                myRichTextBox.Document.Blocks.Add(defP);

                WriteProperties(currentInterface.Properties, colours);
                WriteMethods(currentInterface.Methods, colours);

            }
        }

        private void WriteEnumsPreview(List<EnumDeclaration> enums, DeclarationColours colours)
        {
            foreach (EnumDeclaration currentEnum in enums)
            {
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Enum ", 20, true));
                p.Inlines.Add(MakeRun(currentEnum.Name, 20, true, SettingsModel.Instance?.EnumDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, currentEnum.Definition ?? "NO SUMMARY", SettingsModel.Instance.ObjectDefinitionStyle);
                myRichTextBox.Document.Blocks.Add(defP);

                WriteEnumMembers(currentEnum.EnumMembers);
            }
        }

        private void WriteStructsPreview(List<StructDeclaration> structs, DeclarationColours colours)
        {
            foreach (StructDeclaration currentStruct in structs)
            {
                var p = new Paragraph();
                p.Inlines.Add(MakeRun("Struct ", 20, true));
                p.Inlines.Add(MakeRun(currentStruct.Name, 20, true, SettingsModel.Instance?.StructDeclarationColour));
                myRichTextBox.Document.Blocks.Add(p);

                var defP = new Paragraph();
                AddDefinition(defP, currentStruct.Definition ?? "NO SUMMARY", SettingsModel.Instance.ObjectDefinitionStyle);
                myRichTextBox.Document.Blocks.Add(defP);

                WriteProperties(currentStruct.Properties, colours);
                WriteFields(currentStruct.Fields, colours);
                WriteMethods(currentStruct.Methods, colours);
            }
        }

        /// <summary>
        /// Helper to pick colour based on ObjectType
        /// </summary>
        /// <param name="decl"></param>
        /// <param name="colours"></param>
        /// <returns></returns>
        private Color? GetTypeColour(Declaration decl, DeclarationColours colours)
        {
            if (SettingsModel.Instance == null)
            {
                return null;
            }

            Color col = SettingsModel.Instance.PrimitiveDeclarationColour;

            if (decl.IsTypePrimitive.HasValue && decl.IsTypePrimitive.Value == false)
            {
                switch (decl.WhatIsType)
                {
                    case ObjectType.Class: col = SettingsModel.Instance.ClassDeclarationColour; break;
                    case ObjectType.Enum: col = SettingsModel.Instance.EnumDeclarationColour; break;
                    case ObjectType.Interface: col = SettingsModel.Instance.InterfaceDeclarationColour; break;
                    case ObjectType.Struct: col = SettingsModel.Instance.StructDeclarationColour; break;
                }
            }
            
            return col;
        }

        private void WriteProperties(Declaration[]? properties, DeclarationColours colours)
        {
            if (properties == null || properties.Length == 0 || SettingsModel.Instance == null) { return; }
            var parag = new Paragraph();
            parag.Inlines.Add(MakeRun("Properties: ", SettingsModel.Instance.MemberHeadingStyle.FontSize, SettingsModel.Instance.MemberHeadingStyle.IsBold,
                Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberHeadingStyle.IsItalic));
            myRichTextBox.Document.Blocks.Add(parag);

            foreach (var prop in properties)
            {
                parag = new Paragraph();
                parag.Inlines.Add(MakeRun($"{prop.Type} ", SettingsModel.Instance.MemberTypeStyle.FontSize, SettingsModel.Instance.MemberTypeStyle.IsBold, 
                    GetTypeColour(prop, colours), SettingsModel.Instance.MemberTypeStyle.IsItalic));
                parag.Inlines.Add(MakeRun($"{prop.Name} - ", SettingsModel.Instance.MemberStyle.FontSize, SettingsModel.Instance.MemberStyle.IsBold,
                    Color.FromRgb(1,1,1), SettingsModel.Instance.MemberStyle.IsItalic));
                parag.Inlines.Add(MakeRun(prop.Definition ?? "NO SUMMARY", SettingsModel.Instance.MemberDefinitionStyle.FontSize, SettingsModel.Instance.MemberDefinitionStyle.IsBold, 
                    Color.FromRgb(1,1,1), SettingsModel.Instance.MemberDefinitionStyle.IsItalic));
                myRichTextBox.Document.Blocks.Add(parag);
            }
        }

        private void WriteFields(Declaration[]? fields, DeclarationColours colours)
        {
            if (fields == null || fields.Length == 0 || SettingsModel.Instance == null) { return; }

            var fieldP = new Paragraph();
            fieldP.Inlines.Add(MakeRun("Fields: ", SettingsModel.Instance.MemberHeadingStyle.FontSize, SettingsModel.Instance.MemberHeadingStyle.IsBold,
                Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberHeadingStyle.IsItalic));
            myRichTextBox.Document.Blocks.Add(fieldP);

            foreach (var field in fields)
            {
                fieldP = new Paragraph();
                fieldP.Inlines.Add(MakeRun($"{field.Type} ", SettingsModel.Instance.MemberTypeStyle.FontSize, SettingsModel.Instance.MemberTypeStyle.IsBold,
                    GetTypeColour(field, colours), SettingsModel.Instance.MemberTypeStyle.IsItalic));
                fieldP.Inlines.Add(MakeRun($"{field.Name} - ", SettingsModel.Instance.MemberStyle.FontSize, SettingsModel.Instance.MemberStyle.IsBold,
                    Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberStyle.IsItalic));
                fieldP.Inlines.Add(MakeRun(field.Definition ?? "NO SUMMARY", SettingsModel.Instance.MemberDefinitionStyle.FontSize, SettingsModel.Instance.MemberDefinitionStyle.IsBold,
                    Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberDefinitionStyle.IsItalic));
                myRichTextBox.Document.Blocks.Add(fieldP);
            }
        }

        private void WriteMethods(Declaration[]? methods, DeclarationColours colours)
        {
            if (methods == null || methods.Length == 0 || SettingsModel.Instance == null) { return; }

            var methodP = new Paragraph();
            methodP.Inlines.Add(MakeRun("Methods: ", SettingsModel.Instance.MemberHeadingStyle.FontSize, SettingsModel.Instance.MemberHeadingStyle.IsBold,
                Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberHeadingStyle.IsItalic));
            myRichTextBox.Document.Blocks.Add(methodP);

            foreach (var m in methods)
            {
                methodP = new Paragraph();
                methodP.Inlines.Add(MakeRun($"{m.Type} ", SettingsModel.Instance.MemberTypeStyle.FontSize, SettingsModel.Instance.MemberTypeStyle.IsBold,
                    GetTypeColour(m, colours), SettingsModel.Instance.MemberTypeStyle.IsItalic));
                methodP.Inlines.Add(MakeRun($"{m.Name} - ", SettingsModel.Instance.MemberStyle.FontSize, SettingsModel.Instance.MemberStyle.IsBold,
                    Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberStyle.IsItalic));
                methodP.Inlines.Add(MakeRun(m.Definition ?? "NO SUMMARY", SettingsModel.Instance.MemberDefinitionStyle.FontSize, SettingsModel.Instance.MemberDefinitionStyle.IsBold,
                    Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberDefinitionStyle.IsItalic));
                if (!string.IsNullOrEmpty(m.ReturnDefinition))
                {
                    methodP.Inlines.Add(MakeRun($" - {m.ReturnDefinition}", SettingsModel.Instance.MemberDefinitionStyle.FontSize, SettingsModel.Instance.MemberDefinitionStyle.IsBold,
                    Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberDefinitionStyle.IsItalic));
                }
                myRichTextBox.Document.Blocks.Add(methodP);
            }
        }

        private void WriteEnumMembers(Declaration[]? members)
        {
            if (members == null || members.Length == 0 || SettingsModel.Instance == null) { return; }

            foreach (var mem in members)
            {
                var memP = new Paragraph();
                memP.Inlines.Add(MakeRun("Member: ", SettingsModel.Instance.MemberHeadingStyle.FontSize, SettingsModel.Instance.MemberHeadingStyle.IsBold,
                    Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberHeadingStyle.IsItalic));
                memP.Inlines.Add(MakeRun($"{mem.Name} - ", SettingsModel.Instance.MemberStyle.FontSize, SettingsModel.Instance.MemberStyle.IsBold,
                    Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberStyle.IsItalic));
                memP.Inlines.Add(MakeRun(mem.Definition ?? "NO SUMMARY", SettingsModel.Instance.MemberStyle.FontSize, SettingsModel.Instance.MemberStyle.IsBold,
                    Color.FromRgb(1, 1, 1), SettingsModel.Instance.MemberStyle.IsItalic));
                myRichTextBox.Document.Blocks.Add(memP);
            }
        }

        private void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            FilePopup.IsOpen = true;
        }

        private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void OnMaximizeButtonClicked(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;

            if(WindowState == WindowState.Maximized)
            {
                MaximiseButton.Content = "[]";
            }else if(WindowState == WindowState.Normal)
            {
                MaximiseButton.Content = "[  ]";
            }
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Allow dragging the window by clicking on the custom title bar
        private void OnChromeBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }

            if(e.ClickCount == 2)
            {
                OnMaximizeButtonClicked(sender, e);
            }
        }

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;
            /// <summary>y coordinate of point.</summary>
            public int y;
            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new RECT();
            public int Width { get { return Math.Abs(right - left); } }
            public int Height { get { return bottom - top; } }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }
            public bool IsEmpty { get { return left >= right || top >= bottom; } }
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }
            public override bool Equals(object? obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }
            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);


        private void OnExportButtonClick(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = true;
        }

        private void OnExportMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            // Close both popups
            FilePopup.IsOpen = false;
            ExportPopup.IsOpen = false;
        }
    }
}
