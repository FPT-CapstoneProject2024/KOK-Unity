using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Audio;

namespace KOK
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(FFMPEG))]
    public class TestLoadDirectWavFile : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioMixerGroup audioMixerGroup;

        string wavFilePath;
        public void OnClickTest()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            DownLoadFile();

            StartCoroutine(PlayAudio());
        }

        private void DownLoadFile()
        {
            string localFilePath = "S4_minhcute_10-08-2024_23-48-06";
            audioSource.clip = null;
            //audioSource.clip = Resources.Load<AudioClip>("AudioProcess/S4_minhcute_10-08-2024_23-48-06");
            //Debug.Log("audioSource" + audioSource.clip);

            FirebaseStorageManager.Instance
                .DownloadVoiceRecordingFile(
                    "Assets/Resources/AudioProcess/" + localFilePath + ".zip",
                    () =>
                    {

                        //StartCoroutine(PlayAudio());
                        wavFilePath = ExtractWavFileFromZip("Assets/Resources/AudioProcess/" + localFilePath + ".zip", "Assets/Resources/AudioProcess/");
                        Debug.Log(wavFilePath + "\n" + FileCompressionHelper.ExtractWavFileFromZip("Assets/Resources/AudioProcess/" + localFilePath + ".zip", "Assets/Resources/AudioProcess/"));
                        Debug.Log("AudioProcess/" + Path.GetFileNameWithoutExtension(wavFilePath));
                        
                        //Debug.Log("audioSource" + audioSource.clip);
                    },
                    () => { }
                );
        }

        IEnumerator PlayAudio()
        {
            yield return new WaitForSeconds(1f);
            string fullPath = "AudioProcess/" + Path.GetFileNameWithoutExtension(wavFilePath);
            audioSource.clip = Resources.Load<AudioClip>(fullPath);
            Debug.Log("wavFilePath:" + wavFilePath);
            if (audioSource.clip == null)
            {
                StartCoroutine(PlayAudio());
            }
            else
            {
                Debug.LogError("AudioProcess/" + Path.GetFileNameWithoutExtension(wavFilePath));
                //audioSource.clip = Resources.Load<AudioClip>("AudioProcess/" + Path.GetFileNameWithoutExtension(wavFilePath));
                //audioSource.clip = Resources.Load<AudioClip>(fullPath);
                audioSource.Play();
                Debug.Log(audioSource.isPlaying);
            }
        }

        public  string ExtractWavFileFromZip(string zipFilePath, string destinationFolder)
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
