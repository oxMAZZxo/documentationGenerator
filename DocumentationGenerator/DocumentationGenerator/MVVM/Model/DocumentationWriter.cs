using System.IO;

namespace DocumentationGenerator.MVVM.Model
{
    public class DocumentationWriter
    {
        private StreamWriter? streamWriter;

        public DocumentationWriter()
        {
        }

        public async Task<bool> WriteDocumentation(string path, string data)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path) || path == "") { return false; }

            if(streamWriter == null) { streamWriter = new StreamWriter(path); }
            
            await streamWriter.WriteAsync(data);
            streamWriter.Close();

            return true;
        }
    }
}