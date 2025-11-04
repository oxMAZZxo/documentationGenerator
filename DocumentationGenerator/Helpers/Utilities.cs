using System;

namespace DocumentationGenerator.Helpers;

public static class Utilities
{

    public static byte HexToByte(string hex)
    {
        if (hex.Length != 2)
            throw new ArgumentException("Hex string must be 2 characters long.");

        int high = HexCharToInt(hex[0]);
        int low = HexCharToInt(hex[1]);

        return (byte)((high << 4) + low);
    }

    public static int HexCharToInt(char c)
    {
        if (c >= '0' && c <= '9')
            return c - '0';
        if (c >= 'A' && c <= 'F')
            return c - 'A' + 10;
        if (c >= 'a' && c <= 'f')
            return c - 'a' + 10;

        throw new ArgumentException("Invalid hex character: " + c);
    }

    // public static Microsoft.Msagl.Drawing.Color UIntColourToMSAGLColour(uint r, uint g, uint b)
    // {
    //     return new Microsoft.Msagl.Drawing.Color(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
    // }

    // public static Microsoft.Msagl.Drawing.Color MigraDocColourToMSAGLColour(MigraDoc.DocumentObjectModel.Color color)
    // {
    //     return new Microsoft.Msagl.Drawing.Color(Convert.ToByte(color.R), Convert.ToByte(color.G), Convert.ToByte(color.B));
    // }

}

public enum XmlTag
{
    summary,
    returns
}