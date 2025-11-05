using System;
using System.IO;
using PdfSharp.Fonts;

namespace DocumentationGenerator.Helpers;

public class SimpleFontResolver
{
    public byte[] GetFont(string faceName)
    {
        string fontPath = "Invalid";

        switch (faceName)
        {
            case "Arial#": fontPath = "arial.ttf"; break;
            case "Arial-Black#": fontPath = "ariblk.ttf"; break;
            case "Arial-Bold#": fontPath = "arialbd.ttf"; break;
            case "Arial-Italic#": fontPath = "ariali.ttf"; break;
            case "Arial-BoldItalic#": fontPath = "arialbi.ttf"; break;

            case "Courier-New#": fontPath = "cour.ttf"; break;
            case "Courier-New-Bold#": fontPath = "courbd.ttf"; break;
            case "Courier-New-Italic#": fontPath = "couri.ttf"; break;
            case "Courier-New-BoldItalic#": fontPath = "courbi.ttf"; break;
        }

        if (fontPath == "Invalid")
        {
            throw new InvalidOperationException("Unknown face name: " + faceName);
        }

        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "fonts", fontPath));
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        familyName = familyName.ToLowerInvariant();
        if (familyName == "courier new")
        {
            if (isBold && isItalic) { return new FontResolverInfo("Courier-New-BoldItalic#"); }
            if (isBold) { return new FontResolverInfo("Courier-New-Bold#"); }
            if (isItalic) { return new FontResolverInfo("Courier-New-Italic#"); }
            return new FontResolverInfo("Courier-New#");
        }

        if (familyName == "arial black")
        {
            return new FontResolverInfo("Arial-Black#");
        }

        if (familyName == "arial")
        {
            if (isBold && isItalic) { return new FontResolverInfo("Arial-BoldItalic#"); }

            if (isBold) { return new FontResolverInfo("Arial-Bold#"); }

            if (isItalic) { return new FontResolverInfo("Arial-Italic#"); }

            return new FontResolverInfo("Arial#");
        }

        throw new InvalidOperationException("Font family not resolved: " + familyName);
    }
}