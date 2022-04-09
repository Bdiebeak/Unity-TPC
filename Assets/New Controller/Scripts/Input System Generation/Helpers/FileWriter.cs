using System.IO;

namespace Bdiebeak.InputSystemGeneration
{
    public static class FileWriter
    {
        public static void CreateAndWrite(string directoryPath, string fileNameWithExtension, string data)
        {
            var fullPath = $"{directoryPath}/{fileNameWithExtension}";
            
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(fullPath, data);
        }
    }
}