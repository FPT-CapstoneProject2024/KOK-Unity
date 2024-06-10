using System.IO;
using UnityEngine;

namespace KOK
{
    public static class WavHelper
    {
        /// <summary>
        /// Saves the provided AudioClip to a WAV file at the specified file path.
        /// </summary>
        /// <param name="filePath">The path where the WAV file will be saved.</param>
        /// <param name="clip">The AudioClip to be saved as a WAV file.</param>
        public static void SaveWavFile(string filePath, AudioClip clip)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                WriteWavFile(fileStream, clip);
            }
        }

        /// <summary>
        /// Converts an AudioClip to a WAV byte array.
        /// </summary>
        /// <param name="clip">The AudioClip to convert.</param>
        /// <returns>A byte array containing the WAV file data.</returns>
        public static byte[] GetWavBytes(AudioClip clip)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                WriteWavFile(memoryStream, clip);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Writes the WAV file data to the provided stream from an AudioClip.
        /// </summary>
        /// <param name="stream">The stream to write the WAV data to.</param>
        /// <param name="clip">The AudioClip containing the audio data.</param>
        private static void WriteWavFile(Stream stream, AudioClip clip)
        {
            // Number of samples in the audio clip
            int sampleCount = clip.samples;
            // Number of channels (e.g., 1 for mono, 2 for stereo)
            int channelCount = clip.channels;
            // Sampling rate (e.g., 44100 Hz)
            int sampleRate = clip.frequency;

            // Bit depth (16 bits per sample)
            short bitsPerSample = 16;
            // Number of bytes per sample (2 bytes for 16-bit samples)
            int bytesPerSample = bitsPerSample / 8;
            // Byte rate (sampleRate * channelCount * bytesPerSample)
            int byteRate = sampleRate * channelCount * bytesPerSample;
            // Block align (channelCount * bytesPerSample)
            int blockAlign = channelCount * bytesPerSample;
            // Size of the data sub-chunk (sampleCount * channelCount * bytesPerSample)
            int subChunk2Size = sampleCount * channelCount * bytesPerSample;
            // Size of the overall chunk (36 + subChunk2Size)
            int chunkSize = 36 + subChunk2Size;

            // BinaryWriter to write data to the stream
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Write the RIFF header
                writer.Write("RIFF".ToCharArray()); // Chunk ID
                writer.Write(chunkSize);            // Chunk size
                writer.Write("WAVE".ToCharArray()); // Format

                // Write the format sub-chunk (fmt)
                writer.Write("fmt ".ToCharArray()); // Sub-chunk ID
                writer.Write(16);                   // Sub-chunk size (16 for PCM)
                writer.Write((short)1);             // Audio format (1 for PCM)
                writer.Write((short)channelCount);  // Number of channels
                writer.Write(sampleRate);           // Sample rate
                writer.Write(byteRate);             // Byte rate
                writer.Write((short)blockAlign);    // Block align
                writer.Write(bitsPerSample);        // Bits per sample

                // Write the data sub-chunk
                writer.Write("data".ToCharArray()); // Sub-chunk ID
                writer.Write(subChunk2Size);        // Sub-chunk size

                // Array to hold the audio samples
                float[] samples = new float[sampleCount * channelCount];

                // Get the audio data from the clip
                clip.GetData(samples, 0);

                // Write each sample to the stream as a 16-bit integer
                foreach (float sample in samples)
                {
                    // Convert the sample from float to short (16-bit) and clamp it
                    short intSample = (short)Mathf.Clamp(sample * short.MaxValue, short.MinValue, short.MaxValue);
                    // Write the sample to the stream
                    writer.Write(intSample);
                }
            }

        }

        /// <summary>
        /// Delete local file with given file path. Use for clean up purpose after upload file to cloud storage.
        /// </summary>
        /// <param name="localFilePath">Path to the to be deleted local file.</param>
        public static void DeleteLocalFile(string localFilePath)
        {
            if (File.Exists(localFilePath))
            {
                File.Delete(localFilePath);
                Debug.Log($"File successfully deleted: {localFilePath}");
            }
            else
            {
                Debug.LogError($"File not found: {localFilePath}");
            }
        }
    }
}
