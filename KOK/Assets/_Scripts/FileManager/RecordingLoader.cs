using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.VoiceAudios;
using System;
using KOK.Assets._Scripts.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using Photon.Voice;
using System.IO;
using KOK;
using System.Reflection;
using KOK.Assets._Scripts.FileManager;
using System.Collections;
using WebSocketSharp;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PostComment;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;

public class RecordingLoader : MonoBehaviour
{
    public TMP_Dropdown tmpDropdown; 

    // List to store the mappings between dropdown options and Recording/VoiceAudio objects
    private List<(Recording recording, VoiceAudio voiceAudio)> optionMappings;
    private string songUrl;
    private string recordingUrl;
    public WaveformDisplay waveformDisplay;
    public VideoLoader videoLoader;
    public GameObject recordingPrefab;
    public GameObject displayPanel;

    FFMPEG ffmpeg = new FFMPEG();

    private void Start()
    {
        var playerId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
        GetRecordingByOwnerId(Guid.Parse(playerId));
        //GetRecordingByOwnerId(Guid.Parse("4375037A-739D-45EF-8021-87EFFACD3FFE"));
    }

    public void GetPurchasedSongCoroutine(Guid purchasedSongId)
    {
        FindAnyObjectByType<ApiHelper>().gameObject
            .GetComponent<PurchasedSongController>()
            .GetPurchasedSongByIdCoroutine( purchasedSongId,
                                            GetSongCoroutine,
                                            Test2
                                            );
    }

    private void Test(List<PurchasedSong> song)
    {
        Debug.Log(song[0]);
    }

    // Dont ask me about the song[0], fk api
    private void GetSongCoroutine(List<PurchasedSong> song)
    {
        FindAnyObjectByType<ApiHelper>().gameObject
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
        FindAnyObjectByType<ApiHelper>().gameObject
            .GetComponent<RecordingController>()
            .GetRecordingsByOwnerIdCoroutine(   ownerId, 
                                                RecordingsGenerate, 
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

    private void PopulateTMPDropdown(List<Recording> recordings)
    {
        // Clear existing options
        tmpDropdown.ClearOptions();
        optionMappings = new List<(Recording recording, VoiceAudio voiceAudio)>();

        // Create a list to store new options
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        // Populate the options list
        foreach (var recording in recordings)
        {
            foreach (var voiceAudio in recording.VoiceAudios)
            {
                string optionText = $"{recording.RecordingName} - {voiceAudio.VoiceUrl}";
                options.Add(new TMP_Dropdown.OptionData(optionText));
                optionMappings.Add((recording, voiceAudio));
            }
        }

        tmpDropdown.AddOptions(options);
    }

    void RecordingsGenerate(List<Recording> recordingList)
    {
        optionMappings = new List<(Recording recording, VoiceAudio voiceAudio)>();

        foreach (Transform child in displayPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var recording in recordingList)
        {
            foreach (var voiceAudio in recording.VoiceAudios)
            {
                //string optionText = $"{recording.RecordingName} - {voiceAudio.VoiceUrl}";
                optionMappings.Add((recording, voiceAudio));
                GameObject recordingObj = Instantiate(recordingPrefab, displayPanel.transform);
                recordingObj.transform.Find("Label 1").GetComponent<TMP_Text>().text = recording.RecordingName;
                recordingObj.transform.Find("Label 2").GetComponent<TMP_Text>().text = voiceAudio.VoiceUrl;
                var purchasedSongId = recording.PurchasedSongId;
                var startTimeRecording = recording.StartTime;
                var startTimeVoiceAudio = voiceAudio.StartTime;
                recordingObj.transform.Find("EditButton").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    OnEditButtonClicked(purchasedSongId, voiceAudio.VoiceUrl);
                });

                recordingObj.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    OnPlayButtonClicked(purchasedSongId, voiceAudio.VoiceUrl, startTimeRecording, startTimeVoiceAudio);
                });
            }
        }
    }

    public void OnEditButtonClicked(Guid purchasedSongId, string voiceUrl)
    {
        /*int index = tmpDropdown.value;
        if (index >= 0 && index < optionMappings.Count)
        {
            var selectedMapping = optionMappings[index];

            Guid purchasedSongId = selectedMapping.recording.PurchasedSongId;
            GetPurchasedSongCoroutine(purchasedSongId);
            recordingUrl = selectedMapping.voiceAudio.VoiceUrl;
            //Process(songUrl, recordingUrl);
            StartCoroutine(test2());
        }*/
        GetPurchasedSongCoroutine(purchasedSongId);
        recordingUrl = voiceUrl;
        StartCoroutine(test2());
    }

    // chua down
    public void OnPlayButtonClicked(Guid purchasedSongId, string voiceUrl, float startTimeRecording, float startTimeVoiceAudio)
    {
        GetPurchasedSongCoroutine(purchasedSongId);
        recordingUrl = voiceUrl;
        string voiceFolderPath = Path.Combine(Application.persistentDataPath + "/AudioProcess/" + recordingUrl + ".wav");
        StartCoroutine(test(voiceFolderPath, startTimeRecording, startTimeVoiceAudio, recordingUrl));
    }

    public IEnumerator test(string voiceFolderPath, float startTimeRecording, float startTimeVoiceAudio,string recordingUrl)
    {
        yield return new WaitForSeconds(0.1f);
        if(songUrl.IsNullOrEmpty())
        {
            StartCoroutine(test(voiceFolderPath, startTimeRecording, startTimeVoiceAudio, recordingUrl));
        }
        else
        {
            string voiceFolderPath2 = Path.Combine(Application.persistentDataPath + "/AudioProcess/" + recordingUrl + ".wav");
            videoLoader.ShowPopup(songUrl, voiceFolderPath2, startTimeRecording, startTimeVoiceAudio);
            gameObject.SetActive(false);
        }
    }

    public IEnumerator test2()
    {
        yield return new WaitForSeconds(0.1f);
        if (songUrl.IsNullOrEmpty())
        {
            StartCoroutine(test2());
        }
        else
        {
            Process(songUrl, recordingUrl);
        }
    }

    private async void Process(string songUrl, string voiceUrl)
    {
        Debug.Log($"SongUrl: {songUrl}, VoiceUrl: {voiceUrl}");
        
        var recordingName = recordingUrl + ".zip";
        string folderPath = Path.Combine(Application.persistentDataPath + "/AudioProcess/" + recordingName);
        await ffmpeg.DownloadFile2(recordingName, folderPath);
        //StartCoroutine(ffmpeg.DownloadFile(voiceUrl, "recordedVoice.wav", "AudioProcess"));
        try { StartCoroutine(ffmpeg.DownloadFile(songUrl, "downloadedVideo.mp4", "AudioProcess")); }
        catch { }
        

        string videoFolderPath = Path.Combine(Application.persistentDataPath, "AudioProcess", "downloadedVideo.mp4");
        try { StartCoroutine(ffmpeg.ExtractAudioFromVideo(videoFolderPath)); }
        catch { }

        waveformDisplay.ShowPopup(Path.Combine(Application.persistentDataPath + "/AudioProcess/" + recordingUrl + ".wav"));
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
