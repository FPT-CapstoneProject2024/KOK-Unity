using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK
{
    [RequireComponent(typeof(AudioSource))]
    public class RecordingManager : MonoBehaviour
    {
        public static RecordingManager Instance { get; private set; }

        public string cloudFileName1 = string.Empty;
        public string cloudFileName2 = string.Empty;
        private string recordingsTempLocation = string.Empty;
        private AudioSource audioSource;

        string audioFile1 = "";
        string audioFile2 = ""; 
        [SerializeField] VoiceRecorder voiceRecorder;

        public string RecordingFileName;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            recordingsTempLocation = Application.persistentDataPath + "/Recordings/Temp";
            InitializeAudioSources();
        }

        public void StartRecording()
        {
            audioFile1 = "SongCode_SingerUsername_" + DateTime.Now.ToString();
            audioFile1 = audioFile1.Replace(" ", "");
            audioFile1 = audioFile1.Replace(":", "");
            audioFile1 = audioFile1.Replace("/", "");
            voiceRecorder.StartRecording(audioFile1);
        }

        public void StopRecording()
        {
            voiceRecorder.StopRecording();
            //CreateRecording(
            //    "testing",
            //    UnityEngine.Random.Range(1, 99),
            //    "ebe4174c-5069-4767-a5c3-a962563d813f",
            //    "b7e64218-d316-4f2f-bde1-b96ec24057ec",
            //    "b7e64218-d316-4f2f-bde1-b96ec24057ec",
            //    "60135c7c-003b-453a-b7c1-6c2c2835a423",
            //    new List<string>()
            //    {
            //        "b7e64218-d316-4f2f-bde1-b96ec24057ec",
            //    }
            //    );
        }

        public void CreateRecording(string recordingName,int recordingType, int score, string purchasedSongId, string hostId, string ownerId, string karaokeRoomId, List<string> audioFilePath, List<string> memberIds)
        {
            //Get information


            //Create Recording request
            CreateRecordingRequest createRecordingRequest = new CreateRecordingRequest()
            {
                RecordingName = recordingName,
                RecordingType = recordingType,
                Score = score,
                PurchasedSongId = Guid.Parse(purchasedSongId),
                HostId = Guid.Parse(hostId),
                OwnerId = Guid.Parse(ownerId),
                KaraokeRoomId = Guid.Parse(karaokeRoomId),
                StartTime = 0,
                EndTime = 0,
                VoiceAudios = new List<CreateVoiceAudioRequest>()
            };
            for (int i = 0; i< audioFilePath.Count; i++)
            {
                createRecordingRequest.VoiceAudios.Add(
                    new()
                    {
                        VoiceUrl = audioFilePath[i],
                        DurationSecond = 0,
                        StartTime = 0f,
                        EndTime = 0f,
                        MemberId = Guid.Parse(memberIds[i]),
                    });
            }
            //Debug.LogError(createRecordingRequest);

            //Call api create recording, set by file name



            ApiHelper.Instance.GetComponent<RecordingController>().AddRecordingCoroutine(
                createRecordingRequest,
                (rr) => { /*Debug.Log(rr.Value);*/ },
                (ex) => { Debug.LogError(ex.Message); }
            );

        }

        //[ContextMenu("Prepare audio")]
        //private void PrepareAudioSource()
        //{
        //    LoadVoiceRecordingToAudioSource(cloudFileName1, recordingsTempLocation + "/test.zip", 0);
        //}

        [ContextMenu("Play audio")]
        private void PlayAudio()
        {
            PlayAudioSource(0);
            Debug.Log("Play audio source at position 0");
        }

        private void InitializeAudioSources()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("No audio sources attached to recording manager!");
                return;
            }
            audioSource.playOnAwake = false;
        }

        public void LoadRecording()
        {

        }

        //public async void LoadVoiceRecordingToAudioSource(string cloudFileName, string localZipFilePath, int audioSourceIndex)
        //{
        //    AudioClip clip = await LoadWavToAudioClip(cloudFileName, localZipFilePath);
        //    if (clip == null)
        //    {
        //        Debug.LogError("Failed to load voice recording to audio source. Cannot download and load wav file to audio clip!");
        //        return;
        //    }
        //    audioSource.clip = clip;
        //    Debug.Log("Voice recording successfully loaded to audio source!");
        //}

        //private async Task<AudioClip> LoadWavToAudioClip(string cloudFileName, string localZipFilePath)
        //{
        //    // Get download url
        //    string downloadUrl = await FirebaseStorageManager.Instance.GetRecordingDownloadUrl(cloudFileName);
        //    if (string.IsNullOrEmpty(downloadUrl))
        //    {
        //        Debug.LogError("Failed to retrieve voice recording download url");
        //        return null;
        //    }

        //    // Download .zip file
        //    await FirebaseStorageManager.Instance.DownloadVoiceRecordingFile(downloadUrl, localZipFilePath);

        //    if (!File.Exists(localZipFilePath))
        //    {
        //        Debug.LogError("Failed to download voice recording with given download url");
        //        return null;
        //    }

        //    // Extract .zip file
        //    string wavFilePath = FileCompressionHelper.ExtractWavFileFromZip(localZipFilePath, recordingsTempLocation);

        //    if (string.IsNullOrEmpty(wavFilePath))
        //    {
        //        Debug.LogError("Failed to extract zipped voice recording file");
        //        return null;
        //    }

        //    // Load wav file to audio clip
        //    AudioClip audioClip = WavHelper.WavFileToAudioClip(wavFilePath);
        //    if (audioClip == null)
        //    {
        //        Debug.LogError("Failed to load wav file to audio clip");
        //        return null;
        //    }

        //    // Clean up logic
        //    try
        //    {
        //        if (File.Exists(localZipFilePath))
        //        {
        //            File.Delete(localZipFilePath);
        //            Debug.Log($"Deleted local zip file: {localZipFilePath}");
        //        }

        //        if (File.Exists(wavFilePath))
        //        {
        //            File.Delete(wavFilePath);
        //            Debug.Log($"Deleted local wav file: {wavFilePath}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError($"Failed to clean up files: {ex}");
        //    }

        //    return audioClip;
        //}

        #region AudioSourcesManupilation

        public void PlayAllAudioSources()
        {
            //if (audioSources.Length <= 0)
            //{
            //    Debug.LogError("No audio sources attached to recording manager!");
            //    return;
            //}

            //// Check if all audio sources contains audio clip
            //if (audioSources[0].clip == null || audioSources[1].clip == null)
            //{
            //    Debug.LogError("One or multiple audio sources does not contains any audio clip!");
            //    return;
            //}

            //for (int i = 0; i < audioSources.Length; i++)
            //{
            //    audioSources[i].Play();
            //}
        }

        public void StopAllAudioSources()
        {
            //if (audioSources.Length <= 0)
            //{
            //    Debug.LogError("No audio sources attached to recording manager!");
            //    return;
            //}

            //foreach (AudioSource audioSource in audioSources)
            //{
            //    audioSource.Stop();
            //}
        }

        public void PauseAllAudioSources()
        {
            //if (audioSources.Length <= 0)
            //{
            //    Debug.LogError("No audio sources attached to recording manager!");
            //    return;
            //}
            //foreach (AudioSource audioSource in audioSources)
            //{
            //    audioSource.Pause();
            //}
        }

        public void UnPauseAllAudioSources()
        {
            //if (audioSources.Length <= 0)
            //{
            //    Debug.LogError("No audio sources attached to recording manager!");
            //    return;
            //}
            //foreach (AudioSource audioSource in audioSources)
            //{
            //    audioSource.UnPause();
            //}
        }

        public void PlayAudioSource(int audioSourceIndex)
        {
            //if (audioSources[audioSourceIndex] == null)
            //{
            //    Debug.LogError("No audio sources attached to recording manager!");
            //    return;
            //}

            //// Check if audio source contains any audio clip
            //if (audioSources[audioSourceIndex].clip == null)
            //{
            //    Debug.LogError($"Audio source at position {audioSourceIndex} does not contains any audio clip!");
            //    return;
            //}

            //audioSources[audioSourceIndex].Play();
        }

        public void StopAudioSource(int audioSourceIndex)
        {
            //if (audioSources[audioSourceIndex] == null)
            //{
            //    Debug.LogError("No audio sources attached to recording manager!");
            //    return;
            //}

            //audioSources[audioSourceIndex].Stop();
        }

        public void PauseAudioSource(int audioSourceIndex)
        {
            //if (audioSources[audioSourceIndex] == null)
            //{
            //    Debug.LogError("No audio sources attached to recording manager!");
            //    return;
            //}

            //audioSources[audioSourceIndex].Pause();
        }

        public void UnPauseAudioSource(int audioSourceIndex)
        {
            //if (audioSources[audioSourceIndex] == null)
            //{
            //    Debug.LogError("No audio sources attached to recording manager!");
            //    return;
            //}

            //audioSources[audioSourceIndex].UnPause();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
        #endregion
    }
}
