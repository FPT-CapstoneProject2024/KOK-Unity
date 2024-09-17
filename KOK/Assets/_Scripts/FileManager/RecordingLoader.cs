using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.VoiceAudios;
using System;
using KOK.ApiHandler.DTOModels;
using System.IO;
using KOK;
using KOK.Assets._Scripts.FileManager;
using System.Collections;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using UnityEngine.Video;
using System.Linq;

public class RecordingLoader : MonoBehaviour
{
    //public TMP_Dropdown tmpDropdown; 

    // List to store the mappings between dropdown options and Recording/VoiceAudio objects
    private List<(Recording recording, List<VoiceAudio> voiceAudio)> optionMappings;
    private string songUrl;
    private string recordingUrl;
    //public WaveformDisplay waveformDisplay;
    public VideoLoader videoLoader;
    public GameObject recordingPrefab;
    public GameObject displayPanel;
    public GameObject videoDisplayPanel;

    public GameObject loadingPanel;
    public AlertManager messageAlert;
    public AlertManager confirmAlert;

    [SerializeField] VideoPlayer videoPlayer;
    List<AudioSource> audioSources = new();
    string songVideoUrl = "";
    List<string> voiceAudioUrls = new List<string>();
    List<AudioClip> audioClipList = new();
    PurchasedSong purchasedSong;
    bool readyToPlay = false;
    Recording recording;
    [SerializeField] Slider progressSlider;
    [SerializeField] RawImage videoRenderTexture;

    //public RecordingHelper recordingHelper;

    bool isDraggingSlider = false; // To track if the user is dragging the slider

    string audioLocalDirectory = "";

    private void Start()
    {
        audioLocalDirectory = Application.persistentDataPath + "/AudioProcess/";
        var playerId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
        GetRecordingByOwnerId(Guid.Parse(playerId));
        //ApiHelper.Instance.ToString();
        
    }

    public void RefreshRecordingList()
    {
        var playerId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
        GetRecordingByOwnerId(Guid.Parse(playerId));
    }

    public void GetPurchasedSongCoroutine(Guid purchasedSongId)
    {
        ApiHelper.Instance.gameObject
            .GetComponent<PurchasedSongController>()
            .GetPurchasedSongByIdCoroutine(purchasedSongId,
                                            GetSongCoroutine,
                                            Test2
                                            );
    }

    // Dont ask me about the song[0], fk api
    private void GetSongCoroutine(List<PurchasedSong> song)
    {
        ApiHelper.Instance.gameObject
            .GetComponent<SongController>()
            .GetSongByIdCoroutine(song[0].SongId,
                                    SetSongUrl,
                                    Test4
                                    );
    }

    private void SetSongUrl(ResponseResult<SongDetail> song)
    {
        songUrl = song.Value.SongUrl;
        //Process(songUrl, recordingUrl);
    }

    public void GetRecordingByOwnerId(Guid ownerId)
    {
        loadingPanel.SetActive(true);
        ApiHelper.Instance.gameObject
            .GetComponent<RecordingController>()
            .GetRecordingsByOwnerIdCoroutine(ownerId,
                                                (recordingList) =>
                                                {
                                                    RecordingsGenerate(recordingList);
                                                    loadingPanel.gameObject.SetActive(false);
                                                },
                                                Test2
                                                );
    }

    private void Test2(string error)
    {
        Debug.Log(error);
    }

    private void Test4(ResponseResult<SongDetail> detail)
    {
        Debug.Log(detail);
    }

    void RecordingsGenerate(List<Recording> recordingList)
    {
        optionMappings = new List<(Recording recording, List<VoiceAudio> voiceAudio)>();

        foreach (Transform child in displayPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var recording in recordingList)
        {
            GameObject recordingObj = Instantiate(recordingPrefab, displayPanel.transform);

            optionMappings.Add((recording, (List<VoiceAudio>)recording.VoiceAudios));
            recordingObj.GetComponent<RecordingItem>().Init(recording, messageAlert, confirmAlert, this);
            recordingObj.transform.Find("Label 1").GetComponent<TMP_Text>().text = recording.RecordingName + " - " + recording.RecordingType;
            recordingObj.transform.Find("Label 2").GetComponent<TMP_Text>().text = string.Empty;
            recordingObj.transform.Find("Label 2").GetComponent<TMP_Text>().text += "Recording time: " + recording.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss") + "\n";

            recordingObj.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(delegate ()
            {
                OnPlayButtonClicked(recording);
            });

        }
    }
    private void ResetAudioSourceComponent()
    {
        //Reset audio source component
        audioClipList.Clear();
        voiceAudioUrls.Clear();
        audioSources = GetComponents<AudioSource>().ToList();
        foreach (var audioSource in audioSources)
        {
            Destroy(audioSource);
        }
    }

    List<bool> isAudioReady = new List<bool>();
    public void OnPlayButtonClicked(Recording recording)
    {
        StartCoroutine(AutoCancelLoadFail());
        loadingPanel.SetActive(true);
        this.recording = recording;
        songVideoUrl = recording.PurchasedSong.Song.SongUrl;
        audioSources = new();
        List<RecordingHelper> recordingHelpers = gameObject.GetComponents<RecordingHelper>().ToList();
        foreach (RecordingHelper recordingHelper in recordingHelpers)
        {
            Destroy(recordingHelper);
        }    
        foreach (var audio in recording.VoiceAudios)
        {
            isAudioReady.Add(false);
            var localFilePathZip = "AudioProcess/" + audio.VoiceUrl + ".wav";
            if (!File.Exists(localFilePathZip))
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                RecordingHelper recordingHelper = gameObject.AddComponent<RecordingHelper>();
                recordingHelper.PrepareAudioSourceDownloadAudio(
                        audio.VoiceUrl,
                        (audioClip) =>
                        {
                            audioSource.clip = audioClip;
                            audioSources.Add(audioSource);
                            isAudioReady.RemoveAt(0);
                            Debug.Log($"Audio {audio.VoiceUrl} is ready!");
                        },
                        () => { }
                    );
            }
            else
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                RecordingHelper recordingHelper = gameObject.AddComponent<RecordingHelper>();
                recordingHelper.PrepareAudioSourceLoadAudioClip(
                        audio.VoiceUrl,
                        (audioClip) =>
                        {
                            audioSource.clip = audioClip;
                            audioSources.Add(audioSource);
                            isAudioReady.RemoveAt(0);
                            Debug.Log($"Audio {audio.VoiceUrl} is ready!");
                            
                        },
                        () => { loadingPanel.SetActive(false); }

                    );

            }


        }
        StartCoroutine(Prepare());
        //StartCoroutine(Prepare());
        //StartCoroutine(LoadRecording(songVideoUrl, localFilePaths));
    }

    IEnumerator AutoCancelLoadFail()
    {
        yield return new WaitForSeconds(30);
        if (!readyToPlay) {
            StopAllCoroutines();
            loadingPanel.SetActive(false);
            videoDisplayPanel.SetActive(false);
            messageAlert.Alert("Tải bản thu âm thất bại!", false);
        }
    }

    private IEnumerator Prepare()
    {

        Debug.Log("prepare " + isAudioReady.Count);
        yield return new WaitUntil(() => isAudioReady.Count == 0);

        Debug.Log("prepare audio complete ");
        Debug.Log("audioSources " + audioSources.Count);
        videoPlayer.Stop();
        videoPlayer.url = songVideoUrl;
        videoPlayer.Prepare();
        videoPlayer.SetDirectAudioVolume(0, 0.2f);

        readyToPlay = true;
        Play();
        loadingPanel.SetActive(false);

    }
    private void Play()
    {
        if (!readyToPlay)
        {
            return;
        }

        videoDisplayPanel.SetActive(true);
        videoRenderTexture.gameObject.SetActive(true);
        videoPlayer.Play();
        StartCoroutine(SyncVoiceAudioWithVideo());
        StartCoroutine(SyncStart(recording.StartTime, audioSources)) ;
    }

    IEnumerator SyncStart(float videoStartTime, List<AudioSource> audioSources)
    {
        yield return new WaitForSeconds(0.1f);
        if (videoPlayer.time > 0)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.Play();
                audioSource.time = (float)videoPlayer.time;
            }
            if (progressSlider.minValue != 0 || progressSlider.maxValue != (float)videoPlayer.length)
            {
                progressSlider.minValue = 0;
                progressSlider.maxValue = (float)videoPlayer.length;

                StartCoroutine(SliderFollowVideo());
            }

        }
        else
        {
            StartCoroutine(SyncStart(videoStartTime, audioSources));
        }
    }
    IEnumerator SyncVoiceAudioWithVideo()
    {
        yield return new WaitForSeconds(0.5f);
        if (videoPlayer.isPlaying)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                //if (!audioSource.isPlaying) audioSource.Play();
                if (videoPlayer.time < audioSource.clip.length)
                {
                    audioSource.time = (float)videoPlayer.time;
                }
                else
                {
                    audioSource.time = audioSource.clip.length;
                }
                //Debug.Log(videoPlayer.time + " | " + audioSource.time);
            }
        }
    }

    private void Pause()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.Pause();
        }
        videoPlayer.Pause();
    }

    public void Stop()
    {
        ResetAudioSourceComponent();
        videoPlayer.Stop();
        videoRenderTexture.gameObject.SetActive(false);
        progressSlider.value = 0;
    }
    IEnumerator SliderFollowVideo()
    {
        yield return new WaitForSeconds(0.2f);
        if (!isDraggingSlider)
        {
            progressSlider.value = (float)videoPlayer.time;
        }

        StartCoroutine(SliderFollowVideo());
    }
    public void OnSliderDragStart()
    {
        isDraggingSlider = true;
        Pause();
    }

    public void OnSliderDragEnd()
    {
        isDraggingSlider = false;
        if (!readyToPlay)
        {
            return;
        }
        Play();
        videoPlayer.time = progressSlider.value;
    }

    private void OnDisable()
    {
        Stop();
    }

    private void OnDestroy()
    {
        DirectoryInfo dir = new DirectoryInfo(audioLocalDirectory);

        foreach (FileInfo file in dir.GetFiles())
        {
            file.Delete();
        }

        StopAllCoroutines();
    }

    //bool readyToPlay = false;
    //private IEnumerator Prepare()
    //{
    //    yield return new WaitUntil(() => isAudioReady.Count == 0);

    //    Debug.Log("prepare audio complete ");
    //    Debug.Log("audioSources " + audioSources.Count);
    //    videoPlayer.Stop();
    //    videoPlayer.url = songVideoUrl;
    //    videoPlayer.Prepare();
    //    videoPlayer.SetDirectAudioVolume(0, 0.2f);

    //    readyToPlay = true;

    //}

    //public void OnPlayButtonClicked1(Recording recording)
    //{
    //    loadingPanel.SetActive(true);
    //    ApiHelper.Instance.gameObject
    //        .GetComponent<PurchasedSongController>()
    //        .GetPurchasedSongByIdCoroutine(recording.PurchasedSongId,
    //                                        (purchasedSongs) =>
    //                                        {
    //                                            Guid songId = new();
    //                                            string songVideoUrl = "";
    //                                            songId = purchasedSongs[0].SongId;
    //                                            ApiHelper.Instance.gameObject
    //                                                .GetComponent<SongController>()
    //                                                .GetSongByIdCoroutine(songId,
    //                                                                      (sd) =>
    //                                                                      {
    //                                                                          songVideoUrl = sd.Value.SongUrl;
    //                                                                          List<string> localFilePaths = new();
    //                                                                          isAudioReady = new List<bool>();
    //                                                                          foreach (var audio in recording.VoiceAudios)
    //                                                                          {
    //                                                                              var cloudFilePath = audio.VoiceUrl + ".zip";
    //                                                                              var localFilePath = Application.persistentDataPath + "/AudioProcess/" + audio.VoiceUrl + ".zip";
    //                                                                              if (!File.Exists(localFilePath))
    //                                                                              {
    //                                                                                  ffmpeg.DownloadFile2(localFilePath,
    //                                                                                      () => { isAudioReady.RemoveAt(0); Debug.Log($"Audio {audio.VoiceUrl} is ready!"); },
    //                                                                                      () => { }
    //                                                                                  );
    //                                                                              }
    //                                                                              else
    //                                                                              {
    //                                                                                  isAudioReady.RemoveAt(0); Debug.Log($"Audio {audio.VoiceUrl} is ready!");
    //                                                                              }
    //                                                                              localFilePaths.Add(localFilePath);
    //                                                                          }

    //                                                                          StartCoroutine(LoadRecording(songVideoUrl, localFilePaths));

    //                                                                      },
    //                                                                      (ex) => Debug.LogError(ex));
    //                                        },
    //                                        (ex) =>
    //                                        {
    //                                            Debug.LogError(ex);
    //                                            loadingPanel.SetActive(false);
    //                                        }
    //        );
    //}

    //IEnumerator LoadRecording(string songVideoUrl, List<string> voiceAudioUrls)
    //{
    //    yield return new WaitUntil(() => isAudioReady.Count == 0);
    //    PlayVideo(songVideoUrl, voiceAudioUrls);
    //    Debug.Log("Success: " + songVideoUrl);
    //    loadingPanel.SetActive(false);
    //}


    //public void PlayVideo(string videoSongUrl, List<string> voiceAudioUrls)
    //{

    //    //string voiceFolderPath2 = Path.Combine(Application.persistentDataPath + "/AudioProcess/" + recordingUrl + ".wav");
    //    videoDisplayPanel.SetActive(true);
    //    videoLoader.ShowPopup(videoSongUrl, voiceAudioUrls);
    //    //gameObject.SetActive(false);

    //}

    //public IEnumerator EditVideo()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    if (songUrl.IsNullOrEmpty())
    //    {
    //        StartCoroutine(EditVideo());
    //    }
    //    else
    //    {
    //        //Process(songUrl, recordingUrl);
    //    }
    //}

    //public void Show()
    //{
    //    gameObject.SetActive(true);
    //}
}
