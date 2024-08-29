using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Audio;

namespace KOK
{
    public class RecordingHelper : MonoBehaviour
    {
        [SerializeField] AudioMixerGroup audioMixerGroup;

        string wavFilePath;

        public void PrepareAudioSourceDownloadAudio(string localAudioFilePath, Action<AudioClip> onSuccess, Action onError)
        {
            localAudioFilePath = localAudioFilePath.Replace(".wav", "");
            localAudioFilePath = localAudioFilePath.Replace(".zip", "");
            StopAllCoroutines();
            DownLoadFile(localAudioFilePath, onError);
            StartCoroutine(AssignAudio(onSuccess));

        }

        public void PrepareAudioSourceLoadAudioClip(string localAudioFilePath, Action<AudioClip> onSuccess, Action onError)
        {
            localAudioFilePath = localAudioFilePath.Replace(".wav", "");
            localAudioFilePath = localAudioFilePath.Replace(".zip", "");
            AudioClip clip = Resources.Load<AudioClip>(localAudioFilePath);
        }




        private void DownLoadFile(string localAudioFilePath, Action onError)
        {
            FirebaseStorageManager.Instance
                .DownloadVoiceRecordingFile(
                    "Assets/Resources/AudioProcess/" + localAudioFilePath +".zip",
                    () =>
                    {
                        wavFilePath = ExtractWavFileFromZip("Assets/Resources/AudioProcess/" + localAudioFilePath + ".zip", "Assets/Resources/AudioProcess/");
                        Debug.Log(wavFilePath + "\n" + localAudioFilePath + "\n" + FileCompressionHelper.ExtractWavFileFromZip("Assets/Resources/AudioProcess/" + localAudioFilePath + ".zip", "Assets/Resources/AudioProcess/"));
                        //Debug.Log("AudioProcess/" + Path.GetFileNameWithoutExtension(wavFilePath));

                    },
                    () =>
                    {
                        onError.Invoke();
                    }
                );
        }

        IEnumerator AssignAudio(Action<AudioClip> onSuccess)
        {
            yield return new WaitForSeconds(1f);
            string fullPath = "AudioProcess/" + Path.GetFileNameWithoutExtension(wavFilePath);
            var audioClip = Resources.Load<AudioClip>(fullPath);
            if (audioClip == null)
            {
                StartCoroutine(AssignAudio(onSuccess));
            }
            else
            {
                Debug.Log("AudioProcess/" + Path.GetFileNameWithoutExtension(wavFilePath));
                onSuccess.Invoke(audioClip);
            }
        }

        private string ExtractWavFileFromZip(string zipFilePath, string destinationFolder)
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
