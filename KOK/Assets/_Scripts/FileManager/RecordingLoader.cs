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
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Song;

public class RecordingLoader : MonoBehaviour
{
    //public TMP_Dropdown tmpDropdown; 

    // List to store the mappings between dropdown options and Recording/VoiceAudio objects
    private List<(Recording recording, List<VoiceAudio> voiceAudio)> optionMappings;
    private string songUrl;
    private string recordingUrl;
    public WaveformDisplay waveformDisplay;
    public VideoLoader videoLoader;
    public GameObject recordingPrefab;
    public GameObject displayPanel;

    FFMPEG ffmpeg;

    private void Start()
    {
        var playerId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
        GetRecordingByOwnerId(Guid.Parse(playerId));
        //ApiHelper.Instance.ToString();
        ffmpeg = GetComponent<FFMPEG>();
        if (ffmpeg == null) ffmpeg = gameObject.AddComponent<FFMPEG>();
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
        ApiHelper.Instance.gameObject
            .GetComponent<RecordingController>()
            .GetRecordingsByOwnerIdCoroutine(ownerId,
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

            recordingObj.transform.Find("Label 1").GetComponent<TMP_Text>().text = recording.RecordingName + " - " + recording.RecordingType;
            recordingObj.transform.Find("Label 2").GetComponent<TMP_Text>().text = string.Empty;

            foreach (var voiceAudio in recording.VoiceAudios)
            {
                //string optionText = $"{recording.RecordingName} - {voiceAudio.VoiceUrl}";
                recordingObj.transform.Find("Label 2").GetComponent<TMP_Text>().text += "Recording time: " + recording.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss") + "\n";

            }

            //recordingObj.transform.Find("EditButton").GetComponent<Button>().onClick.AddListener(delegate ()
            //{
            //    OnEditButtonClicked(recording.PurchasedSongId, recording.VoiceAudios);
            //});

            recordingObj.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(delegate ()
            {
                OnPlayButtonClicked(recording);
            });

        }
    }

    public void OnPlayButtonClicked(Recording recording)
    {
        ApiHelper.Instance.gameObject
            .GetComponent<PurchasedSongController>()
            .GetPurchasedSongByIdCoroutine(recording.PurchasedSongId,
                                            (purchasedSongs) =>
                                            {
                                                Guid songId = new();
                                                string songVideoUrl = "";
                                                songId = purchasedSongs[0].SongId;
                                                ApiHelper.Instance.gameObject
                                                    .GetComponent<SongController>()
                                                    .GetSongByIdCoroutine(songId,
                                                                          (sd) =>
                                                                          {
                                                                              songVideoUrl = sd.Value.SongUrl;
                                                                              List<string> localFilePaths = new();
                                                                              foreach (var audio in recording.VoiceAudios)
                                                                              {
                                                                                  var cloudFilePath = audio.VoiceUrl + ".zip";
                                                                                  var localFilePath = Application.persistentDataPath + "/AudioProcess/" + audio.VoiceUrl + ".zip";
                                                                                  ffmpeg.DownloadFile2(localFilePath);
                                                                                  localFilePaths.Add(localFilePath);
                                                                              }

                                                                              StartCoroutine(LoadRecording(songVideoUrl, localFilePaths));

                                                                          },
                                                                          (ex) => Debug.LogError(ex));
                                            },
                                            (ex) => Debug.LogError(ex)
            );
    }

    IEnumerator LoadRecording(string songVideoUrl, List<string> localFilePaths)
    {
        yield return new WaitForSeconds(2);
        PlayVideo(songVideoUrl, localFilePaths);
        Debug.Log("Success: " + songVideoUrl);
    }


    public void PlayVideo(string videoSongUrl, List<string> voiceAudioUrls)
    {

        //string voiceFolderPath2 = Path.Combine(Application.persistentDataPath + "/AudioProcess/" + recordingUrl + ".wav");
        videoLoader.ShowPopup(videoSongUrl, voiceAudioUrls);
        //gameObject.SetActive(false);

    }

    public IEnumerator EditVideo()
    {
        yield return new WaitForSeconds(0.1f);
        if (songUrl.IsNullOrEmpty())
        {
            StartCoroutine(EditVideo());
        }
        else
        {
            //Process(songUrl, recordingUrl);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
