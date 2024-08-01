using System;
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

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
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

        #region .wav file bytes to Unity AudioClip conversion methods

        public static AudioClip WavFileToAudioClip(string wavFilePath)
        {
            if (!wavFilePath.StartsWith(Application.persistentDataPath) && !wavFilePath.StartsWith(Application.dataPath))
            {
                Debug.LogWarning("This only supports files that are stored using Unity's Application data path. \nTo load bundled resources use 'Resources.Load(\"filename\") typeof(AudioClip)' method. \nhttps://docs.unity3d.com/ScriptReference/Resources.Load.html");
                return null;
            }
            byte[] fileBytes = File.ReadAllBytes(wavFilePath);
            return WavBytesToAudioClip(fileBytes, 0, Path.GetFileNameWithoutExtension(wavFilePath));
        }

        public static AudioClip WavBytesToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "wav")
        {
            //string riff = Encoding.ASCII.GetString (fileBytes, 0, 4);
            //string wave = Encoding.ASCII.GetString (fileBytes, 8, 4);
            int subChunk1 = BitConverter.ToInt32(fileBytes, 16);
            UInt16 audioFormat = BitConverter.ToUInt16(fileBytes, 20);

            // NB: Only uncompressed PCM wav files are supported.
            string formatCode = FormatCode(audioFormat);
            Debug.AssertFormat(audioFormat == 1 || audioFormat == 65534, "Detected format code '{0}' {1}, but only PCM and WaveFormatExtensible uncompressed formats are currently supported.", audioFormat, formatCode);

            UInt16 channels = BitConverter.ToUInt16(fileBytes, 22);
            int sampleRate = BitConverter.ToInt32(fileBytes, 24);
            //int byteRate = BitConverter.ToInt32 (fileBytes, 28);
            //UInt16 blockAlign = BitConverter.ToUInt16 (fileBytes, 32);
            UInt16 bitDepth = BitConverter.ToUInt16(fileBytes, 34);

            int headerOffset = 16 + 4 + subChunk1 + 4;
            int subChunk2 = BitConverter.ToInt32(fileBytes, headerOffset);
            //Debug.LogFormat ("riff={0} wave={1} subChunk1={2} format={3} channels={4} sampleRate={5} byteRate={6} blockAlign={7} bitDepth={8} headerOffset={9} subChunk2={10} filesize={11}", riff, wave, subChunk1, formatCode, channels, sampleRate, byteRate, blockAlign, bitDepth, headerOffset, subChunk2, fileBytes.Length);

            float[] data;
            switch (bitDepth)
            {
                case 8:
                    data = Convert8BitByteArrayToAudioClipData(fileBytes, headerOffset, subChunk2);
                    break;
                case 16:
                    data = Convert16BitByteArrayToAudioClipData(fileBytes, headerOffset, subChunk2);
                    break;
                case 24:
                    data = Convert24BitByteArrayToAudioClipData(fileBytes, headerOffset, subChunk2);
                    break;
                case 32:
                    data = Convert32BitByteArrayToAudioClipData(fileBytes, headerOffset, subChunk2);
                    break;
                default:
                    throw new Exception(bitDepth + " bit depth is not supported.");
            }

            AudioClip audioClip = AudioClip.Create(name, data.Length, (int)channels, sampleRate, false);
            audioClip.SetData(data, 0);
            return audioClip;
        }

        private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 8-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            float[] data = new float[wavSize];

            sbyte maxValue = sbyte.MaxValue;

            int i = 0;
            while (i < wavSize)
            {
                data[i] = (float)source[i] / maxValue;
                ++i;
            }

            return data;
        }

        private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 16-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = sizeof(Int16); // block size = 2
            int convertedSize = wavSize / x;

            float[] data = new float[convertedSize];

            Int16 maxValue = Int16.MaxValue;

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = i * x + headerOffset;
                data[i] = (float)BitConverter.ToInt16(source, offset) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 24-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = 3; // block size = 3
            int convertedSize = wavSize / x;

            int maxValue = Int32.MaxValue;

            float[] data = new float[convertedSize];

            byte[] block = new byte[sizeof(int)]; // using a 4 byte block for copying 3 bytes, then copy bytes with 1 offset

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = i * x + headerOffset;
                Buffer.BlockCopy(source, offset, block, 1, x);
                data[i] = (float)BitConverter.ToInt32(block, 0) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 32-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = sizeof(float); //  block size = 4
            int convertedSize = wavSize / x;

            Int32 maxValue = Int32.MaxValue;

            float[] data = new float[convertedSize];

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = i * x + headerOffset;
                data[i] = (float)BitConverter.ToInt32(source, offset) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static string FormatCode(UInt16 code)
        {
            switch (code)
            {
                case 1:
                    return "PCM";
                case 2:
                    return "ADPCM";
                case 3:
                    return "IEEE";
                case 7:
                    return "μ-law";
                case 65534:
                    return "WaveFormatExtensible";
                default:
                    Debug.LogWarning("Unknown wav code format:" + code);
                    return "";
            }
        }

        #endregion
    }
}
