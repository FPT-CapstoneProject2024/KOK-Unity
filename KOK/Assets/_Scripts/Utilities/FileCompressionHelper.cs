using System.IO;
using System.IO.Compression;

namespace KOK
{
    public static class FileCompressionHelper
    {
        public static string CompressWavFileAsZip(string sourceFileLocation, string sourceFilePath)
        {

            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string compressedFilePath = Path.Combine(sourceFileLocation, fileName + ".zip");
            if (!File.Exists(compressedFilePath))
            {
                using (FileStream fs = File.Create(compressedFilePath))
                {
                    // File.Create will create the file, so there's nothing else to do here
                }
            }
            using (FileStream zipToOpen = new FileStream(compressedFilePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry entry = archive.CreateEntryFromFile(sourceFilePath, fileName + ".wav", CompressionLevel.Optimal);
                }
            }
            return compressedFilePath;
        }
    }
}
