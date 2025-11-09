using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using SkiaSharp;

namespace DocumentationGenerator.Helpers;

public static class Utilities
{
    public static SKColor MigraDocColorToSKColor(MigraDoc.DocumentObjectModel.Color color)
    {
        return new SKColor(color.Argb);
    }
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

    public static async Task<bool> CopyFileAsync(string sourceFileName, string sourceFilePath, IStorageFolder outputFolder)
    {
        Stream? sourceStream = await TryOpenReadStream(sourceFilePath);

        if(sourceStream == null) { return false; }
        
        IStorageFile? copyFile = await outputFolder.CreateFileAsync(sourceFileName);

        if (copyFile == null) { return false; }

        Stream destinationStream = await copyFile.OpenWriteAsync();

        await sourceStream.CopyToAsync(destinationStream);

        destinationStream.Close();
        await destinationStream.DisposeAsync();

        sourceStream.Close();
        await sourceStream.DisposeAsync();

        copyFile.Dispose();

        return true;
    }

    public static async Task<Stream?> TryOpenReadStream(string sourceFilePath)
    {
        try
        {
            if (App.Instance == null || App.Instance.TopLevel == null) { return null; }

            IStorageFile? file = await App.Instance.TopLevel.StorageProvider.TryGetFileFromPathAsync(sourceFilePath);
            if (file == null) { throw new FileNotFoundException(); }

            return await file.OpenReadAsync();
        }
        catch (FileNotFoundException)
        {
            Debug.WriteLine($"File: {sourceFilePath}, could not be found");
            return null;
        }
        catch (DirectoryNotFoundException)
        {
            Debug.WriteLine($"Directory of file {sourceFilePath}, could not be found");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not complete resource copying operation because of the following reason.\n Message: {ex.Message}");
            return null;
        }
    }
}

public enum XmlTag
{
    summary,
    returns
}