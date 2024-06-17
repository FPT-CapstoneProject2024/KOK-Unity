using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK
{
    public class RecordingManager : Singleton<RecordingManager>
    {
        public string cloudFileName1 = string.Empty;
        public string cloudFileName2 = string.Empty;
        private string recordingsTempLocation = string.Empty;
        private AudioSource[] audioSources;

        private void Start()
        {
            recordingsTempLocation = Application.persistentDataPath + "/Recordings/Temp";
            InitializeAudioSources();
        }

        [ContextMenu("Prepare audio")]
        private void PrepareAudioSource()
        {
            LoadVoiceRecordingToAudioSource(cloudFileName1, recordingsTempLocation + "/test.zip", 0);
        }

        [ContextMenu("Play audio")]
        private void PlayAudio()
        {
            PlayAudioSource(0);
            Debug.Log("Play audio source at position 0");
        }

        private void InitializeAudioSources()
        {
            audioSources = GetComponents<AudioSource>();
            if (audioSources.Length <= 0)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.playOnAwake = false;
            }
        }

        public void LoadRecording()
        {

        }

        public async void LoadVoiceRecordingToAudioSource(string cloudFileName, string localZipFilePath, int audioSourceIndex)
        {
            AudioClip clip = await LoadWavToAudioClip(cloudFileName, localZipFilePath);
            if (clip == null)
            {
                Debug.LogError("Failed to load voice recording to audio source. Cannot download and load wav file to audio clip!");
                return;
            }
            audioSources[audioSourceIndex].clip = clip;
            Debug.Log("Voice recording successfully loaded to audio source!");
        }

        private async Task<AudioClip> LoadWavToAudioClip(string cloudFileName, string localZipFilePath)
        {
            // Get download url
            string downloadUrl = await FirebaseStorageManager.Instance.GetRecordingDownloadUrl(cloudFileName);
            if (string.IsNullOrEmpty(downloadUrl))
            {
                Debug.LogError("Failed to retrieve voice recording download url");
                return null;
            }

            // Download .zip file
            await FirebaseStorageManager.Instance.DownloadVoiceRecordingFile(downloadUrl, localZipFilePath);

            if (!File.Exists(localZipFilePath))
            {
                Debug.LogError("Failed to download voice recording with given download url");
                return null;
            }

            // Extract .zip file
            string wavFilePath = FileCompressionHelper.ExtractWavFileFromZip(localZipFilePath, recordingsTempLocation);

            if (string.IsNullOrEmpty(wavFilePath))
            {
                Debug.LogError("Failed to extract zipped voice recording file");
                return null;
            }

            // Load wav file to audio clip
            AudioClip audioClip = WavHelper.WavFileToAudioClip(wavFilePath);
            if (audioClip == null)
            {
                Debug.LogError("Failed to load wav file to audio clip");
                return null;
            }

            // Clean up logic
            try
            {
                if (File.Exists(localZipFilePath))
                {
                    File.Delete(localZipFilePath);
                    Debug.Log($"Deleted local zip file: {localZipFilePath}");
                }

                if (File.Exists(wavFilePath))
                {
                    File.Delete(wavFilePath);
                    Debug.Log($"Deleted local wav file: {wavFilePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to clean up files: {ex}");
            }

            return audioClip;
        }

        #region AudioSourcesManupilation

        public void PlayAllAudioSources()
        {
            if (audioSources.Length <= 0)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }

            // Check if all audio sources contains audio clip
            if (audioSources[0].clip == null || audioSources[1].clip == null)
            {
                Debug.LogError("One or multiple audio sources does not contains any audio clip!");
                return;
            }

            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSources[i].Play();
            }
        }

        public void StopAllAudioSources()
        {
            if (audioSources.Length <= 0)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }

            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.Stop();
            }
        }

        public void PauseAllAudioSources()
        {
            if (audioSources.Length <= 0)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.Pause();
            }
        }

        public void UnPauseAllAudioSources()
        {
            if (audioSources.Length <= 0)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.UnPause();
            }
        }

        public void PlayAudioSource(int audioSourceIndex)
        {
            if (audioSources[audioSourceIndex] == null)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }

            // Check if audio source contains any audio clip
            if (audioSources[audioSourceIndex].clip == null)
            {
                Debug.LogError($"Audio source at position {audioSourceIndex} does not contains any audio clip!");
                return;
            }

            audioSources[audioSourceIndex].Play();
        }

        public void StopAudioSource(int audioSourceIndex)
        {
            if (audioSources[audioSourceIndex] == null)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }

            audioSources[audioSourceIndex].Stop();
        }

        public void PauseAudioSource(int audioSourceIndex)
        {
            if (audioSources[audioSourceIndex] == null)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }

            audioSources[audioSourceIndex].Pause();
        }

        public void UnPauseAudioSource(int audioSourceIndex)
        {
            if (audioSources[audioSourceIndex] == null)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }

            audioSources[audioSourceIndex].UnPause();
        }

        #endregion
    }
}
