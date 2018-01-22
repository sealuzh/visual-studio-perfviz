using System.IO;

namespace ProbeInjector
{
    internal static class FileHelper
    {
        /// <summary>
        /// Returns an available FilePath - adds index to the FileName
        ///     FileName.txt -> FileName (2).txt
        /// </summary>
        public static string GetAvailableFilePath(string filePath)
        {
            var fileDirectory = Path.GetDirectoryName(filePath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            var fileExtension = Path.GetExtension(filePath);
            var index = 2;
            while (File.Exists(filePath))
            {
                // Getting a new FileName (that doesn't exist already)
                var newFileName = $"{fileNameWithoutExtension} ({index}){fileExtension}";
                filePath = fileDirectory != null ? Path.Combine(fileDirectory, newFileName) : newFileName;
                index++;
            }
            return filePath;
        }
    }
}
