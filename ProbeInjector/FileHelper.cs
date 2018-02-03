using System;
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
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            for (var i = 2; File.Exists(filePath); i++)
            {
                var newFileName = $"{Path.GetFileNameWithoutExtension(filePath)} ({i}){Path.GetExtension(filePath)}";
                var directory = Path.GetDirectoryName(filePath);
                filePath = directory != null ? Path.Combine(directory, newFileName) : newFileName;
            }
            return filePath;
        }


        /// <summary>
        /// Copy the Dlls from <see cref="sourceDirectory"/> to <see cref="destinationDirectory"/>
        /// </summary>
        public static void CopyDllsInDirectory(string sourceDirectory, string destinationDirectory)
        {
            if (string.IsNullOrWhiteSpace(sourceDirectory))
            {
                throw new ArgumentNullException(nameof(sourceDirectory));
            }
            if (string.IsNullOrWhiteSpace(destinationDirectory))
            {
                throw new ArgumentNullException(nameof(destinationDirectory));
            }

            var sourceDllFilePaths = Directory.GetFiles(sourceDirectory, "*.dll");
            foreach (var sourceFilePath in sourceDllFilePaths)
            {
                var destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(sourceFilePath));
                File.Copy(sourceFilePath, destinationFilePath, true);
            }
        }
    }
}
