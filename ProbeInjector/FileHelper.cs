using System;
using System.IO;

namespace ProbeInjector
{
    internal static class FileHelper
    {
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
