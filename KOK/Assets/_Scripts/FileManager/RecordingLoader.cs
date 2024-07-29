using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Recording;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.VoiceAudios;
using System;
using KOK.Assets._Scripts.ApiHandler.Controller;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.PurchasedSong;
using KOK.ApiHandler.DTOModels;
using Photon.Voice;
using System.IO;

public class RecordingLoader : MonoBehaviour
{
    public TMP_Dropdown tmpDropdown; 

    // List to store the mappings between dropdown options and Recording/VoiceAudio objects
    private List<(Recording recording, VoiceAudio voiceAudio)> optionMappings;
    private string songUrl;
    private string recordingUrl;
    public WaveformDisplay waveformDisplay;

    private void Start()
    {
        GetRecordingByOwnerId(Guid.Parse("4375037A-739D-45EF-8021-87EFFACD3FFE"));
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
        Process(songUrl, recordingUrl);
    }

    public void GetRecordingByOwnerId(Guid ownerId)
    {
        FindAnyObjectByType<ApiHelper>().gameObject
            .GetComponent<RecordingController>()
            .GetRecordingsByOwnerIdCoroutine(   ownerId, 
                                                PopulateTMPDropdown, 
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

    public void OnGetButtonClicked()
    {
        int index = tmpDropdown.value;
        if (index >= 0 && index < optionMappings.Count)
        {
            var selectedMapping = optionMappings[index];

            Guid purchasedSongId = selectedMapping.recording.PurchasedSongId;
            GetPurchasedSongCoroutine(purchasedSongId);
            recordingUrl = selectedMapping.voiceAudio.VoiceUrl;
        }
    }

    private void Process(string songUrl, string voiceUrl)
    {
        Debug.Log($"SongUrl: {songUrl}, VoiceUrl: {voiceUrl}");

        FFMPEG ffmpeg = new FFMPEG();
        StartCoroutine(ffmpeg.DownloadFile(voiceUrl, "recordedVoice.wav", "AudioProcess"));
        StartCoroutine(ffmpeg.DownloadFile(songUrl, "Tuý Âm.mp4", "AudioProcess"));

        string videoFolderPath = Path.Combine(Application.persistentDataPath, "AudioProcess", "Tuý Âm.mp4");
        StartCoroutine(ffmpeg.ExtractAudioFromVideo(videoFolderPath));

        waveformDisplay.ShowPopup();
    }
}
