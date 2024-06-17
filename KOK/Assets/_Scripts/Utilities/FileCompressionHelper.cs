using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace KOK
{
    public static class FileCompressionHelper
    {
        public static string CompressWavFileAsZip(string sourceFileLocation, string sourceFilePath)
        {

            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string compressedFilePath = Path.Combine(sourceFileLocation, fileName + ".zip");
            try
            {
                using (FileStream zipToOpen = new FileStream(compressedFilePath, FileMode.OpenOrCreate))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        ZipArchiveEntry entry = archive.CreateEntryFromFile(sourceFilePath, fileName + ".wav", System.IO.Compression.CompressionLevel.Optimal);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return string.Empty;
            }

            return compressedFilePath;
        }

        public static string ExtractWavFileFromZip(string zipFilePath, string destinationFolder)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                        {
                            string destinationPath = Path.Combine(destinationFolder, entry.FullName);

                            // Ensure the destination directory exists
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                            // Extract the file
                            entry.ExtractToFile(destinationPath, overwrite: true);
                            return destinationPath;
                        }
                    }
                }
                throw new FileNotFoundException("No WAV file found in the ZIP archive.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return string.Empty;
            }
        }
    }
}
