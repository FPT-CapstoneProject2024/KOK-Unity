﻿using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Assets._Scripts.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

namespace KOK.Assets._Scripts.Posts
{
    public class PostLoader : MonoBehaviour
    {
        private string songUrl;
        public VideoLoader videoLoader;

        public void GetRecordingById(Guid recordingId)
        {
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<RecordingController>()
                .GetRecordingsByIdCoroutine2( recordingId,
                                            LoadVideo,
                                            test2
                                            );
        }

        public void GetPurchasedSongCoroutine(Guid purchasedSongId)
        {
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<PurchasedSongController>()
                .GetPurchasedSongByIdCoroutine(purchasedSongId,
                                                GetSongCoroutine,
                                                OnError2
                                                );
        }

        // Dont ask me about the song[0], fk api
        private void GetSongCoroutine(List<PurchasedSong> song)
        {
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<SongController>()
                .GetSongByIdCoroutine(song[0].SongId,
                                        SetSongUrl,
                                        OnError3
                                        );
        }

        private void SetSongUrl(ResponseResult<SongDetail> song)
        {
            songUrl = song.Value.SongUrl;
            //Process(songUrl, recordingUrl);
        }

        // chua down
        private void LoadVideo(List<Recording> recordings)
        {
            foreach (var recording in recordings)
            {
                foreach (var voiceAudio in recording.VoiceAudios)
                {
                    GetPurchasedSongCoroutine(recording.PurchasedSongId);
                    var startTimeSong = recording.StartTime;
                    var startTimeVoiceAudio = voiceAudio.StartTime;
                    var voiceUrl = voiceAudio.VoiceUrl;
                    string voiceFolderPath = Path.Combine(Application.persistentDataPath + "/AudioProcess/" + voiceUrl + ".wav");
                    StartCoroutine(PlayVideo(voiceFolderPath, startTimeVoiceAudio, startTimeSong, voiceUrl));
                }
            }
        }

        public IEnumerator PlayVideo(string voiceFolderPath, float startTimeVoiceAudio, float startTimeSong, string voiceUrl)
        {
            yield return new WaitForSeconds(0.1f);
            if (songUrl.IsNullOrEmpty())
            {
                StartCoroutine(PlayVideo(voiceFolderPath, startTimeVoiceAudio, startTimeSong, voiceUrl));
            }
            else
            {
                string voiceFolderPath2 = Path.Combine(Application.persistentDataPath + "/AudioProcess/" + voiceUrl + ".wav");
                videoLoader.Load(songUrl, voiceFolderPath2, startTimeVoiceAudio, startTimeSong);
            }
        }

        private void test2 (string error)
        {
            Debug.Log (error);
        }

        private void OnError2(string error)
        {
            Debug.Log(error);
        }

        private void OnError3(ResponseResult<SongDetail> detail)
        {
            Debug.Log(detail);
        }
    }
}